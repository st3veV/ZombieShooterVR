using System;
using UnityEngine;

[Serializable]
public class Weapon
{
    [SerializeField]
    private int _bulletType;
    [SerializeField]
    private int _magazineSize;
    [SerializeField]
    private float _cooldownDelay;
    [SerializeField]
    private float _damage;
    [SerializeField]
    private string _name;
    [SerializeField]
    private int _availableAmmo;
    [SerializeField]
    private GameObject _weaponModel;

    
    public void SetValues(string name, float damage, float cooldownDelay, int magazineSize, int bulletType, int initialAmmo, GameObject weaponModel)
    {
        _name = name;
        _damage = damage;
        _cooldownDelay = cooldownDelay;
        _magazineSize = magazineSize;
        _bulletType = bulletType;
        _availableAmmo = initialAmmo;
        _weaponModel = weaponModel;
    }

    public int BulletType
    {
        get { return _bulletType; }
    }

    public int MagazineSize
    {
        get { return _magazineSize; }
    }

    public float CooldownDelay
    {
        get { return _cooldownDelay; }
    }

    public float Damage
    {
        get { return _damage; }
    }

    public string Name
    {
        get { return _name; }
    }

    public int AvailableAmmo
    {
        get { return _availableAmmo; }
    }

    public GameObject WeaponModel
    {
        get { return _weaponModel; }
    }
}

public class PlayerWeapon : IWeapon
{
    private readonly Weapon _realWeapon;
    private int _availableAmmo;

    public PlayerWeapon(Weapon realWeapon)
    {
        _realWeapon = realWeapon;
        _availableAmmo = _realWeapon.AvailableAmmo;
    }

    public int BulletType
    {
        get { return _realWeapon.BulletType; }
    }

    public int MagazineSize
    {
        get { return _realWeapon.MagazineSize; }
    }

    public float CooldownDelay
    {
        get { return _realWeapon.CooldownDelay; }
    }

    public float Damage
    {
        get { return _realWeapon.Damage; }
    }

    public string Name
    {
        get { return _realWeapon.Name; }
    }

    public int AvailableAmmo
    {
        get { return _availableAmmo; }
        set { _availableAmmo = value; }
    }

    public GameObject WeaponModel
    {
        get { return _realWeapon.WeaponModel; }
    }
}