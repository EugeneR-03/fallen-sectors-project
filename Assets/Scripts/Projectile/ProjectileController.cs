using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    protected KineticSystem _kineticSystem;
    protected EnergeticSystem _energeticSystem;

    protected GameObject _player;
    protected Rigidbody2D _rb;
    protected Vector2 _direction;
    [SerializeField] protected ProjectileVariant _projectileVariant;
    public Projectile ProjectileStats { get; set; }

    private void Awake()
    {
        ProjectileStats = new Projectile(_projectileVariant);
    }

    private void Start()
    {
        if (IsKinetic())
        {
            _kineticSystem = new KineticSystem(gameObject);
        }
        if (IsEnergetic())
        {
            _energeticSystem = new EnergeticSystem(gameObject);
        }
        
        _player = GameObject.Find("Player");
        _rb = GetComponent<Rigidbody2D>();
        _direction = transform.up;

        Vector2 force = _direction * ProjectileStats.MoveSpeed;
        _rb.AddForce(force, ForceMode2D.Impulse);

        SpecificStart(_projectileVariant);

        if (ProjectileStats.LifeTime > 0 && _projectileVariant != ProjectileVariant.KineticProjectile4)
        {
            Invoke(nameof(DestroyProjectile), ProjectileStats.LifeTime);
        }
    }

    private void FixedUpdate()
    {
        SpecificFixedUpdate(_projectileVariant);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _kineticSystem?.OnTriggerEnter2D(other);
        _energeticSystem?.OnTriggerEnter2D(other);
    }

    private void SpecificStart(ProjectileVariant variant)
    {
        switch (variant)
        {
            case ProjectileVariant.KineticProjectile1:
                break;

            case ProjectileVariant.KineticProjectile2:
                break;

            case ProjectileVariant.KineticProjectile3:
                break;

            case ProjectileVariant.KineticProjectile4:
                break;

            case ProjectileVariant.EnergeticProjectile1:
                break;

            case ProjectileVariant.EnergeticProjectile2:
                break;

            case ProjectileVariant.EnergeticProjectile3:
                Vector3 originalScale = transform.localScale;
                transform.localScale = new Vector3(ProjectileStats.ExplosionRadius, ProjectileStats.ExplosionRadius, originalScale.z);
                _energeticSystem._animator.speed = _energeticSystem._animator.runtimeAnimatorController.animationClips.First(x => x.name.EndsWith("Live")).length / ProjectileStats.LifeTime;
                break;

            default:
                break;
        }
    }

    private void SpecificFixedUpdate(ProjectileVariant variant)
    {
        switch (_projectileVariant)
        {
            case ProjectileVariant.KineticProjectile1:
                break;

            case ProjectileVariant.KineticProjectile2:
                break;

            case ProjectileVariant.KineticProjectile3:
                break;

            case ProjectileVariant.KineticProjectile4:
                break;

            case ProjectileVariant.EnergeticProjectile1:
                AutoNavigateToClosestEnemy();
                break;

            case ProjectileVariant.EnergeticProjectile2:
                break;

            case ProjectileVariant.EnergeticProjectile3:
                break;

            default:
                break;
        }
    }

    protected void AutoNavigateToClosestEnemy()
    {
        GameObject closestEnemy = EnemySpawner.FindClosestEnemy(transform.position);
        if (closestEnemy != null)
        {
            Vector2 direction = closestEnemy.transform.position - transform.position;
            float angle = Vector2.SignedAngle(Vector2.up, direction);
            Vector3 targetRotation = new(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), ProjectileStats.TurnSpeed * Time.deltaTime);
            _rb.velocity = transform.up * ProjectileStats.MoveSpeed;
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    public ProjectileVariant GetProjectileVariant()
    {
        return _projectileVariant;
    }

    public void SetProjectileVariant(ProjectileVariant variant)
    {
        _projectileVariant = variant;
    }

    public bool IsKinetic()
    {
        return _projectileVariant switch
        {
            ProjectileVariant.KineticProjectile1 => true,
            ProjectileVariant.KineticProjectile2 => true,
            ProjectileVariant.KineticProjectile3 => true,
            ProjectileVariant.KineticProjectile4 => true,
            ProjectileVariant.EnergeticProjectile1 => true,     // ракеты гибридное оружие, они взрываются при контакте
            _ => false
        };
    }

    public bool IsEnergetic()
    {
        return _projectileVariant switch
        {
            ProjectileVariant.EnergeticProjectile2 => true,
            ProjectileVariant.EnergeticProjectile3 => true,
            _ => false
        };
    }

    public class KineticSystem
    {
        private GameObject _projectile;
        private ProjectileController _projectileController;
        private Projectile ProjectileStats;
        private Animator _animator;
        private BoxCollider2D _boxCollider2D;
        private CircleCollider2D _circleCollider2D;
        private Rigidbody2D _rb;
        private bool _isExploded;

        public KineticSystem(GameObject projectile)
        {
            _projectile = projectile;
            _projectileController = _projectile.GetComponent<ProjectileController>();
            ProjectileStats = _projectileController.ProjectileStats;
            _animator = _projectile.GetComponent<Animator>();
            _boxCollider2D = _projectile.GetComponent<BoxCollider2D>();
            _circleCollider2D = _projectile.GetComponent<CircleCollider2D>();
            _rb = _projectile.GetComponent<Rigidbody2D>();
            _isExploded = false;
        }
        
        private void BreakThrough(int penetrationCount)
        {
            // снаряды рельсотрона не трогаем
            if (_projectileController.GetProjectileVariant() == ProjectileVariant.KineticProjectile4)
            {
                return;
            }

            // взрывающиеся снаряды взрываем
            if (ProjectileStats.ExplosionRadius > 0)
            {
                Explode();
                return;
            }

            ProjectileStats.PenetrationCount -= penetrationCount;
            Debug.Log("Projectile penetration count: " + ProjectileStats.PenetrationCount);
            if (ProjectileStats.PenetrationCount <= 0)
            {
                _projectileController.Invoke(nameof(_projectileController.DestroyProjectile), 0f);
            }
        }

        private void Explode()
        {
            if (_isExploded ||_animator == null || _circleCollider2D == null)
            {
                return;
            }

            _rb.velocity = Vector2.zero;
            ProjectileStats.MoveSpeed = 0f;
            ProjectileStats.TurnSpeed = 0f;

            _animator.SetBool("IsExploding", true);
            float animationSpeed = _animator.runtimeAnimatorController.animationClips.First(x => x.name.EndsWith("Death")).length;
            float animationSpeedMultiplier = 2f;
            float livingTimeForPlayingAnimation = animationSpeed / animationSpeedMultiplier;

            _animator.speed = animationSpeed;
            _animator.SetFloat("AnimationSpeed", animationSpeedMultiplier);
            Debug.Log("Animation speed: " + animationSpeed);
            
            Vector3 originalScale = _projectile.transform.localScale;
            _projectile.transform.localScale = new Vector3(ProjectileStats.ExplosionRadius, ProjectileStats.ExplosionRadius, originalScale.z);

            _boxCollider2D.enabled = false;
            _circleCollider2D.enabled = true;
            _isExploded = true;

            _projectileController.Invoke(nameof(_projectileController.DestroyProjectile), animationSpeed);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController.EnemyStats.Variant == EnemyVariant.Armored)
                {
                    BreakThrough(5);
                }
                else
                {
                    BreakThrough(1);
                }
            }
        }
    }

    protected class EnergeticSystem
    {
        private GameObject _projectile;
        private ProjectileController _projectileController;
        private Projectile ProjectileStats;
        public Animator _animator;
        private BoxCollider2D _boxCollider2D;
        private CircleCollider2D _circleCollider2D;
        private Rigidbody2D _rb;

        public EnergeticSystem(GameObject projectile)
        {
            _projectile = projectile;
            _projectileController = _projectile.GetComponent<ProjectileController>();
            ProjectileStats = _projectileController.ProjectileStats;
            _animator = _projectile.GetComponent<Animator>();
            _boxCollider2D = _projectile.GetComponent<BoxCollider2D>();
            _circleCollider2D = _projectile.GetComponent<CircleCollider2D>();
            _rb = _projectile.GetComponent<Rigidbody2D>();
        }

        private void GoThrough() {}

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                GoThrough();
            }
        }
    }
}