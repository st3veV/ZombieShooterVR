using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = System.Random;

namespace Shooting
{
    class ShootingController : Singleton<ShootingController>
    {
        private GameObject _particleBurst;
        private GameObject _bulletTrail;

        private Gun _gun;
        private IWeapon _currentWeapon;

        private readonly List<ParticleSystem> _runningParticleSystems = new List<ParticleSystem>();
        private Pool<GameObject> _particlePool;
        private Pool<GameObject> _bulletTrailPool;

        private readonly Color _hitGroundColor = new Color(0xDF, 0xD6, 0xB6, 0x55);
        private readonly Color _hitEnemyColor = new Color(0x68, 0x00, 0x00, 0x55);

        private const float MaxBulletVisualRange = 20f;

        private Random _random;

        void Awake()
        {
            _particleBurst = Resources.Load("Prefabs/Shooting/DustParticles") as GameObject;
            _bulletTrail = Resources.Load("Prefabs/Shooting/BulletTrail") as GameObject;

            _particlePool = new Pool<GameObject>(CreateParticles);
            _bulletTrailPool = new Pool<GameObject>(CreateBulletTrail);
        }

        void Start()
        {
            _random = new Random();
        }

        public void SetGun(Gun gun)
        {
            _gun = gun;
            _gun.OnWeaponFireTo += _gun_OnWeaponFireTo;
            _gun.OnWeaponChange += _gun_OnWeaponChange;
        }

        private void _gun_OnWeaponChange(IWeapon obj)
        {
            _currentWeapon = obj;
        }

        void Update()
        {
            if (_runningParticleSystems.Count > 0)
            {
                List<ParticleSystem> toRemove = new List<ParticleSystem>();
                for (int i = 0; i < _runningParticleSystems.Count; i++)
                {
                    ParticleSystem system = _runningParticleSystems[i];
                    if (system == null || !system.isPlaying)
                    {
                        toRemove.Add(system);
                    }
                }
                for (int i = 0; i < toRemove.Count; i++)
                {
                    ParticleSystem system = toRemove[i];
                    _runningParticleSystems.Remove(system);
                    if (system != null)
                    {
                        system.gameObject.SetActive(false);
                        _particlePool.Add(system.gameObject);
                    }
                }
            }
        }

        private void _gun_OnWeaponFireTo(Vector3 direction)
        {
            for (int i = 0; i < _currentWeapon.NumBulletsPerShot; i++)
            {
                RaycastHit hit;
                if (_currentWeapon.BulletSpreadAngle > 0)
                {
                    float spreadX = (float)_random.NextDouble() * (_currentWeapon.BulletSpreadAngle) -
                                    (_currentWeapon.BulletSpreadAngle / 2);
                    float spreadY = (float)_random.NextDouble() * (_currentWeapon.BulletSpreadAngle) -
                                    (_currentWeapon.BulletSpreadAngle / 2);

                    direction.x += spreadX;
                    direction.y += spreadY;
                }

                if (Physics.Raycast(_gun.transform.position, direction, out hit))
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
                    Vector3 target = _gun.transform.position + direction * MaxBulletVisualRange;
                    SpawnBulletTrail(target);
                }
            }
        }

        private void SpawnBulletTrail(Vector3 location)
        {
            GameObject trail = _bulletTrailPool.Get();
            BulletTrail bulletTrail = trail.GetComponent<BulletTrail>();
            bulletTrail.SetTarget(_gun.transform.position, location);
            bulletTrail.OnDone = OnBulletTrailDone;
            trail.SetActive(true);
        }

        private GameObject CreateBulletTrail()
        {
            return Instantiate(_bulletTrail);
        }

        private GameObject CreateParticles()
        {
            return Instantiate(_particleBurst);
        }

        private void OnBulletTrailDone(GameObject trail)
        {
            trail.GetComponent<BulletTrail>().OnDone = null;
            trail.SetActive(false);
            _bulletTrailPool.Add(trail);
        }

        private void SpawnParticles(Vector3 location, Color color)
        {
            GameObject burst = _particlePool.Get();
            burst.transform.position = location;
            ParticleSystem system = burst.GetComponent<ParticleSystem>();
            system.startColor = color;
            _runningParticleSystems.Add(system);
            burst.SetActive(true);
            system.Play();
        }

    }
}