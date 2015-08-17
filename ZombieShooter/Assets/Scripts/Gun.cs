using System;
using UnityEngine;

public class Gun : MonoBehaviour {

    public Rigidbody Bullet;

    public event Action OnWeaponKick;

    private bool _isFiring = false;
    private float timer = 0;
    private IWeapon _currentWeapon;
    private int _shellsInMagazine;

    // Use this for initialization
    void Start ()
    {
        timer = 0;
        SetWeapon(new BasicGun());
    }
    
    // Update is called once per frame
    void Update () {
        if (_isFiring)
        {
            timer -= (Time.deltaTime * 1000f);
            if (timer <= 0)
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
                timer = _currentWeapon.CooldownDelay;
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
            timer = 0;
            //Kick();
        }
    }

    public void StopShooting()
    {
        if (_isFiring)
        {
            //timer = 0;
            _isFiring = false;
            Kick();
        }
    }

    public void Reload()
    {
        _shellsInMagazine = _currentWeapon.MagazineSize;
    }

    protected virtual void Kick()
    {
        var handler = OnWeaponKick;
        if (handler != null) handler();
    }

    public void SetWeapon(IWeapon weapon)
    {
        _currentWeapon = weapon;
        _shellsInMagazine = _currentWeapon.MagazineSize;
    }

    public int ShellsInMagazine
    {
        get { return _shellsInMagazine; }
    }

}
