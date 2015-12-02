using UnityEngine;
using System.Collections.Generic;
using System;
using Controllers;
using Radar;
using Utils;
using Random = UnityEngine.Random;

public class WeaponSpawner : AutoObject<WeaponSpawner>
{
    public event Action<AmmoTarget> OnWeaponTargetSpawned;

    private GameObject _targetPrefab;
    private AutoObjectWrapperPool<AmmoTarget> _targetPool;

    private ZombieSpawner _zombieSpawner;
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
        _zombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;

        _weaponManager = WeaponManager.Instance;
        
        _playerController.Inventory.Reset();


        _targetPrefab = Resources.Load("Prefabs/AmmoTarget") as GameObject;
        _targetPool = new AutoObjectWrapperPool<AmmoTarget>(_targetPrefab, gameObject);
	}

    private void ZombieSpawner_OnZombieSpawned(Zombie zombie)
    {
        zombie.AddDieListener(zombie_OnDie);
    }

    private void zombie_OnDie(Zombie zombie)
    {
        zombie.RemoveDieListener(zombie_OnDie);
        SpawnAmmo(zombie.transform);
    }

    public void SpawnAmmo(Transform sourceTransform)
    {
        bool isNew;
        AmmoTarget target = _targetPool.Get(out isNew);
        
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
        
        target.PickupHolder.Pickable = pickable;
        target.PickupHolder.Activate();
        
        RadarController.Instance.AddTrackedObject(target.RadarTrackable);
        
        if (isNew)
        {
            target.Lifetime.LifetimeDamage = BalancingData.WEAPON_TARGET_HEALTH;
        }
        else
        {
            target.Lifetime.Reset();
        }
        target.AddDieListener(ammoTarget_OnDie);

        target.gameObject.SetActive(true);
        spawned(target);
    }
    
    private void ammoTarget_OnDie(AmmoTarget target)
    {
        RadarController.Instance.RemoveTrackedObject(target.RadarTrackable);

        target.RemoveDieListener(ammoTarget_OnDie);
        PickAmmo(target);

        target.gameObject.SetActive(false);
        _targetPool.Add(target);
    }

    public void PickAmmo(AmmoTarget target)
    {
        var pickable = target.PickupHolder.Pickable;
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
        target.PickupHolder.Clear();
    }
	
    public void Reset()
    {
        _targetPool.Clear();
    }

    public void SetZombieSpawner(ZombieSpawner zombieSpawner)
    {
        _zombieSpawner = zombieSpawner;
    }

    #region Event invocators

    private void spawned(AmmoTarget target)
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