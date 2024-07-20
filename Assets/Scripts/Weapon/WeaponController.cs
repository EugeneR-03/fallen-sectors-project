using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class WeaponController : MonoBehaviour
{
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;
    private SpriteRenderer _spriteRenderer;
    private Transform _transform;
    private GameObject _player;
    private PlayerController _playerController;
    private GameObject _projectile;

    private ProjectileSystem _projectileSystem;
    private ScanSystem _scanSystem;

    private WeaponVariant _weaponVariant = WeaponVariant.MachineGun;
    
    [SerializeField] private GameObject _machineGunPrefab;
    [SerializeField] private GameObject _heavyMachineGunPrefab;
    [SerializeField] private GameObject _largeCaliberGunPrefab;
    [SerializeField] private GameObject _railGunPrefab;
    [SerializeField] private GameObject _rocketCannonPrefab;
    [SerializeField] private GameObject _laserCannonPrefab;
    [SerializeField] private GameObject _plasmaCannonPrefab;

    [SerializeField] private GameObject _kineticProjectile1;
    [SerializeField] private GameObject _kineticProjectile2;
    [SerializeField] private GameObject _kineticProjectile3;
    [SerializeField] private GameObject _kineticProjectile4;
    [SerializeField] private GameObject _energeticProjectile1;
    [SerializeField] private GameObject _energeticProjectile2;
    [SerializeField] private GameObject _energeticProjectile3;

    public Weapon WeaponStats { get; private set; }
    public Projectile ProjectileStats { get; set; }
    public bool IsAutoAttacking { get; private set; } = false;
    public float FireRate
    {
        get
        {
            return WeaponStats.FireRate / _playerController.GetWeaponsEnergyUnitsMultiplier();
        }
    }

    private void Init()
    {        
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _transform = GetComponent<Transform>();

        if (WeaponStats == null)
        {
            WeaponStats = new Weapon(_weaponVariant);
        }
        
        _projectile = GetProjectile(_weaponVariant);
        ProjectileController projectileController = _projectile.GetComponent<ProjectileController>();
        Debug.Log("ProjectileVariant: " + projectileController.GetProjectileVariant());
        if (ProjectileStats == null)
        {
            ProjectileStats = new Projectile(projectileController.GetProjectileVariant());
        }

        Debug.Log("FireSystems: " + _projectileSystem + " " + _scanSystem);
        SpecificInit(_weaponVariant);
        
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
    }

    private void SpecificInit(WeaponVariant weaponVariant)
    {
        int enemyLayerMask = 
            1 << LayerMask.NameToLayer("EnemyOrdinary") |
            1 << LayerMask.NameToLayer("EnemyFast") |
            1 << LayerMask.NameToLayer("EnemyArmored");

        switch (weaponVariant)
        {
            case WeaponVariant.MachineGun:
                _projectileSystem = new ProjectileSystem(gameObject, _projectile);
                break;

            case WeaponVariant.HeavyMachineGun:
                _projectileSystem = new ProjectileSystem(gameObject, _projectile);
                break;

            case WeaponVariant.LargeCaliberGun:
                _projectileSystem = new ProjectileSystem(gameObject, _projectile);
                break;

            case WeaponVariant.RailGun:
                _scanSystem = new MultiHitScanSystem(gameObject, _projectile, enemyLayerMask, 0.2f);
                break;

            case WeaponVariant.RocketCannon:
                _projectileSystem = new ProjectileSystem(gameObject, _projectile);
                break;

            case WeaponVariant.LaserCannon:
                _scanSystem = new RayScanSystem(gameObject, _projectile, enemyLayerMask, 0.2f);
                break;

            case WeaponVariant.PlasmaCannon:
                _projectileSystem = new ProjectileSystem(gameObject, _projectile);
                break;

            default:
                break;
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        _projectileSystem?.Update();
        _scanSystem?.Update();

        if (Input.GetButtonDown("Fire2"))
        {
            IsAutoAttacking = !IsAutoAttacking;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Fire1") || IsAutoAttacking)
        {
            Shoot();
        }
        else
        {
            Idle();
        }
    }

    private void SpecificShoot(WeaponVariant weaponVariant)
    {
        switch (weaponVariant)
        {
            case WeaponVariant.MachineGun:
                _projectileSystem.Shoot(FireRate);
                break;
            
            case WeaponVariant.HeavyMachineGun:
                _projectileSystem.Shoot(FireRate);
                break;

            case WeaponVariant.LargeCaliberGun:
                _projectileSystem.Shoot(FireRate);
                break;

            case WeaponVariant.RailGun:
                _scanSystem.ShootWithoutRaycast(FireRate, 50f);
                break;

            case WeaponVariant.RocketCannon:
                _projectileSystem.Shoot(FireRate);
                break;

            case WeaponVariant.LaserCannon:
                _scanSystem.Shoot(FireRate, 50f);
                break;

            case WeaponVariant.PlasmaCannon:
                _projectileSystem.Shoot(FireRate);
                break;

            default:
                break;
        }
    }

    public void Shoot()
    {
        SpecificShoot(_weaponVariant);    
    }

    public void Idle()
    {
        _projectileSystem?.Idle();
        _scanSystem?.Idle();
    }

    public GameObject GetProjectile(WeaponVariant weaponVariant)
    {
        return weaponVariant switch
        {
            WeaponVariant.MachineGun => _kineticProjectile1,
            WeaponVariant.HeavyMachineGun => _kineticProjectile2,
            WeaponVariant.LargeCaliberGun => _kineticProjectile3,
            WeaponVariant.RailGun => _kineticProjectile4,
            WeaponVariant.RocketCannon => _energeticProjectile1,
            WeaponVariant.LaserCannon => _energeticProjectile2,
            WeaponVariant.PlasmaCannon => _energeticProjectile3,
            _ => null,
        };
    }

    private void SetWeapon(GameObject weapon)
    {
        Init();

        _animator.runtimeAnimatorController = weapon.GetComponent<Animator>().runtimeAnimatorController;
        _boxCollider2D.size = weapon.GetComponent<BoxCollider2D>().size;
        _boxCollider2D.offset = weapon.GetComponent<BoxCollider2D>().offset;
        _spriteRenderer.sprite = weapon.GetComponent<SpriteRenderer>().sprite;
        _transform.localScale = weapon.GetComponent<Transform>().localScale;
    }

    public void SetWeaponVariant(WeaponVariant weaponVariant)
    {
        _weaponVariant = weaponVariant;
        WeaponStats = new Weapon(_weaponVariant);
        _projectile = GetProjectile(_weaponVariant);
        Debug.Log("ProjectileVariant: " + _projectile.GetComponent<ProjectileController>());
        ProjectileStats = new Projectile(_projectile.GetComponent<ProjectileController>().GetProjectileVariant());

        // заменяем всё в текущем объекте на компоненты префабов
        switch (_weaponVariant)
        {
            case WeaponVariant.None:
                gameObject.SetActive(false);
                break;
            case WeaponVariant.MachineGun:
                SetWeapon(_machineGunPrefab);
                break;
            case WeaponVariant.HeavyMachineGun:
                SetWeapon(_heavyMachineGunPrefab);
                break;
            case WeaponVariant.LargeCaliberGun:
                SetWeapon(_largeCaliberGunPrefab);
                break;
            case WeaponVariant.RailGun:
                SetWeapon(_railGunPrefab);
                break;
            case WeaponVariant.RocketCannon:
                SetWeapon(_rocketCannonPrefab);
                break;
            case WeaponVariant.LaserCannon:
                SetWeapon(_laserCannonPrefab);
                break;
            case WeaponVariant.PlasmaCannon:
                SetWeapon(_plasmaCannonPrefab);
                break;
        }
    }

    public bool IsKinetic()
    {
        return _projectile.GetComponent<ProjectileController>().IsKinetic();
    }

    public bool IsEnergetic()
    {
        return _projectile.GetComponent<ProjectileController>().IsEnergetic();
    }


    public class ProjectileSystem
    {
        private readonly GameObject _parent;
        private readonly GameObject _projectile;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private float _initialAnimationSpeed;
        private string _animatorBoolName;
        private float _timer;
        private bool _canFire;
        private float _fireRate;
        
        public ProjectileSystem(GameObject parent, GameObject projectilePrefab, float initialAnimationSpeed = 0.5f, string animatorBoolName = "IsAttacking")
        {
            _parent = parent;
            _projectile = projectilePrefab;
            _transform = _parent.transform;
            _animator = _parent.GetComponent<Animator>();
            _initialAnimationSpeed = initialAnimationSpeed; 
            _animatorBoolName = animatorBoolName;

            _timer = 0.0f;
            _canFire = true;
            _fireRate = 0.5f;
        }

        private void UpdateShootingTimer()
        {
            if (_canFire)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer > _fireRate)
            {
                _canFire = true;
                _timer = 0.0f;
            }
        }

        public void Update()
        {
            UpdateShootingTimer();
        }

        public GameObject Shoot(float fireRate)
        {
            if (!_canFire)
            {
                return null;
            }

            _fireRate = fireRate;
            _canFire = false;
            Debug.Log("Shooting. FireRate: " + _fireRate + " Timer: " + _timer + " CanFire: " + _canFire);
            _animator.speed = _initialAnimationSpeed / _fireRate;
            _animator.SetBool("IsAttacking", true);

            Vector3 position = _transform.position;
            position.z = 0.0f;
            GameObject projectileClone = GameObject.Instantiate(_projectile, position, _transform.rotation);
            projectileClone.SetActive(true);

            return projectileClone;
        }

        public void Idle()
        {
            _animator.SetBool("IsAttacking", false);
        }

        
    }

    public abstract class ScanSystem
    {
        protected readonly GameObject _parent;
        protected readonly GameObject _projectile;
        protected readonly int _layerMask;
        protected readonly Transform _transform;
        protected readonly Animator _animator;
        protected float _initialAnimationSpeed;
        protected string _animatorBoolName;
        protected bool _canFire;
        
        public ScanSystem(GameObject parent, GameObject projectilePrefab, int layerMask, 
                            float initialAnimationSpeed, string animatorBoolName = "IsAttacking")
        {
            _parent = parent;
            _projectile = Instantiate(projectilePrefab);
            _projectile.SetActive(false);
            _layerMask = layerMask;
            _transform = _parent.transform;
            _animator = _parent.GetComponent<Animator>();
            _initialAnimationSpeed = initialAnimationSpeed; 
            _animatorBoolName = animatorBoolName;

            _animator.speed = _initialAnimationSpeed;

            _canFire = true;
        }

        public virtual void Update() {}

        public virtual void Shoot(float fireRate, float maxDistance) {}
        public virtual void ShootWithoutRaycast(float fireRate, float maxDistance) {}
        public virtual void Idle() {}
    }

    public abstract class HitScanSystem : ScanSystem
    {
        private float _fireRate;
        private float _timer;

        public HitScanSystem(GameObject parent, GameObject projectilePrefab, int layerMask, float initialAnimationSpeed, string animatorBoolName = "IsAttacking")
            : base(parent, projectilePrefab, layerMask, initialAnimationSpeed, animatorBoolName)
        {
            _fireRate = 0.5f;
            _timer = 0.0f;
        }

        private void UpdateShootingTimer()
        {
            if (_canFire)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer > _fireRate)
            {
                _canFire = true;
                _timer = 0.0f;
            }
        }

        protected virtual async Task SetActiveProjectilePrefab(float time)
        {
            float tick = time / 10f;
            int tickMS = (int)(tick * 1000);

            SpriteRenderer spriteRenderer = _projectile.GetComponent<SpriteRenderer>();

            float colorR = spriteRenderer.color.r;
            float colorG = spriteRenderer.color.g;
            float colorB = spriteRenderer.color.b;
            float colorA = 1.0f;
            spriteRenderer.color = new Color(colorR, colorG, colorB, colorA);

            _projectile.SetActive(true);
            BoxCollider2D boxCollider2D = _projectile.GetComponent<BoxCollider2D>();
            boxCollider2D.enabled = true;
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(tickMS);
                boxCollider2D.enabled = false;
                colorA *= 0.5f;
                spriteRenderer.color = new Color(colorR, colorG, colorB, colorA);
                Debug.Log("Color: " + spriteRenderer.color);
            }
            _projectile.SetActive(false);
        }

        public override void Update()
        {
            UpdateShootingTimer();
        }

        public override void Shoot(float fireRate, float maxDistance)
        {
            if (!_canFire)
            {
                return;
            }

            _fireRate = fireRate;
            _canFire = false;
            _animator.speed = _initialAnimationSpeed / _fireRate;
            _animator.SetBool(_animatorBoolName, true);

            // доп. логика в дочерних классах
        }

        public override void ShootWithoutRaycast(float fireRate, float maxDistance)
        {
            Shoot(fireRate, maxDistance);
        }
    }

    public class SingleHitScanSystem : HitScanSystem
    {
        private RaycastHit2D _hit;
        public SingleHitScanSystem(GameObject parent, GameObject projectilePrefab, int layerMask,
                    float initialAnimationSpeed, string animatorBoolName = "IsAttacking")
            : base(parent, projectilePrefab, layerMask, initialAnimationSpeed, animatorBoolName)
        {
        }

        protected override async Task SetActiveProjectilePrefab(float time)
        {
            Vector2 spawnPosition = _transform.position;
            Vector2 spawnDirection = _hit.transform.position - _transform.position;
            float distance = spawnDirection.magnitude;
            spawnDirection.Normalize();
            spawnPosition += 0.5f * distance * spawnDirection;

            _projectile.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            
            Vector2 projectileSize = _projectile.GetComponent<BoxCollider2D>().size;

            Vector3 targetScale = _projectile.transform.localScale;
            float scaleY = distance / projectileSize.y;
            
            _projectile.transform.localScale = new Vector3(targetScale.x, scaleY, targetScale.z);

            await SetActiveProjectilePrefab(time);
        }

        public override async void Shoot(float fireRate, float maxDistance)
        {
            if (!_canFire)
            {
                return;
            }

            base.Shoot(fireRate, maxDistance);

            _hit = Physics2D.Raycast(_transform.position, _transform.up, maxDistance, _layerMask);
            Debug.Log("Raycast: " + _hit);

            if (_hit.collider != null)
            {
                Debug.Log("Hit: " + _hit.transform.name);
                float lifeTime = _projectile.GetComponent<ProjectileController>().ProjectileStats.LifeTime;
                await base.SetActiveProjectilePrefab(lifeTime);
            }
        }

        public override async void ShootWithoutRaycast(float fireRate, float maxDistance)
        {
            if (!_canFire)
            {
                return;
            }

            base.Shoot(fireRate, maxDistance);
            
            float lifeTime = _projectile.GetComponent<ProjectileController>().ProjectileStats.LifeTime;
            await base.SetActiveProjectilePrefab(lifeTime);
        }
    }

    public class MultiHitScanSystem : HitScanSystem
    {
        private RaycastHit2D[] _hits;
        public MultiHitScanSystem(GameObject parent, GameObject projectilePrefab, int layerMask,
                    float initialAnimationSpeed, string animatorBoolName = "IsAttacking")
            : base(parent, projectilePrefab, layerMask, initialAnimationSpeed, animatorBoolName)
        {
        }

        protected override async Task SetActiveProjectilePrefab(float time)
        {
            Vector2 spawnPosition = _transform.position;
            Vector2 spawnDirection = Quaternion.identity * _transform.up;
            float distance = 20f;
            spawnDirection.Normalize();
            spawnPosition += 0.5f * distance * spawnDirection;
            _projectile.transform.SetPositionAndRotation(spawnPosition, _transform.rotation);

            Vector2 projectileSize = _projectile.GetComponent<BoxCollider2D>().size;

            Vector3 targetScale = _projectile.transform.localScale;
            float scaleY = distance / projectileSize.y;
            
            _projectile.transform.localScale = new Vector3(targetScale.x, scaleY, targetScale.z);
            Debug.Log("Test message");

            await base.SetActiveProjectilePrefab(time);
        }

        public override async void Shoot(float fireRate, float maxDistance)
        {
            if (!_canFire)
            {
                return;
            }

            base.Shoot(fireRate, maxDistance);

            _hits = Physics2D.RaycastAll(_transform.position, _transform.up, maxDistance, _layerMask);
            Debug.Log("Raycast: " + _hits);

            if (_hits.Length > 0)
            {
                Debug.Log("Hit: " + _hits[0].transform.name);
                float lifeTime = _projectile.GetComponent<ProjectileController>().ProjectileStats.LifeTime;

                await SetActiveProjectilePrefab(lifeTime);
            }
        }

        public override async void ShootWithoutRaycast(float fireRate, float maxDistance)
        {
            if (!_canFire)
            {
                return;
            }

            base.Shoot(fireRate, maxDistance);
            float lifeTime = _projectile.GetComponent<ProjectileController>().ProjectileStats.LifeTime;

            await SetActiveProjectilePrefab(lifeTime);
        }
    }

    public class RayScanSystem : ScanSystem
    {
        private RaycastHit2D _hit;
        public RayScanSystem(GameObject parent, GameObject projectilePrefab, int layerMask, 
                float initialAnimationSpeed, string animatorBoolName = "IsAttacking")
            : base(parent, projectilePrefab, layerMask, initialAnimationSpeed, animatorBoolName)
        {
        }

        private void UpdateRayDependingOnHit()
        {
            if (_hit.transform == null || !_canFire)
            {
                return;
            }

            Vector2 spawnPosition = _transform.position;
            Vector2 spawnDirection = _hit.transform.position - _transform.position;
            float distance = spawnDirection.magnitude;
            spawnDirection.Normalize();
            spawnPosition += 0.5f * distance * spawnDirection;

            _projectile.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

            Vector2 projectileSize = _projectile.GetComponent<BoxCollider2D>().size;

            Vector3 targetScale = _projectile.transform.localScale;
            float scaleY = distance / projectileSize.y;
            
            _projectile.transform.localScale = new Vector3(targetScale.x, scaleY, targetScale.z);

            float angle = Vector2.SignedAngle(Vector2.up, spawnDirection);
            Vector3 targetRotation = new(0, 0, angle);
            _projectile.transform.rotation = Quaternion.Euler(targetRotation);
        }

        public override void Update()
        {
            base.Update();

            UpdateRayDependingOnHit();
        }

        public override void Idle()
        {
            base.Idle();

            _projectile.SetActive(false);
        }

        public override void Shoot(float fireRate, float maxDistance)
        {
            base.Shoot(fireRate, maxDistance);

            _hit = Physics2D.Raycast(_transform.position, _transform.up, maxDistance, _layerMask);
            Debug.Log("Raycast: " + _hit);
    
            if (_hit.collider != null)
            {
                Debug.Log("Hit: " + _hit.transform.name);
                _animator.SetBool(_animatorBoolName, true);
                _projectile.SetActive(true);
            }
            else
            {
                _animator.SetBool(_animatorBoolName, false);
                _projectile.SetActive(false);
            }
        }
    }
}