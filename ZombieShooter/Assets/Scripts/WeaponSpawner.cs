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

    private Pool<GameObject> targetPool;

    public WeaponDatabase WeaponDatabase;
    public WeaponManager WeaponManager;

    public ForceSpawn ForceSpawn;

	void Start ()
	{

	    WeaponDatabase = WeaponDatabase.Instance;
        ZombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;

        WeaponManager = WeaponManager.Instance;
        
        Inventory.Reset();

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
        LifetimeComponent targetLife = null;
        if (target == null)
        {
            target = Instantiate(TargetPrefab);
            targetLife = target.GetComponent<LifetimeComponent>();
            targetLife.LifetimeDamage = BalancingData.WEAPON_TARGET_HEALTH;
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

            IWeapon newWeapon = WeaponManager.GetWeapon(WeaponDatabase.Weapons[index]);

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
        pickupHolder.Activate();

        if (targetLife == null)
        {
            targetLife = target.GetComponent<LifetimeComponent>();
            targetLife.Reset();
        }

        targetLife.OnDie += targetLife_OnDie;

        target.SetActive(true);
        spawned(target);
    }

    private void targetLife_OnDie(LifetimeComponent obj)
    {
        obj.OnDie -= targetLife_OnDie;
        PickAmmo(obj.gameObject);

        obj.gameObject.SetActive(false);
        targetPool.Add(obj.gameObject);
    }

    public void PickAmmo(GameObject target)
    {
        //Debug.Log("Weapon target picked");
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
    private static WeaponManager _instance;
    public static WeaponManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new WeaponManager();
            }
            return _instance;
        }
    }

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