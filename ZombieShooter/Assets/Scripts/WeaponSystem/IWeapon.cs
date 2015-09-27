using System;
using UnityEngine;

public interface IWeapon
{
    void SetValues(string name, float damage, float cooldownDelay, int magazineSize, int bulletType);

    [SerializeField]
    int BulletType { get; }
    [SerializeField]
    int MagazineSize { get; }
    [SerializeField]
    float CooldownDelay { get; }
    [SerializeField]
    float Damage { get; }
    [SerializeField]
    string Name { get; }
    [SerializeField]
    int AvailableAmmo { get; set; }
}

public interface IAmmo
{
    [SerializeField]
    int Type { get; }
    [SerializeField]
    int Amount { get; set; }
}