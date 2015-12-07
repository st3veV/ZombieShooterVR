using System;
using UnityEngine;

[Serializable]
public class Weapon
{
    [SerializeField] private int _bulletType;
    [SerializeField] private int _magazineSize;
    [SerializeField] private float _cooldownDelay;
    [SerializeField] private float _damage;
    [SerializeField] private string _name;
    [SerializeField] private int _availableAmmo;
    [SerializeField] private GameObject _weaponModel;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private AudioClip _reloadSound;
    [SerializeField] private AudioClip _klickSound;
    [SerializeField] private Texture _bulletImage;
    [SerializeField] private Texture _weaponImage;
    [SerializeField] private float _bulletSpreadAngle;
    [SerializeField] private int _numBulletsPerShot;
   
    public int BulletType
    {
        get { return _bulletType; }
        set { _bulletType = value; }
    }

    public int MagazineSize
    {
        get { return _magazineSize; }
        set { _magazineSize = value; }
    }

    public float CooldownDelay
    {
        get { return _cooldownDelay; }
        set { _cooldownDelay = value; }
    }

    public float Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public int AvailableAmmo
    {
        get { return _availableAmmo; }
        set { _availableAmmo = value; }
    }

    public GameObject WeaponModel
    {
        get { return _weaponModel; }
        set { _weaponModel = value; }
    }

    public AudioClip ShootSound
    {
        get { return _shootSound; }
        set { _shootSound = value; }
    }

    public AudioClip ReloadSound
    {
        get { return _reloadSound; }
        set { _reloadSound = value; }
    }

    public AudioClip KlickSound
    {
        get { return _klickSound; }
        set { _klickSound = value; }
    }

    public Texture BulletImage
    {
        get { return _bulletImage; }
        set { _bulletImage = value; }
    }

    public Texture WeaponImage
    {
        get { return _weaponImage; }
        set { _weaponImage = value; }
    }

    public float BulletSpreadAngle
    {
        get { return _bulletSpreadAngle; }
        set { _bulletSpreadAngle = value; }
    }

    public int NumBulletsPerShot
    {
        get { return _numBulletsPerShot; }
        set { _numBulletsPerShot = value; }
    }
}

public class PlayerWeapon : IWeapon
{
    private readonly Weapon _realWeapon;

    public PlayerWeapon(Weapon realWeapon)
    {
        _realWeapon = realWeapon;
        AvailableAmmo = _realWeapon.AvailableAmmo;
    }

    public int BulletType { get { return _realWeapon.BulletType; } }
    public int MagazineSize { get { return _realWeapon.MagazineSize; } }
    public float CooldownDelay { get { return _realWeapon.CooldownDelay; } }
    public float Damage { get { return _realWeapon.Damage; } }
    public string Name { get { return _realWeapon.Name; } }

    public int AvailableAmmo { get; set; }

    public GameObject WeaponModel { get { return _realWeapon.WeaponModel; } }

    public AudioClip ShootSound { get { return _realWeapon.ShootSound; } }
    public AudioClip ReloadSound { get { return _realWeapon.ReloadSound; } }
    public AudioClip KlickSound { get { return _realWeapon.KlickSound; } }

    public Texture BulletImage { get { return _realWeapon.BulletImage; } }
    public Texture WeaponImage { get { return _realWeapon.WeaponImage; } }

    public float BulletSpreadAngle { get { return _realWeapon.BulletSpreadAngle; } }
    public int NumBulletsPerShot { get { return _realWeapon.NumBulletsPerShot; } }
}