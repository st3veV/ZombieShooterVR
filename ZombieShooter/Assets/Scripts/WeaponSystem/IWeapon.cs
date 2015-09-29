using System;
using UnityEngine;

public interface IWeapon
{
    int BulletType { get; }
    int MagazineSize { get; }
    float CooldownDelay { get; }
    float Damage { get; }
    string Name { get; }
    int AvailableAmmo { get; set; }
    GameObject WeaponModel { get; }
}

public interface IAmmo
{
    [SerializeField]
    int Type { get; }
    [SerializeField]
    int Amount { get; set; }
}