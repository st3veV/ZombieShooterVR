﻿using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class WeaponSpawner : MonoBehaviour {

    public ZombieSpawner ZombieSpawner;
    public GameObject TargetPrefab;
    public Transform PlayerLocation;
    public Inventory Inventory;

    public event Action<GameObject> OnWeaponTargetSpawned;

    private Pool<GameObject> targetPool;

    public WeaponDatabase WeaponDatabase;
    public WeaponManager weaponManager;

    public ForceSpawn ForceSpawn;

	void Start ()
	{

	    WeaponDatabase = WeaponDatabase.Instance;
        ZombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;

        weaponManager = new WeaponManager();
        //init - selecting first weapon
        Debug.Log("pick weapon");
	    IWeapon weapon = weaponManager.GetWeapon(WeaponDatabase.Weapons[0]);
        Debug.Log("selecting weapon: " + weapon.Name);
	    Inventory.PickWeapon(weapon);

        //adding enough bullets
	    var modularAmmo = new ModularAmmo();
	    modularAmmo.SetValues(weapon.BulletType, weapon.MagazineSize + 100);
	    Inventory.PickAmmo(modularAmmo);

        targetPool = new Pool<GameObject>();
	}

    private void ZombieSpawner_OnZombieSpawned(GameObject zombie)
    {
        LifetimeComponent zombieLife = zombie.GetComponent<LifetimeComponent>();
        zombieLife.OnDie += zombieLife_OnDie;
    }

    private void zombieLife_OnDie(LifetimeComponent obj)
    {
        obj.OnDie -= zombieLife_OnDie;
        SpawnAmmo(obj.transform);
    }

    public void SpawnAmmo(Transform sourceTransform)
    {
        GameObject target = targetPool.Get();
        if (target == null)
        {
            target = Instantiate(TargetPrefab);
        }

        Vector3 targetPosition = sourceTransform.position;
        targetPosition.y = 0;
        target.transform.position = targetPosition;
        target.transform.LookAt(PlayerLocation);

        IPickable pickable;
        if (ForceSpawn != null)
        {
            pickable = ForceSpawn;
            ForceSpawn = null;
        }
        else
        {
            //select the weapon
            int index = (int) (WeaponDatabase.Weapons.Count*Random.value);

            IWeapon newWeapon = weaponManager.GetWeapon(WeaponDatabase.Weapons[index]);

            pickable = new Pickable();
            if (Random.value > .7)
            {
                //spawn weapon
                pickable.SetItem(newWeapon);
            }
            //spawn ammo
            ModularAmmo ammo = new ModularAmmo();
            ammo.SetValues(newWeapon.BulletType, (int) (Random.value*2 + 1)*newWeapon.MagazineSize);
            pickable.SetItem(ammo);
        }

        PickupHolder pickupHolder = target.GetComponent<PickupHolder>();
        pickupHolder.Pickable = pickable;

        LifetimeComponent targetLife = target.GetComponent<LifetimeComponent>();
        targetLife.OnDie += targetLife_OnDie;

        target.SetActive(true);
        spawned(target);
    }

    private void targetLife_OnDie(LifetimeComponent obj)
    {
        PickAmmo(obj.gameObject);
    }

    public void PickAmmo(GameObject target)
    {
        Debug.Log("Weapon target picked");
        PickupHolder pickupHolder = target.GetComponent<PickupHolder>();
        if (pickupHolder.Pickable != null)
        {
            if (pickupHolder.Pickable.Weapon != null)
            {
                Inventory.PickWeapon(pickupHolder.Pickable.Weapon);
            }
            if (pickupHolder.Pickable.Ammo != null)
            {
                Inventory.PickAmmo(pickupHolder.Pickable.Ammo);
            }
        }
        pickupHolder.Clear();
        target.SetActive(false);

        targetPool.Add(target);
    }
	
    private void spawned(GameObject target)
    {
        var handler = OnWeaponTargetSpawned;
        if (handler != null)
            handler(target);
    }

	void Update () {
	
	}

    public void Reset()
    {
        targetPool.Clear();
    }
}

public class WeaponManager
{
    private Dictionary<Weapon, IWeapon> weapons;

    public WeaponManager()
    {
        weapons = new Dictionary<Weapon, IWeapon>();
    }


    public IWeapon GetWeapon(Weapon realWeapon)
    {
        if (!weapons.ContainsKey(realWeapon))
        {
            weapons.Add(realWeapon, new PlayerWeapon(realWeapon));
        }
        return weapons[realWeapon];
    }
}

class ModularAmmo : IAmmo
{
    private int _type;

    public void SetValues(int type, int amount)
    {
        _type = type;
        Amount = amount;
    }

    public int Type
    {
        get { return _type; }
    }

    public int Amount { get; set; }
}

public class ForceSpawn:IPickable
{
    private IAmmo _ammo;
    private IWeapon _weapon;

    public IAmmo Ammo
    {
        get { return _ammo; }
    }

    public IWeapon Weapon
    {
        get { return _weapon; }
    }

    public void SetItem(IAmmo ammo)
    {
        _ammo = ammo;
    }

    public void SetItem(IWeapon weapon)
    {
        _weapon = weapon;
    }
}