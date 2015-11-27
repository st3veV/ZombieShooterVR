using System;
using UnityEngine;

namespace Controllers
{
    public class GameController : MonoBehaviour
    {

        public ZombieSpawner ZombieSpawner;
        public WeaponSpawner WeaponSpawner;
        public Gun UserGun;
        public LifetimeComponent UserLifetime;

        public event Action OnGameEnded;

        private void Start()
        {
            AssignReferences();
        }

        private void AssignReferences()
        {
            ZombieSpawner = GameObject.Find("ZombieSpawner").GetComponent<ZombieSpawner>();
            WeaponSpawner = GameObject.Find("WeaponSpawner").GetComponent<WeaponSpawner>();
            UserGun = GameObject.Find("Gun").GetComponent<Gun>();
            UserLifetime = GameObject.Find("UserData").GetComponent<LifetimeComponent>();

            Initialize();
        }

        public void Initialize()
        {
            ZombieSpawner.Reset();
            ZombieSpawner.IsSpawning = true;

            UserGun.SetFlashlightEnabled(true);
            UserGun.FiringEnabled = true;

            WeaponSpawner.Reset();
            UserLifetime.OnDie += UserLifetime_OnDie;
        }

        private void UserLifetime_OnDie(LifetimeComponent obj)
        {
            UserLifetime.OnDie -= UserLifetime_OnDie;
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