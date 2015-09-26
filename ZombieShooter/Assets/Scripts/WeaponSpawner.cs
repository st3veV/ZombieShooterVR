using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class WeaponSpawner : MonoBehaviour {

    public ZombieSpawner ZombieSpawner;
    public GameObject TargetPrefab;
    public Transform PlayerLocation;
    public Inventory Inventory;

    public event Action<GameObject> OnWeaponTargetSpawned;

    private List<GameObject> targetPool;

    public WeaponDatabase WeaponDatabase;

	void Start ()
	{

	    WeaponDatabase = WeaponDatabase.Instance;
        ZombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;

        targetPool = new List<GameObject>();
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
        GameObject target;
        if(targetPool.Count > 0)
        {
            target = targetPool[0];
            targetPool.RemoveAt(0);
        }
        else
        {
            target = Instantiate(TargetPrefab);
        }
        Vector3 targetPosition = sourceTransform.position;
        targetPosition.y = 0;
        target.transform.position = targetPosition;
        target.transform.LookAt(PlayerLocation);
        
        //select the weapon
        int index = (int)(WeaponDatabase.Weapons.Count * Random.value);
        IWeapon newWeapon = WeaponDatabase.Weapons[index];

        IPickable pickable = new Pickable();
        if (Random.value > .7)
        {
            //spawn weapon
            pickable.SetItem(newWeapon);
        }
        //spawn ammo
        ModularAmmo ammo = new ModularAmmo();
        ammo.SetValues(newWeapon.BulletType, (int) (Random.value*20) + 20);
        pickable.SetItem(ammo);

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
        Debug.Log("Ammo picked");
        PickupHolder pickupHolder = target.GetComponent<PickupHolder>();
        if (pickupHolder.Pickable.Weapon != null)
        {
            Inventory.PickWeapon(pickupHolder.Pickable.Weapon);
        }
        if (pickupHolder.Pickable.Ammo != null)
        {
            Inventory.PickAmmo(pickupHolder.Pickable.Ammo);
        }

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