using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyController : MonoBehaviour
{
    private const float Tick = 1/30;
    private float _damagingTimer;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Transform _player;
    private EnemySpawner _enemySpawner;
    [SerializeField] private GameObject _experiencePrefab;
    
    [SerializeField] private EnemyVariant _enemyVariant;
    public Enemy EnemyStats { get; private set; }
    public bool IsDead { get; private set; } = false;

    void Awake()
    {
        EnemyStats = new Enemy(_enemyVariant);
    }

    void Start()
    {
        _damagingTimer = Tick;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _player = GameObject.Find("Player").GetComponent<Transform>();
        _enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
    }

    void Update()
    {
        if (EnemyStats.CurrentHealth <= 0 && !IsDead)
        {
            TriggerDeath();
        }

        // дистанция до игрока
        float distance = Vector2.Distance(_player.position, transform.position);
        if (distance > 15)
        {
            transform.position = _enemySpawner.CalculateSpawnPosition();
        }

        Vector2 direction = _player.position - transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        Vector3 targetRotation = new(0, 0, angle);
        // дополнительное поведение для быстрых врагов
        if (_enemyVariant == EnemyVariant.Fast && distance > 3.5f)
        {
            float turnSpeed = EnemyStats.TurnSpeed * 10.0f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), turnSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), EnemyStats.TurnSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = transform.up * EnemyStats.MoveSpeed;
        _rb.velocity = velocity;
        if (!IsDead)
        {
            _animator.SetBool("IsMoving", true);
        }
        else
        {
            _animator.SetBool("IsMoving", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Collision happened!");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Projectile") && !IsDead) {
            ProjectileController projectile = other.gameObject.GetComponent<ProjectileController>();
            TakeDamage(projectile.ProjectileStats.KineticDamage, projectile.ProjectileStats.EnergeticDamage);
            Debug.Log("Enemy took damage from projectile: " + projectile.GetProjectileVariant() + " penetration: " + projectile.ProjectileStats.PenetrationCount);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile") && !IsDead) {
            ProjectileController projectile = other.gameObject.GetComponent<ProjectileController>();
            if (projectile.IsKinetic())
            {
                return;
            }

            if (_damagingTimer > 0)
            {
                _damagingTimer -= Time.deltaTime;
                return;
            }
            _damagingTimer = Tick;

            TakeDamage(projectile.ProjectileStats.KineticDamage, projectile.ProjectileStats.EnergeticDamage);
        }
    }

    void SpawnExperience()
    {
        GameObject experience = Instantiate(_experiencePrefab, transform.position, Quaternion.identity);
        experience.GetComponent<ExperienceController>().ExperienceCount = EnemyStats.ExperienceAmount;
        
        Transform expTransform = experience.GetComponent<Transform>();
        BoxCollider2D expBoxCollider2D = experience.GetComponent<BoxCollider2D>();
        SpriteRenderer expSpriteRenderer = experience.GetComponent<SpriteRenderer>();

        float sizeMultiplier = 1;
        Color color = new(1, 1, 1);

        if (EnemyStats.ExperienceAmount >= 10)
        {
            sizeMultiplier = 2.0f;
            color = new Color(1, 1, 0.4f);
        }
        else if (EnemyStats.ExperienceAmount >= 5)
        {
            sizeMultiplier = 1.5f;
            color = new Color(1, 1, 0.8f);
        }
        
        expTransform.localScale *= sizeMultiplier;
        expBoxCollider2D.size *= sizeMultiplier;
        expSpriteRenderer.color = color;
    }

    public void TakeDamage(float kineticDamage, float energeticDamage)
    {
        float resultKineticDamage = (kineticDamage - EnemyStats.KineticDamageResistance.Block) * EnemyStats.KineticDamageResistance.Reduction;
        float resultEnergeticDamage = (energeticDamage - EnemyStats.EnergeticDamageResistance.Block) * EnemyStats.EnergeticDamageResistance.Reduction;
        EnemyStats.CurrentHealth -= resultKineticDamage + resultEnergeticDamage;
        if (EnemyStats.CurrentHealth < 0)
        {
            EnemyStats.CurrentHealth = 0;
        }
        if (EnemyStats.CurrentHealth > EnemyStats.MaxHealth)
        {
            EnemyStats.CurrentHealth = EnemyStats.MaxHealth;
        }
        if (EnemyStats.CurrentHealth <= 0)
        {
            TriggerDeath();
        }
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        EnemyStats.CurrentHealth = currentHealth;
        EnemyStats.MaxHealth = maxHealth;
    }

    public void SetHealthMultiplier(float healthMultiplier)
    {
        SetHealth(EnemyStats.CurrentHealth * healthMultiplier, EnemyStats.MaxHealth * healthMultiplier);
    }

    void TriggerDeath()
    {
        IsDead = true;
        _animator.SetBool("IsDead", true);
        EnemyStats.MoveSpeed = 0;
        EnemyStats.TurnSpeed = 0;
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        GetComponent<PolygonCollider2D>().enabled = false;
        Invoke(nameof(DestroyEnemy), 0.70f);
        _enemySpawner.KillEnemyAndAddCurrency(EnemyStats.CurrencyAmount);
    }

    void DestroyEnemy()
    {
        SpawnExperience();
        Destroy(gameObject);
    }
}
