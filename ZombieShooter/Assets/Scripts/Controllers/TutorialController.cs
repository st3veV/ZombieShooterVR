using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class TutorialController : MonoBehaviour
    {

        public GameObject TutorialTarget;
        
        public GameObject ZombieSpawnPoint;

        public List<GameObject> TutorialInstructions;
        public GameObject TutorialInstructionReoad;

        public event Action OnTutorialComplete;

        private ZombieSpawner _zombieSpawner;
        private WeaponSpawner _weaponSpawner;

        private MyoHandler _myoHandler;
        private Gun _playerGun;
        private Transform _playerTransform;
        private LifetimeComponent _targetLifetime;
        private bool _checkingAmmo;
        private int _oldShellsInMagazine;
        private float _reloadDistance;

        private void Start()
        {
            Controller.Instance.SetTutorialController(this);
            AssignReferences();
        }

        private void AssignReferences()
        {
            PlayerController playerController = PlayerController.Instance;
            _myoHandler = playerController.MyoHandler;
            _playerGun = playerController.Gun;
            _playerTransform = _playerGun.gameObject.transform;

            _zombieSpawner = ZombieSpawner.Create();
            _weaponSpawner = WeaponSpawner.Create();
            _weaponSpawner.ZombieSpawner = _zombieSpawner;

            ZombieSpawnPoint = GameObject.Find("ZombieSpawnPoint");
            
            Initialize();
        }

        public void Initialize()
        {
            TutorialTarget.SetActive(false);
            _targetLifetime = TutorialTarget.GetComponent<LifetimeComponent>();
            HideTutorialInstructions();
            TutorialStep1();
            TutorialInstructionReoad.SetActive(false);
            _playerGun.FiringEnabled = false;
            _playerGun.SetFlashlightEnabled(false);

            ForceSpawn forceSpawn = new ForceSpawn();
            IWeapon weapon = WeaponManager.Instance.GetWeapon(WeaponDatabase.Instance.Weapons[3]);

            //forceSpawn.SetItem(ammo);
            forceSpawn.SetWeapon(weapon);
            forceSpawn.ContainsWeapon = true;
            forceSpawn.ContainsAmmo = false;
            //*
            ModularAmmo ammo = new ModularAmmo();
            ammo.SetValues(weapon.BulletType, 50);
            forceSpawn.SetAmmo(ammo);
            forceSpawn.ContainsAmmo = true;
            //*/

            _weaponSpawner.ForceSpawn = forceSpawn;

            _oldShellsInMagazine = _playerGun.ShellsInMagazine;

            _reloadDistance = Vector3.Distance(_playerTransform.position, TutorialInstructionReoad.transform.position);

            //Vector3 oldPos = ZombieSpawnPoint.transform.position;
            ZombieSpawnPoint.transform.position = new Vector3(0, 1, -30);
        }

        private void HideTutorialInstructions()
        {
            foreach (GameObject o in TutorialInstructions)
            {
                o.SetActive(false);
            }
        }

        private void TutorialStep1()
        {
            //Reset hand position
            Debug.Log("Tutorial step - reset hand position");
            _checkingAmmo = false;
            _myoHandler.OnMyoReset += MyoHandler_OnMyoReset;
            TutorialInstructions[0].SetActive(true);
        }

        private void TutorialStep2()
        {
            //Shoot target
            Debug.Log("Tutorial step - shoot target");
            HideTutorialInstructions();
            TutorialInstructions[1].SetActive(true);
            TutorialTarget.SetActive(true);
            _checkingAmmo = true;
            _playerGun.FiringEnabled = true;
            _targetLifetime.OnDie += _targetLifetime_OnDie;
        }

        private void TutorialStep3()
        {
            //Kill zombie
            Debug.Log("Tutorial step - kill zombie");
            HideTutorialInstructions();
            TutorialInstructions[2].SetActive(true);
            TutorialInstructions[3].SetActive(true);
            _zombieSpawner.IsSpawning = true;
            _zombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;
            _weaponSpawner.OnWeaponTargetSpawned += WeaponSpawner_OnWeaponTargetSpawned;
        }

        private void TutorialStep4()
        {
            //Pick gun
            Debug.Log("Tutorial step - pick gun");
            HideTutorialInstructions();
            TutorialInstructions[4].SetActive(true);
        }

        private void TutorialStep5()
        {
            //Change gun
            Debug.Log("Tutorial step - change gun");
            HideTutorialInstructions();
            TutorialInstructions[5].SetActive(true);
            _playerGun.OnWeaponChange += PlayerGunOnWeaponChange;
        }

        private void EndTutorial()
        {
            //Shoot to start the game
            Debug.Log("Tutorial step - End tutorial");
            HideTutorialInstructions();
            TutorialInstructions[TutorialInstructions.Count - 1].SetActive(true); // last instruction
            _targetLifetime.Reset();
            _targetLifetime.OnDie += _targetLifetime_OnDie2;
            TutorialTarget.SetActive(true);
        }

        private void _targetLifetime_OnDie2(LifetimeComponent obj)
        {
            _targetLifetime.OnDie -= _targetLifetime_OnDie;
            TutorialTarget.SetActive(false);
            //switch scenes
            Debug.Log("let's enter the game");
            OnOnTutorialComplete();
        }

        private void PlayerGunOnWeaponChange(IWeapon obj)
        {
            _playerGun.OnWeaponChange -= PlayerGunOnWeaponChange;
            EndTutorial();
        }

        private void WeaponSpawner_OnWeaponTargetSpawned(GameObject obj)
        {
            _weaponSpawner.OnWeaponTargetSpawned -= WeaponSpawner_OnWeaponTargetSpawned;
            LifetimeComponent ammoTargetLifetime = obj.GetComponent<LifetimeComponent>();
            ammoTargetLifetime.OnDie += ammoTargetLifetime_OnDie;
        }

        private void ammoTargetLifetime_OnDie(LifetimeComponent obj)
        {
            obj.OnDie -= ammoTargetLifetime_OnDie;
            obj.gameObject.SetActive(false);
            TutorialStep5();
        }

        private void ZombieSpawner_OnZombieSpawned(GameObject obj)
        {
            _zombieSpawner.OnZombieSpawned -= ZombieSpawner_OnZombieSpawned;
            _zombieSpawner.IsSpawning = false;
            LifetimeComponent zombieLifetime = obj.GetComponent<LifetimeComponent>();
            zombieLifetime.OnDie += zombieLifetime_OnDie;
        }

        private void zombieLifetime_OnDie(LifetimeComponent obj)
        {
            obj.OnDie -= zombieLifetime_OnDie;
            TutorialStep4();
        }

        private void MyoHandler_OnMyoReset()
        {
            _myoHandler.OnMyoReset -= MyoHandler_OnMyoReset;
            //_checkingAmmo = false;
            TutorialStep2();
        }

        private void _targetLifetime_OnDie(LifetimeComponent obj)
        {
            _targetLifetime.OnDie -= _targetLifetime_OnDie;
            TutorialTarget.SetActive(false);
            TutorialStep3();
        }


        private void Update()
        {
            if (_checkingAmmo)
            {
                if (_playerGun.ShellsInMagazine > _oldShellsInMagazine)
                {
                    _checkingAmmo = false;
                }
                _oldShellsInMagazine = _playerGun.ShellsInMagazine;
                if (_playerGun.ShellsInMagazine == 0)
                {
                    TutorialInstructionReoad.SetActive(true);
                    TutorialInstructionReoad.transform.position = transform.position +
                                                                  _reloadDistance*_playerTransform.forward;
                    TutorialInstructionReoad.transform.LookAt(_playerTransform);
                    Vector3 eulerAngles = TutorialInstructionReoad.transform.localRotation.eulerAngles;
                    TutorialInstructionReoad.transform.localRotation = Quaternion.Euler(eulerAngles.x,
                        eulerAngles.y + 180, eulerAngles.z);
                }
                else
                {
                    TutorialInstructionReoad.SetActive(false);
                }
            }
        }

        #region Event invocators

        protected virtual void OnOnTutorialComplete()
        {
            var handler = OnTutorialComplete;
            if (handler != null) handler();
        }

        #endregion
    }
}