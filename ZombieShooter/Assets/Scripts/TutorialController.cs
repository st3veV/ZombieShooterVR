using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    public GameObject TutorialTarget;
    public MyoHandler MyoHandler;
    public Gun UserGun;
    public ZombieSpawner ZombieSpawner;
    public WeaponSpawner WeaponSpawner;

    public List<GameObject> TutorialInstructions;
    public GameObject TutorialInstructionReoad;


    private LifetimeComponent _targetLifetime;
    private bool _checkingAmmo;

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
        _checkingAmmo = false;
        MyoHandler.OnMyoReset += MyoHandler_OnMyoReset;
        TutorialInstructions[0].SetActive(true);
    }

    private void TutorialStep2()
    {
        //Shoot target
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
        HideTutorialInstructions();
        TutorialInstructions[2].SetActive(true);
        TutorialInstructions[3].SetActive(true);
        ZombieSpawner.IsSpawning = true;
        ZombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;
    }

    private void TutorialStep4()
    {
        HideTutorialInstructions();
        TutorialInstructions[4].SetActive(true);
        WeaponSpawner.OnWeaponTargetSpawned += WeaponSpawner_OnWeaponTargetSpawned;
    }

    void WeaponSpawner_OnWeaponTargetSpawned(GameObject obj)
    {
        WeaponSpawner.OnWeaponTargetSpawned -= WeaponSpawner_OnWeaponTargetSpawned;
        LifetimeComponent ammoTargetLifetime = obj.GetComponent<LifetimeComponent>();
        ammoTargetLifetime.OnDie += ammoTargetLifetime_OnDie;
    }

    void ammoTargetLifetime_OnDie(LifetimeComponent obj)
    {
        obj.OnDie -= ammoTargetLifetime_OnDie;
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
        _checkingAmmo = false;
        TutorialStep2();
    }

    void _targetLifetime_OnDie(LifetimeComponent obj)
    {
        _targetLifetime.OnDie -= _targetLifetime_OnDie;
        TutorialStep3();
    }


    void Update () {
	    if (_checkingAmmo)
	    {
	        TutorialInstructionReoad.SetActive(UserGun.ShellsInMagazine == 0);
	    }
	}
}
