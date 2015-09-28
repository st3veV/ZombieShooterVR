using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Thalmic.Myo;

public class InventorySystem
{

    public Gun UserGun;

    private List<IWeapon> _availableWeapons;
    private int _currentWeaponIndex;

	// Use this for initialization
	public void Init ()
	{
	    _availableWeapons = new List<IWeapon>();
	}
	
    public void AddWeapon(IWeapon weapon)
    {
        if (_availableWeapons.IndexOf(weapon) == -1)
        {
            _availableWeapons.Add(weapon);
        }
        if (weapon.Damage >= _availableWeapons[_currentWeaponIndex].Damage)
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

        if (newIndex != _currentWeaponIndex)
        {
            SetWeapon(newIndex);
        }
    }

    private void SetWeapon(int index)
    {
        _currentWeaponIndex = index;
        UserGun.SetWeapon(_availableWeapons[_currentWeaponIndex]);
    }
}

public class Inventory : MonoBehaviour
{
    public Gun UserGun;
    public ThalmicMyo Myo;
    private InventorySystem _inventory;

    void Start()
    {
        Debug.Log(("inventory.start"));
        InitInventory();
    }

    void Update()
    {
        switch (Myo.pose)
        {
            case Pose.WaveIn:
                _inventory.SwitchWeapon(-1);
                break;
            case Pose.WaveOut:
                _inventory.SwitchWeapon(+1);
                break;
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


}