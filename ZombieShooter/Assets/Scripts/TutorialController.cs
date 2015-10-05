using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public GameObject TutorialTarget;
    public MyoHandler MyoHandler;
    public Gun UserGun;
    public ZombieSpawner ZombieSpawner;
    public WeaponSpawner WeaponSpawner;
    public Transform PlayerTransform;

    public List<GameObject> TutorialInstructions;
    public GameObject TutorialInstructionReoad;


    private LifetimeComponent _targetLifetime;
    private bool _checkingAmmo;
    private int _oldShellsInMagazine;
    private float _reloadDistance;

    // Use this for initialization
	void Start () {
        TutorialTarget.SetActive(false);
	    _targetLifetime = TutorialTarget.GetComponent<LifetimeComponent>();
        HideTutorialInstructions();
	    TutorialStep1();
	    TutorialInstructionReoad.SetActive(false);
	    UserGun.FiringEnabled = false;

	    ForceSpawn forceSpawn = new ForceSpawn();
	    IWeapon weapon = WeaponSpawner.weaponManager.GetWeapon(WeaponDatabase.Instance.Weapons[1]);
        ModularAmmo ammo = new ModularAmmo();
        ammo.SetValues(weapon.BulletType, 100);
        forceSpawn.SetItem(ammo);
        forceSpawn.SetItem(weapon);
	    WeaponSpawner.ForceSpawn = forceSpawn;

	    _oldShellsInMagazine = UserGun.ShellsInMagazine;

	    _reloadDistance = Vector3.Distance(PlayerTransform.position, TutorialInstructionReoad.transform.position);
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
        MyoHandler.OnMyoReset += MyoHandler_OnMyoReset;
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
        UserGun.FiringEnabled = true;
        _targetLifetime.OnDie += _targetLifetime_OnDie;
    }

    private void TutorialStep3()
    {
        //Kill zombie
        Debug.Log("Tutorial step - kill zombie");
        HideTutorialInstructions();
        TutorialInstructions[2].SetActive(true);
        TutorialInstructions[3].SetActive(true);
        ZombieSpawner.IsSpawning = true;
        ZombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;
        WeaponSpawner.OnWeaponTargetSpawned += WeaponSpawner_OnWeaponTargetSpawned;
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
        UserGun.OnWeaponChange += UserGun_OnWeaponChange;
    }

    private void EndTutorial()
    {
        //Shoot to start the game
        HideTutorialInstructions();
        TutorialInstructions[TutorialInstructions.Count - 1].SetActive(true); // last instruction
        _targetLifetime.Reset();
        _targetLifetime.OnDie += _targetLifetime_OnDie2;
        TutorialTarget.SetActive(true);
    }

    void _targetLifetime_OnDie2(LifetimeComponent obj)
    {
        _targetLifetime.OnDie -= _targetLifetime_OnDie;
        TutorialTarget.SetActive(false);
        //switch scenes
    }

    void UserGun_OnWeaponChange(IWeapon obj)
    {
        UserGun.OnWeaponChange -= UserGun_OnWeaponChange;
        EndTutorial();
    }

    void WeaponSpawner_OnWeaponTargetSpawned(GameObject obj)
    {
        Debug.Log("Weapon spawned");
        WeaponSpawner.OnWeaponTargetSpawned -= WeaponSpawner_OnWeaponTargetSpawned;
        LifetimeComponent ammoTargetLifetime = obj.GetComponent<LifetimeComponent>();
        ammoTargetLifetime.OnDie += ammoTargetLifetime_OnDie;
    }

    void ammoTargetLifetime_OnDie(LifetimeComponent obj)
    {
        Debug.Log("Weapon picked");
        obj.OnDie -= ammoTargetLifetime_OnDie;
        obj.gameObject.SetActive(false);
        TutorialStep5();
    }

    void ZombieSpawner_OnZombieSpawned(GameObject obj)
    {
        ZombieSpawner.IsSpawning = false;
        LifetimeComponent zombieLifetime = obj.GetComponent<LifetimeComponent>();
        zombieLifetime.OnDie += zombieLifetime_OnDie;
    }

    void zombieLifetime_OnDie(LifetimeComponent obj)
    {
        obj.OnDie -= zombieLifetime_OnDie;
        TutorialStep4();
    }

    void MyoHandler_OnMyoReset()
    {
        MyoHandler.OnMyoReset -= MyoHandler_OnMyoReset;
        //_checkingAmmo = false;
        TutorialStep2();
    }

    void _targetLifetime_OnDie(LifetimeComponent obj)
    {
        _targetLifetime.OnDie -= _targetLifetime_OnDie;
        TutorialTarget.SetActive(false);
        TutorialStep3();
    }


    void Update () {
        if (_checkingAmmo)
        {
            if (UserGun.ShellsInMagazine > _oldShellsInMagazine)
            {
                _checkingAmmo = false;
            }
            _oldShellsInMagazine = UserGun.ShellsInMagazine;
            if (UserGun.ShellsInMagazine == 0)
            {
                TutorialInstructionReoad.SetActive(true);
                TutorialInstructionReoad.transform.position = transform.position + _reloadDistance * PlayerTransform.forward;
                TutorialInstructionReoad.transform.LookAt(PlayerTransform);
                Vector3 eulerAngles = TutorialInstructionReoad.transform.localRotation.eulerAngles;
                TutorialInstructionReoad.transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + 180, eulerAngles.z);
            }
            else
            {
                TutorialInstructionReoad.SetActive(false);
            }
	    }
	}
}
