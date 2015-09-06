using System;
using UnityEngine;

public class Gun : MonoBehaviour {

    public Rigidbody Bullet;

    public event Action OnWeaponKick;
    public event Action<IWeapon> OnWeaponChange;

    private bool _isFiring = false;
    private float _timer = 0;
    private IWeapon _currentWeapon;
    private int _shellsInMagazine;

    // Use this for initialization
    void Start ()
    {
        _timer = 0;
    }
    
    // Update is called once per frame
    void Update () {
        if (_isFiring)
        {
            _timer -= (Time.deltaTime * 1000f);
            if (_timer <= 0)
            {
                if (_shellsInMagazine > 0)
                {
                    Fire();
                    _shellsInMagazine--;
                }
                else
                {
                    Klick();
                }
                _timer = _currentWeapon.CooldownDelay;
            }
        }
    }

    private void Klick()
    {
        Debug.Log("Klick!");
    }

    public void Fire()
    {
        Kick();
        //Debug.Log("Fire!");
        Rigidbody clone = Instantiate(Bullet, transform.position, transform.rotation) as Rigidbody;
        Bullet bulletClone = clone.GetComponent<Bullet>();
        bulletClone.SetDamage(_currentWeapon.Damage);
        clone.AddForce(clone.transform.forward * 1500);
    }

    public void StartShooting()
    {
        if (_isFiring == false)
        {
            _isFiring = true;
            _timer = 0;
        }
    }

    public void StopShooting()
    {
        if (_isFiring)
        {
            _isFiring = false;
            Kick();
        }
    }

    public void Reload()
    {
        if (_currentWeapon.AvailableAmmo <= 0) return;

        if (_currentWeapon.AvailableAmmo >= _currentWeapon.MagazineSize)
        {
            _currentWeapon.AvailableAmmo -= _currentWeapon.MagazineSize;
            _shellsInMagazine = _currentWeapon.MagazineSize;
        }
        else
        {
            _shellsInMagazine = _currentWeapon.AvailableAmmo;
            _currentWeapon.AvailableAmmo = 0;
        }
    }

    protected virtual void Kick()
    {
        var handler = OnWeaponKick;
        if (handler != null) handler();
    }

    public void SetWeapon(IWeapon weapon)
    {
        _currentWeapon = weapon;
        Reload();
        OnOnWeaponChange(_currentWeapon);
    }

    public int ShellsInMagazine
    {
        get { return _shellsInMagazine; }
    }

    protected virtual void OnOnWeaponChange(IWeapon obj)
    {
        var handler = OnWeaponChange;
        if (handler != null) handler(obj);
    }
}
