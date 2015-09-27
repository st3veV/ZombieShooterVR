using System;
using UnityEngine;

[Serializable]
public class Weapon : IWeapon
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

    public void SetValues(string name, float damage, float cooldownDelay, int magazineSize, int bulletType)
    {
        _name = name;
        _damage = damage;
        _cooldownDelay = cooldownDelay;
        _magazineSize = magazineSize;
        _bulletType = bulletType;
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
        set { _availableAmmo = value; }
    }
}
