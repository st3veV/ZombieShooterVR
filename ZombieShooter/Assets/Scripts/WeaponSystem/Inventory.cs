using System.Collections.Generic;
using Controllers;
using Thalmic.Myo;
using UnityEngine;

public class InventorySystem
{

    public Gun UserGun;

    private List<IWeapon> _availableWeapons;
    private int _currentWeaponIndex = 0;

	// Use this for initialization
	public void Init ()
	{
	    _availableWeapons = new List<IWeapon>();
	}
	
    public void AddWeapon(IWeapon weapon)
    {
        bool firstWeapon = _availableWeapons.Count == 0;
        bool newWeapon = false;
        if (_availableWeapons.IndexOf(weapon) == -1)
        {
            _availableWeapons.Add(weapon);
            newWeapon = true;
        }

        if (firstWeapon)
        {
            SetWeapon(0);
        }
        else if (newWeapon && weapon.Damage > _availableWeapons[_currentWeaponIndex].Damage)
        {
            SetWeapon(_availableWeapons.IndexOf(weapon));
        }
        
    }

    public void AddAmmo(IAmmo ammo)
    {
        foreach (IWeapon weapon in _availableWeapons)
        {
            if (weapon.BulletType == ammo.Type)
            {
                weapon.AvailableAmmo += ammo.Amount;
                break;
            }
        }
    }

    public void SwitchWeapon(int offset)
    {
        int newIndex = _currentWeaponIndex + offset;
        if (newIndex < 0)
            newIndex += _availableWeapons.Count;
        else
            newIndex = newIndex%_availableWeapons.Count;

        Debug.Log("Switch weapon: " + _currentWeaponIndex + " " + offset + " = " + newIndex + "/" +
                  _availableWeapons.Count);
        if (newIndex != _currentWeaponIndex)
        {
            SetWeapon(newIndex);
        }
    }

    private void SetWeapon(int index)
    {
        //Debug.Log("Setting weapon: " + index);
        _currentWeaponIndex = index;
        UserGun.SetWeapon(_availableWeapons[_currentWeaponIndex]);
    }

    public void Reset()
    {
        Init();
    }
}

public class Inventory : MonoBehaviour
{
    public Gun UserGun;
    private InventorySystem _inventory;
    private Pose _currentPose;
    private ThalmicMyo _myo;

    private void Start()
    {
        //Debug.Log(("inventory.start"));
        _myo = ThalmicHub.instance.GetComponentInChildren<ThalmicMyo>();
        EventManager.Instance.AddUpdateListener(OnUpdate);
        InitInventory();
    }

    private void OnUpdate()
    {
        if (_currentPose != _myo.pose)
        {
            _currentPose = _myo.pose;
            switch (_currentPose)
            {
                case Pose.WaveIn:
                    _inventory.SwitchWeapon(-1);
                    break;
                case Pose.WaveOut:
                    _inventory.SwitchWeapon(+1);
                    break;
            }
        }
    }

    public void PickWeapon(IWeapon weapon)
    {
        InitInventory();
        _inventory.AddWeapon(weapon);
    }

    public void PickAmmo(IAmmo ammo)
    {
        InitInventory();
        _inventory.AddAmmo(ammo);
    }

    private void InitInventory()
    {
        if (_inventory == null)
        {
            _inventory = new InventorySystem {UserGun = UserGun};
            _inventory.Init();
        }
    }
    
    public void Reset()
    {
        InitInventory();
        _inventory.Reset();
        PickFirstWeapon();
    }

    private void PickFirstWeapon()
    {
        //init - selecting first weapon
        IWeapon weapon = WeaponManager.Instance.GetWeapon(WeaponDatabase.Instance.Weapons[0]);
        PickWeapon(weapon);

        //adding enough bullets
        var modularAmmo = new ModularAmmo();
        modularAmmo.SetValues(weapon.BulletType, weapon.MagazineSize + 100);
        PickAmmo(modularAmmo);
    }
}