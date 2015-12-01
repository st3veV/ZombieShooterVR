using System;
using UnityEngine;

namespace Controllers
{
    public class GameController : MonoBehaviour
    {
        public event Action OnGameEnded;

        private ZombieSpawner _zombieSpawner;
        private WeaponSpawner _weaponSpawner;

        private Gun _playerGun;
        private LifetimeComponent _playerLifetime;

        private void Start()
        {
            Controller.Instance.SetGameController(this);
            AssignReferences();
        }

        private void AssignReferences()
        {
            _zombieSpawner = ZombieSpawner.Create();
            _weaponSpawner = WeaponSpawner.Create();
            _weaponSpawner.ZombieSpawner = _zombieSpawner;

            PlayerController playerController = PlayerController.Instance;
            _playerGun = playerController.Gun;
            _playerLifetime = playerController.Lifetime;

            Initialize();
        }

        public void Initialize()
        {
            _zombieSpawner.Reset();
            _zombieSpawner.IsSpawning = true;

            _playerGun.SetFlashlightEnabled(true);
            _playerGun.FiringEnabled = true;

            _weaponSpawner.Reset();
            _playerLifetime.OnDie += PlayerLifetime_OnDie;
        }

        private void PlayerLifetime_OnDie(LifetimeComponent obj)
        {
            _playerLifetime.OnDie -= PlayerLifetime_OnDie;
            _zombieSpawner.Reset();
            _zombieSpawner.IsSpawning = false;
            OnOnGameEnded();
        }

        #region Event invocators

        protected virtual void OnOnGameEnded()
        {
            var handler = OnGameEnded;
            if (handler != null) handler();
        }

        #endregion
    }
}