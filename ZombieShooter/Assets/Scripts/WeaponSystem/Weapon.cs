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

    public void SetValues(string name, float damage, float cooldownDelay, int magazineSize, int bulletType,
        int initialAmmo, GameObject weaponModel, AudioClip shootSound, AudioClip reloadSound, AudioClip klickSound,
        Texture bulletImage, Texture weaponImage, float spreadAngle, int numBulletsPerShot)
    {
        _name = name;
        _damage = damage;
        _cooldownDelay = cooldownDelay;
        _magazineSize = magazineSize;
        _bulletType = bulletType;
        _availableAmmo = initialAmmo;
        _weaponModel = weaponModel;
        _shootSound = shootSound;
        _reloadSound = reloadSound;
        _klickSound = klickSound;
        _bulletImage = bulletImage;
        _weaponImage = weaponImage;
        _bulletSpreadAngle = spreadAngle;
        _numBulletsPerShot = numBulletsPerShot;
    }

    public int BulletType { get { return _bulletType; } }
    public int MagazineSize { get { return _magazineSize; } }
    public float CooldownDelay { get { return _cooldownDelay; } }
    public float Damage { get { return _damage; } }
    public string Name { get { return _name; } }
    public int AvailableAmmo { get { return _availableAmmo; } }
    public GameObject WeaponModel { get { return _weaponModel; } }

    public AudioClip ShootSound { get { return _shootSound; } }
    public AudioClip ReloadSound { get { return _reloadSound; } }
    public AudioClip KlickSound { get { return _klickSound; } }

    public Texture BulletImage { get { return _bulletImage; } }
    public Texture WeaponImage { get { return _weaponImage; } }

    public float BulletSpreadAngle { get { return _bulletSpreadAngle; } }
    public int NumBulletsPerShot { get { return _numBulletsPerShot; } }
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

    public int BulletType { get { return _realWeapon.BulletType; } }
    public int MagazineSize { get { return _realWeapon.MagazineSize; } }
    public float CooldownDelay { get { return _realWeapon.CooldownDelay; } }
    public float Damage { get { return _realWeapon.Damage; } }
    public string Name { get { return _realWeapon.Name; } }

    public int AvailableAmmo
    {
        get { return _availableAmmo; }
        set { _availableAmmo = value; }
    }
    public GameObject WeaponModel { get { return _realWeapon.WeaponModel; } }

    public AudioClip ShootSound { get { return _realWeapon.ShootSound; } }
    public AudioClip ReloadSound { get { return _realWeapon.ReloadSound; } }
    public AudioClip KlickSound { get { return _realWeapon.KlickSound; } }

    public Texture BulletImage { get { return _realWeapon.BulletImage; } }
    public Texture WeaponImage { get { return _realWeapon.WeaponImage; } }

    public float BulletSpreadAngle { get { return _realWeapon.BulletSpreadAngle; } }
    public int NumBulletsPerShot { get { return _realWeapon.NumBulletsPerShot; } }
}