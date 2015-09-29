using UnityEngine;
using System.Collections;

public class PickupHolder : MonoBehaviour
{
    public IPickable Pickable;

    public void Clear()
    {
        this.Pickable = null;
    }
}

public interface IPickable
{
    IAmmo Ammo { get; }
    IWeapon Weapon { get; }

    void SetItem(IAmmo ammo);
    void SetItem(IWeapon weapon);
}

public class Pickable : IPickable
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
