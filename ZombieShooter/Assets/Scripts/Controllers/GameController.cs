using System;
using UnityEngine;

namespace Controllers
{
    public class GameController : MonoBehaviour
    {

        public ZombieSpawner ZombieSpawner;
        public WeaponSpawner WeaponSpawner;

        public event Action OnGameEnded;

        private Gun _playerGun;
        private LifetimeComponent _playerLifetime;

        private void Start()
        {
            AssignReferences();
        }

        private void AssignReferences()
        {
            ZombieSpawner = GameObject.Find("ZombieSpawner").GetComponent<ZombieSpawner>();
            WeaponSpawner = GameObject.Find("WeaponSpawner").GetComponent<WeaponSpawner>();

            PlayerController playerController = PlayerController.Instance;
            _playerGun = playerController.Gun;
            _playerLifetime = playerController.Lifetime;

            Initialize();
        }

        public void Initialize()
        {
            ZombieSpawner.Reset();
            ZombieSpawner.IsSpawning = true;

            _playerGun.SetFlashlightEnabled(true);
            _playerGun.FiringEnabled = true;

            WeaponSpawner.Reset();
            _playerLifetime.OnDie += PlayerLifetime_OnDie;
        }

        private void PlayerLifetime_OnDie(LifetimeComponent obj)
        {
            _playerLifetime.OnDie -= PlayerLifetime_OnDie;
            ZombieSpawner.Reset();
            ZombieSpawner.IsSpawning = false;
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