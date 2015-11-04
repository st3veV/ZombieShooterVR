using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Gun : MonoBehaviour {

    public event Action OnWeaponKick;
    public event Action OnWeaponFire;
    public event Action OnWeaponKlick;
    public event Action OnWeaponReload;
    public event Action<IWeapon> OnWeaponChange;

    public GameObject ParticleBurst;
    public GameObject BulletTrail;

    public bool FiringEnabled;

    private bool _isFiring = false;
    private InternalTimer _timer;
    private IWeapon _currentWeapon;
    private int _shellsInMagazine;

    private GameObject _weaponContainer;

    private readonly Color _hitGroundColor = new Color(0xDF, 0xD6, 0xB6, 0xFF);
    private readonly Color _hitEnemyColor = new Color(0x68, 0x00, 0x00, 0xFF);

    private const float MaxBulletVisualRange = 20f;

    private Random _random;
    // Use this for initialization
    void Awake ()
    {
        _timer = new InternalTimer();

        _weaponContainer = transform.FindChild("GunContainer").gameObject;
        
        _random = new Random();
    }

    // Update is called once per frame
    void Update () {
        if (_isFiring)
        {
            if (_timer.Update())
            {
                if (_shellsInMagazine > 0)
                {
                    Fire();
                    _shellsInMagazine--;
                }
                else
                {
                    Klick();
                }
                _timer.Set(_currentWeapon.CooldownDelay);
            }
        }
        if (_runningSystems.Count > 0)
        {
            List<ParticleSystem> toRemove = new List<ParticleSystem>();
            foreach (ParticleSystem system in _runningSystems)
            {
                if (system == null || !system.isPlaying)
                {
                    toRemove.Add(system);
                }
            }
            foreach (ParticleSystem system in toRemove)
            {
                _runningSystems.Remove(system);
                if (system != null)
                {
                    system.gameObject.SetActive(false);
                    _particlePool.Add(system.gameObject);
                }
            }
        }
    }

    private void Klick()
    {
        WeaponKlick();
    }

    public void Fire()
    {
        Kick();
        WeaponFire();

        for (int i = 0; i < _currentWeapon.NumBulletsPerShot; i++)
        {
            RaycastHit hit;
            Vector3 direction = transform.forward;
            if (_currentWeapon.BulletSpreadAngle > 0)
            {
                float spreadX = (float) _random.NextDouble()*(_currentWeapon.BulletSpreadAngle) -
                                (_currentWeapon.BulletSpreadAngle/2);
                float spreadY = (float)_random.NextDouble() * (_currentWeapon.BulletSpreadAngle) -
                                (_currentWeapon.BulletSpreadAngle / 2);
                //Debug.Log(string.Format("sx: {0}, sy: {1}", spreadX, spreadY));
                direction.x += spreadX;
                direction.y += spreadY;
            }

            if (Physics.Raycast(transform.position, direction, out hit))
            {
                Collider target = hit.collider;
                Vector3 location = hit.point;

                GameObject targetGo = target.gameObject;

                SpawnParticles(location, targetGo.tag != "Enemy" ? _hitGroundColor : _hitEnemyColor);
                SpawnBulletTrail(location);

                LifetimeComponent targetLifetime = targetGo.GetComponent<LifetimeComponent>();
                if (targetLifetime != null)
                {
                    targetLifetime.ReceiveDamage(_currentWeapon.Damage);
                }
            }
            else
            {
                Vector3 target = transform.position + direction * MaxBulletVisualRange;
                SpawnBulletTrail(target);
            }
        }
    }

    private readonly Pool<GameObject> _bulletTrailPool = new Pool<GameObject>();

    private void SpawnBulletTrail(Vector3 location)
    {
        GameObject trail = _bulletTrailPool.Get();
        if (trail == null)
        {
            trail = Instantiate(BulletTrail);
        }
        BulletTrail bulletTrail = trail.GetComponent<BulletTrail>();
        bulletTrail.SetTarget(transform.position, location);
        bulletTrail.OnDone = OnBulletTrailDone;
        trail.SetActive(true);
    }

    private void OnBulletTrailDone(GameObject trail)
    {
        trail.GetComponent<BulletTrail>().OnDone = null;
        trail.SetActive(false);
        _bulletTrailPool.Add(trail);
    }

    private readonly List<ParticleSystem> _runningSystems = new List<ParticleSystem>();
    private readonly Pool<GameObject> _particlePool = new Pool<GameObject>();
    private void SpawnParticles(Vector3 location, Color color)
    {
        GameObject burst = _particlePool.Get();
        if (!burst)
        {
            burst = Instantiate(ParticleBurst);
        }
        burst.transform.position = location;
        ParticleSystem system = burst.GetComponent<ParticleSystem>();
        system.startColor = color;
        _runningSystems.Add(system);
        burst.SetActive(true);
        system.Play();
    }

    public void StartShooting()
    {
        if (FiringEnabled && _isFiring == false)
        {
            _isFiring = true;
            _timer.Set(0);
        }
    }

    public void StopShooting()
    {
        if (FiringEnabled && _isFiring)
        {
            _isFiring = false;
            Kick();
        }
    }

    public void Reload()
    {
        if (_currentWeapon.AvailableAmmo <= 0) return;

        if (_currentWeapon.AvailableAmmo >= _currentWeapon.MagazineSize)
        {
            _currentWeapon.AvailableAmmo -= _currentWeapon.MagazineSize;
            _shellsInMagazine = _currentWeapon.MagazineSize;
        }
        else
        {
            _shellsInMagazine = _currentWeapon.AvailableAmmo;
            _currentWeapon.AvailableAmmo = 0;
        }
        WeaponReload();
    }

    protected virtual void Kick()
    {
        var handler = OnWeaponKick;
        if (handler != null) handler();
    }

    public void SetWeapon(IWeapon weapon)
    {
        _currentWeapon = weapon;
        LoadWeaponModel(weapon.WeaponModel);
        Reload();
        OnOnWeaponChange(_currentWeapon);
    }

    private void LoadWeaponModel(GameObject weaponModel)
    {
        //delete old gun
        if (_weaponContainer.transform.childCount > 0)
        {
            foreach (Transform childTransform in _weaponContainer.transform) Destroy(childTransform.gameObject);
        }

        //instantiate new gun
        GameObject newWeapon = (GameObject) Instantiate(weaponModel, _weaponContainer.transform.position, _weaponContainer.transform.rotation);
        
        newWeapon.transform.SetParent(_weaponContainer.transform);
        newWeapon.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public int ShellsInMagazine
    {
        get { return _shellsInMagazine; }
    }

    public void TriggerWeaponChange()
    {
        if (_currentWeapon != null)
        {
            OnOnWeaponChange(_currentWeapon);
        }
    }

    protected virtual void OnOnWeaponChange(IWeapon obj)
    {
        var handler = OnWeaponChange;
        if (handler != null) handler(obj);
    }

    public void Reset()
    {
    }

    protected virtual void WeaponKlick()
    {
        var handler = OnWeaponKlick;
        if (handler != null) handler();
    }

    protected virtual void WeaponReload()
    {
        var handler = OnWeaponReload;
        if (handler != null) handler();
    }

    protected virtual void WeaponFire()
    {
        var handler = OnWeaponFire;
        if (handler != null) handler();
    }
}
