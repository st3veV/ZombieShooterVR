﻿using UnityEngine;

public interface IWeapon
{
    int BulletType { get; }
    int MagazineSize { get; }
    float CooldownDelay { get; }
    float Damage { get; }
    string Name { get; }
    int AvailableAmmo { get; set; }
    GameObject WeaponModel { get; }

    AudioClip ShootSound { get; }
    AudioClip ReloadSound { get; }
    AudioClip KlickSound { get; }

    Texture BulletImage { get; }
    Texture WeaponImage { get; }

    float BulletSpreadAngle { get; }
    int NumBulletsPerShot { get; }
}

public interface IAmmo
{
    [SerializeField]
    int Type { get; }
    [SerializeField]
    int Amount { get; set; }
}