using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    private PolygonCollider2D _polygonCollider;
    private Animator _animator;
    private CircleCollider2D _circleCollider;
    private Image _healthBar;
    private Image _experienceBar;
    private GameObject[] _weapons;
    private WeaponController[] _weaponControllers;

    private int _weaponsEnergyUnits;
    private int _enginesEnergyUnits;
    private int _shieldsEnergyUnits;

    private const float PeriodOfInvulnerability = 0.5f;
    private float _damagingTimer;
    private bool _isAutoAiming;
    private int _level;
    private const float ExperienceDifference = 2f;
    private float _experienceToLevelUp;
    private float _experienceGathered;

    private GameObject _activeShipPrefab;
    [SerializeField] private GameObject _shipFighterPrefab;
    [SerializeField] private GameObject _shipFrigatePrefab;

    [SerializeField] private UIController _uiController;
    [SerializeField] private ReactorEnergyController _reactorEnergyController;
    [SerializeField] private ShipVariant _shipVariant = ShipVariant.None;
    public Ship PlayerStats { get; private set; }

    void Awake()
    {
        _shipVariant = ShipVariant.Fighter;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _polygonCollider = GetComponent<PolygonCollider2D>();
        _animator = GetComponent<Animator>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
        _experienceBar = GameObject.Find("ExperienceBar").GetComponent<Image>();

        _damagingTimer = 1.0f;
        _isAutoAiming = false;
        _level = 1;
        _experienceToLevelUp = 10f;
        _experienceGathered = 0f;

        _weapons = gameObject.GetComponentsInChildren<WeaponController>(true)
            .Select(x => x.gameObject)
            .OrderBy(x => x.name)
            .ToArray();
        
        _weaponControllers = new WeaponController[_weapons.Length];
    }

    void Start()
    {
        Debug.Log(_weapons.Length);
        
        for (int i = 0; i < _weapons.Length; i++)
        {
            _weaponControllers[i] = _weapons[i].GetComponent<WeaponController>();
            _weaponControllers[i].SetWeaponVariant(PlayerSelectionController.GetWeaponVariant(i + 1));
        }

        _weaponsEnergyUnits = _reactorEnergyController.GetWeaponsEnergyUnitsCount();
        _enginesEnergyUnits = _reactorEnergyController.GetEnginesEnergyUnitsCount();
        _shieldsEnergyUnits = _reactorEnergyController.GetShieldsEnergyUnitsCount();

        _shipVariant = PlayerSelectionController.GetShipType();
        SetShipType(_shipVariant);
        SetConstantUpgrades();
        Debug.Log("MaxHealth: " + PlayerStats.MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (_damagingTimer <= PeriodOfInvulnerability)
        {
            _damagingTimer += Time.deltaTime;
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isAutoAiming = !_isAutoAiming;
        }

        if (_isAutoAiming && !Input.GetMouseButton(0))
        {
            GameObject closestEnemy = EnemySpawner.FindClosestEnemy(transform.position);
            if (closestEnemy != null)
            {
                Vector2 direction = closestEnemy.transform.position - transform.position;
                float angle = Vector2.SignedAngle(Vector2.up, direction);
                Vector3 targetRotation = new(0, 0, angle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), PlayerStats.TurnSpeed * Time.deltaTime);
            }
        }
        else
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - transform.position;
            float angle = Vector2.SignedAngle(Vector2.up, direction);
            Vector3 targetRotation = new(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), PlayerStats.TurnSpeed * Time.deltaTime * GetEnergyUnitsMultiplier(_enginesEnergyUnits));
        }
    }

    private void FixedUpdate() {
        Vector2 pVelocity = Vector2.zero;
        if (Input.GetKey (KeyCode.W)){
            pVelocity += new Vector2(0, 1);
        }
        if (Input.GetKey (KeyCode.S)){
            pVelocity += new Vector2(0, -1);
        }
        if (Input.GetKey (KeyCode.A)){
            pVelocity += new Vector2(-1, 0);
        }
        if (Input.GetKey (KeyCode.D)){
            pVelocity += new Vector2(1, 0);
        }

        if (pVelocity == Vector2.zero) {
            _animator.SetBool("IsMoving", false);
            _rb.velocity = Vector2.zero;
        }
        else {
            _animator.SetBool("IsMoving", true);
            Vector2 velocity = GetEnergyUnitsMultiplier(_enginesEnergyUnits) * PlayerStats.MoveSpeed * Vector2.ClampMagnitude(pVelocity, 1.0f);
            _rb.velocity = velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy") && _polygonCollider.IsTouching(other)) {
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            TakeDamage(enemyController.EnemyStats.KineticDamage, enemyController.EnemyStats.EnergeticDamage);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy") && _polygonCollider.IsTouching(other)) {
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            TakeDamage(enemyController.EnemyStats.KineticDamage, enemyController.EnemyStats.EnergeticDamage);
        }
    }

    public void Heal(float healAmount)
    {
        PlayerStats.CurrentHealth += healAmount;
        if (PlayerStats.CurrentHealth > PlayerStats.MaxHealth)
        {
            PlayerStats.CurrentHealth = PlayerStats.MaxHealth;
        }
        _healthBar.fillAmount = PlayerStats.CurrentHealth / PlayerStats.MaxHealth;
    }

    public void TakeDamage(float kineticDamage, float energeticDamage)
    {
        if (_damagingTimer <= PeriodOfInvulnerability)
        {
            return;
        }
        else
        {
            _damagingTimer = 0.0f;
        }

        float resultKineticDamage = (kineticDamage - PlayerStats.KineticDamageResistance.Block) * PlayerStats.KineticDamageResistance.Reduction;
        float resultEnergeticDamage = (energeticDamage - PlayerStats.EnergeticDamageResistance.Block) * PlayerStats.EnergeticDamageResistance.Reduction;
        PlayerStats.CurrentHealth -= resultKineticDamage + resultEnergeticDamage;
        if (PlayerStats.CurrentHealth < 0)
        {
            PlayerStats.CurrentHealth = 0;
        }
        if (PlayerStats.CurrentHealth > PlayerStats.MaxHealth)
        {
            PlayerStats.CurrentHealth = PlayerStats.MaxHealth;
        }
        _healthBar.fillAmount = PlayerStats.CurrentHealth / PlayerStats.MaxHealth;
        if (PlayerStats.CurrentHealth <= 0)
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        _uiController.ShowEndMenu();
    }

    public void GatherExperience(float experienceCount)
    {
        _experienceGathered += experienceCount;
        if (_experienceGathered >= _experienceToLevelUp)
        {   
            LevelUp();
        }
        else
        {
            _experienceBar.fillAmount = _experienceGathered / _experienceToLevelUp;
        }
    }

    public void LevelUp()
    {
        int gatheredLevels = (int)(_experienceGathered / _experienceToLevelUp);
        _level += gatheredLevels;
        _experienceGathered %= _experienceToLevelUp;
        _experienceToLevelUp += ExperienceDifference;
        _experienceBar.fillAmount = _experienceGathered / _experienceToLevelUp;
        while (gatheredLevels > 0)
        {
            _uiController.UpdatePlayerLevel(_level);
            _uiController.ShowUpgradesMenu();
            gatheredLevels--;
        }
    }

    public void UpgradeStats(TemporaryUpgrade upgrade)
    {
        switch (upgrade.Variant)
        {
            // tier 1
            case TemporaryUpgradeVariant.HealthUpgrade:
                PlayerStats.MaxHealth += upgrade.Value;
                Heal(upgrade.Value);
                break;

            case TemporaryUpgradeVariant.KineticDamageBlockUpgrade:
                PlayerStats.KineticDamageResistance = new DamageResistance(PlayerStats.KineticDamageResistance.Block + upgrade.Value, PlayerStats.KineticDamageResistance.Reduction);
                break;

            case TemporaryUpgradeVariant.KineticDamageReductionUpgrade:
                PlayerStats.KineticDamageResistance = new DamageResistance(PlayerStats.KineticDamageResistance.Block, PlayerStats.KineticDamageResistance.Reduction * upgrade.Value);
                break;

            case TemporaryUpgradeVariant.EnergeticDamageBlockUpgrade:
                PlayerStats.EnergeticDamageResistance = new DamageResistance(PlayerStats.EnergeticDamageResistance.Block + upgrade.Value, PlayerStats.EnergeticDamageResistance.Reduction);
                break;

            case TemporaryUpgradeVariant.EnergeticDamageReductionUpgrade:
                PlayerStats.EnergeticDamageResistance = new DamageResistance(PlayerStats.EnergeticDamageResistance.Block, PlayerStats.EnergeticDamageResistance.Reduction * upgrade.Value);
                break;

            // tier 2
            case TemporaryUpgradeVariant.MoveSpeedUpgrade:
                PlayerStats.MoveSpeed *= upgrade.Value;
                break;

            case TemporaryUpgradeVariant.TurnSpeedUpgrade:
                PlayerStats.TurnSpeed *= upgrade.Value;
                break;

            case TemporaryUpgradeVariant.ExperienceGatheringRadiusUpgrade:
                PlayerStats.ExperienceGatheringRadius *= upgrade.Value;
                _circleCollider.radius = PlayerStats.ExperienceGatheringRadius;
                break;

            case TemporaryUpgradeVariant.AttackRangeUpgrade:
                foreach (WeaponController weaponController in _weaponControllers)
                {
                    if (weaponController.WeaponStats.Variant == WeaponVariant.LaserCannon)
                        continue;
                    
                    weaponController.ProjectileStats.LifeTime *= upgrade.Value;
                }
                break;

            // tier 3
            case TemporaryUpgradeVariant.KineticDamageUpgrade:
                foreach (WeaponController weaponController in _weaponControllers)
                {
                    weaponController.ProjectileStats.KineticDamage *= upgrade.Value;
                }
                break;

            case TemporaryUpgradeVariant.EnergeticDamageUpgrade:
                foreach (WeaponController weaponController in _weaponControllers)
                {
                    weaponController.ProjectileStats.EnergeticDamage *= upgrade.Value;
                }
                break;

            case TemporaryUpgradeVariant.AttackSpeedUpgrade:
                foreach (WeaponController weaponController in _weaponControllers)
                {
                    if (weaponController.WeaponStats.Variant == WeaponVariant.LaserCannon)
                        continue;
                    
                    weaponController.WeaponStats.FireRate *= upgrade.Value;
                }
                break;

            case TemporaryUpgradeVariant.PenetrationCountUpgrade:
                foreach (WeaponController weaponController in _weaponControllers)
                {
                    weaponController.ProjectileStats.PenetrationCount += (int)upgrade.Value;
                    Debug.Log("PenetrationCount: "+weaponController.ProjectileStats.PenetrationCount);
                }
                break;

            case TemporaryUpgradeVariant.ExplosionRadiusUpgrade:
                foreach (WeaponController weaponController in _weaponControllers)
                {
                    weaponController.ProjectileStats.ExplosionRadius *= upgrade.Value;
                }
                break;

            default:
                break;
        }
    }

    public float GetEnergyUnitsMultiplier(int energyUnitsCount)
    {
        switch (energyUnitsCount)
        {
            case 1:
                return 0.2f;
            case 2:
                return 0.4f;
            case 3:
                return 0.6f;
            case 4:
                return 0.8f;
            case 5:
                return 1.0f;
            case 6:
                return 1.2f;
            case 7:
                return 1.4f;
            case 8:
                return 1.6f;
            default:
                return 0.0f;
        }
    }

    public float GetWeaponsEnergyUnitsMultiplier()
    {
        return GetEnergyUnitsMultiplier(_weaponsEnergyUnits);
    }

    public void SetWeaponsEnergyUnits(int energyUnitsCount)
    {
        _weaponsEnergyUnits = energyUnitsCount;
    }

    public float GetEnginesEnergyUnitsMultiplier()
    {
        return GetEnergyUnitsMultiplier(_enginesEnergyUnits);
    }

    public void SetEnginesEnergyUnits(int energyUnitsCount)
    {
        _enginesEnergyUnits = energyUnitsCount;
    }

    public float GetShieldsEnergyUnitsMultiplier()
    {
        return GetEnergyUnitsMultiplier(_shieldsEnergyUnits);
    }

    public void SetShieldsEnergyUnits(int energyUnitsCount)
    {
        _shieldsEnergyUnits = energyUnitsCount;
    }

    private void SetConstantUpgrade(ConstantUpgrade constantUpgrade)
    {
        switch (constantUpgrade.Variant)
        {
            case ConstantUpgradeVariant.None:
                break;

            case ConstantUpgradeVariant.HealthUpgrade:
                PlayerStats.MaxHealth += constantUpgrade.Value;
                Heal(constantUpgrade.Value);
                break;

            case ConstantUpgradeVariant.KineticDamageBlockUpgrade:
                PlayerStats.KineticDamageResistance = new DamageResistance(PlayerStats.KineticDamageResistance.Block + constantUpgrade.Value, PlayerStats.KineticDamageResistance.Reduction);
                break;

            case ConstantUpgradeVariant.EnergeticDamageBlockUpgrade:
                PlayerStats.EnergeticDamageResistance = new DamageResistance(PlayerStats.EnergeticDamageResistance.Block + constantUpgrade.Value, PlayerStats.EnergeticDamageResistance.Reduction);
                break;

            case ConstantUpgradeVariant.MoveSpeedUpgrade:
                PlayerStats.MoveSpeed *= constantUpgrade.Value;
                break;

            case ConstantUpgradeVariant.KineticDamageUpgrade:
                foreach (WeaponController weaponController in _weaponControllers)
                {
                    weaponController.ProjectileStats.KineticDamage *= constantUpgrade.Value;
                }
                break;

            case ConstantUpgradeVariant.EnergeticDamageUpgrade:
                foreach (WeaponController weaponController in _weaponControllers)
                {
                    weaponController.ProjectileStats.EnergeticDamage *= constantUpgrade.Value;
                }
                break;

            case ConstantUpgradeVariant.AttackSpeedUpgrade:
                foreach (WeaponController weaponController in _weaponControllers)
                {
                    if (weaponController.WeaponStats.Variant == WeaponVariant.LaserCannon)
                        continue;
                    
                    weaponController.WeaponStats.FireRate *= constantUpgrade.Value;
                }
                break;

            case ConstantUpgradeVariant.TurnSpeedUpgrade:
                PlayerStats.TurnSpeed *= constantUpgrade.Value;
                break;

            default:
                break;
        }
    }

    public void SetConstantUpgrades()
    {
        var constantUpgrades = UserDataController.Instance.GetUserData().ConstantUpgrades;

        foreach (var kvp in constantUpgrades)
        {
            SetConstantUpgrade(kvp.Value);
        }
    }

    public void SetShipType(ShipVariant shipVariant)
    {
        _shipVariant = shipVariant;
        PlayerStats = new Ship(shipVariant);

        switch (shipVariant)
        {
            case ShipVariant.None:
                gameObject.SetActive(false);
                break;
            case ShipVariant.Fighter:
                _activeShipPrefab = _shipFighterPrefab;
                break;
            case ShipVariant.Frigate:
                _activeShipPrefab = _shipFrigatePrefab;
                break;
        }

        _spriteRenderer.sprite = _activeShipPrefab.GetComponent<SpriteRenderer>().sprite;
        _polygonCollider.points = _activeShipPrefab.GetComponent<PolygonCollider2D>().points;
        _animator.runtimeAnimatorController = _activeShipPrefab.GetComponent<Animator>().runtimeAnimatorController;
        _circleCollider.radius = PlayerStats.ExperienceGatheringRadius;

        GameObject[] activeShipPrefabWeapons = _activeShipPrefab.GetComponentsInChildren<WeaponController>(true)
            .Select(x => x.gameObject)
            .OrderBy(x => x.name)
            .ToArray();
        
        for (int i = 0; i < activeShipPrefabWeapons.Length; i++)
        {
            _weapons[i].transform.localPosition = activeShipPrefabWeapons[i].transform.localPosition;
            _weapons[i].SetActive(activeShipPrefabWeapons[i].activeSelf);
        }
    }

    public bool HasKineticWeapons()
    {
        foreach (GameObject weapon in _weapons)
        {
            if (weapon.activeSelf && weapon.GetComponent<WeaponController>().IsKinetic())
            {
                return true;
            }
        }

        return false;
    }

    public bool HasEnergeticWeapons()
    {
        foreach (GameObject weapon in _weapons)
        {
            if (weapon.activeSelf && weapon.GetComponent<WeaponController>().IsEnergetic())
            {
                return true;
            }
        }

        return false;
    }
}
