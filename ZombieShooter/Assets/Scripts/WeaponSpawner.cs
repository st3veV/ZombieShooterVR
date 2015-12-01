using UnityEngine;
using System.Collections.Generic;
using System;
using Controllers;
using Radar;
using Utils;
using Random = UnityEngine.Random;

public class WeaponSpawner : AutoObject<WeaponSpawner>
{

    public ZombieSpawner ZombieSpawner;
    
    public event Action<GameObject> OnWeaponTargetSpawned;

    private GameObject _targetPrefab;
    private GameObjectPool _targetPool;

    private PlayerController _playerController;
    private WeaponDatabase _weaponDatabase;
    private WeaponManager _weaponManager;
    private Transform _playerLocation;

    public ForceSpawn ForceSpawn;
    
    void Start ()
	{
	    _playerController = PlayerController.Instance;
        _playerLocation = _playerController.PlayerTransform;

	    _weaponDatabase = WeaponDatabase.Instance;
        ZombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;

        _weaponManager = WeaponManager.Instance;
        
        _playerController.Inventory.Reset();


        _targetPrefab = Resources.Load("Prefabs/AmmoTarget") as GameObject;
        _targetPool = new GameObjectPool(_targetPrefab, gameObject);
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
        bool isNew;
        GameObject target = _targetPool.Get(out isNew);
        
        Vector3 targetPosition = sourceTransform.position;
        targetPosition.y = 0;
        target.transform.position = targetPosition;
        target.transform.LookAt(_playerLocation);

        IPickable pickable;
        if (ForceSpawn != null)
        {
            pickable = ForceSpawn;
            ForceSpawn = null;
        }
        else
        {
            //select the weapon
            int index = (int) (_weaponDatabase.Weapons.Count*Random.value);

            var newWeapon = _weaponManager.GetWeapon(_weaponDatabase.Weapons[index]);

            pickable = new Pickable();
            pickable.SetWeapon(newWeapon);
            pickable.ContainsWeapon = Random.value > .7;
            //spawn ammo
            var ammo = new ModularAmmo();
            ammo.SetValues(newWeapon.BulletType, ((int) (Random.value*2 + 1))*newWeapon.MagazineSize);
            pickable.SetAmmo(ammo);
            pickable.ContainsAmmo = true;

        }

        PickupHolder pickupHolder = target.GetComponent<PickupHolder>();
        pickupHolder.Pickable = pickable;
        pickupHolder.Activate();

        var radarTrackable = target.GetComponent<RadarTrackable>();
        RadarController.Instance.AddTrackedObject(radarTrackable);

        LifetimeComponent targetLife = target.GetComponent<LifetimeComponent>();
        if (isNew)
        {
            targetLife.LifetimeDamage = BalancingData.WEAPON_TARGET_HEALTH;
        }
        else
        {
            targetLife.Reset();
        }
        targetLife.OnDie += targetLife_OnDie;

        target.SetActive(true);
        spawned(target);
    }
    
    private void targetLife_OnDie(LifetimeComponent obj)
    {
        var radarTrackable = obj.GetComponent<RadarTrackable>();
        RadarController.Instance.RemoveTrackedObject(radarTrackable);

        obj.OnDie -= targetLife_OnDie;
        PickAmmo(obj.gameObject);

        obj.gameObject.SetActive(false);
        _targetPool.Add(obj.gameObject);
    }

    public void PickAmmo(GameObject target)
    {
        //Debug.Log("Weapon target picked");
        var pickupHolder = target.GetComponent<PickupHolder>();
        var pickable = pickupHolder.Pickable;
        if (pickable != null)
        {
            if (pickable.ContainsWeapon)
            {
                _playerController.Inventory.PickWeapon(pickable.Weapon);
            }
            if (pickable.ContainsAmmo)
            {
                _playerController.Inventory.PickAmmo(pickable.Ammo);
            }
        }
        pickupHolder.Clear();
    }
	
    public void Reset()
    {
        _targetPool.Clear();
    }

    #region Event invocators

    private void spawned(GameObject target)
    {
        var handler = OnWeaponTargetSpawned;
        if (handler != null)
            handler(target);
    }

    #endregion
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
    public void SetWeapon(IWeapon weapon)
    {
        _weapon = weapon;
    }

    public void SetAmmo(IAmmo ammo)
    {
        _ammo = ammo;
    }

    public bool ContainsWeapon { get; set; }

    public bool ContainsAmmo { get; set; }

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
    
}