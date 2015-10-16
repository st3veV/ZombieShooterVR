using System;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public event Action OnWeaponKick;
    public event Action OnWeaponFire;
    public event Action OnWeaponKlick;
    public event Action OnWeaponReload;
    public event Action<IWeapon> OnWeaponChange;

    public bool FiringEnabled;

    private bool _isFiring = false;
    private InternalTimer _timer;
    private IWeapon _currentWeapon;
    private int _shellsInMagazine;

    private GameObject _weaponContainer;

    // Use this for initialization
    void Awake ()
    {
        _timer = new InternalTimer();

        _weaponContainer = transform.FindChild("GunContainer").gameObject;

    }

    // Update is called once per frame
    void Update () {
        if (_isFiring)
        {
            if (_timer.Update())
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
                _timer.Set(_currentWeapon.CooldownDelay);
            }
        }
    }

    private void Klick()
    {
        WeaponKlick();
    }

    public void Fire()
    {
        Kick();
        WeaponFire();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Collider target = hit.collider;
            float distance = hit.distance;
            Vector3 location = hit.point;
            GameObject targetGo = target.gameObject;
            LifetimeComponent targetLifetime = targetGo.GetComponent<LifetimeComponent>();
            if (targetLifetime != null)
            {
                targetLifetime.ReceiveDamage(_currentWeapon.Damage);
            }
        }
    }

    public void StartShooting()
    {
        if (FiringEnabled && _isFiring == false)
        {
            _isFiring = true;
            _timer.Set(0);
        }
    }

    public void StopShooting()
    {
        if (FiringEnabled && _isFiring)
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
        WeaponReload();
    }

    protected virtual void Kick()
    {
        var handler = OnWeaponKick;
        if (handler != null) handler();
    }

    public void SetWeapon(IWeapon weapon)
    {
        _currentWeapon = weapon;
        LoadWeaponModel(weapon.WeaponModel);
        Reload();
        OnOnWeaponChange(_currentWeapon);
    }

    private void LoadWeaponModel(GameObject weaponModel)
    {
        //delete old gun
        if (_weaponContainer.transform.childCount > 0)
        {
            foreach (Transform childTransform in _weaponContainer.transform) Destroy(childTransform.gameObject);
        }

        //instantiate new gun
        GameObject newWeapon = (GameObject) Instantiate(weaponModel, _weaponContainer.transform.position, _weaponContainer.transform.rotation);
        
        newWeapon.transform.SetParent(_weaponContainer.transform);
        newWeapon.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public int ShellsInMagazine
    {
        get { return _shellsInMagazine; }
    }

    public void TriggerWeaponChange()
    {
        if (_currentWeapon != null)
        {
            OnOnWeaponChange(_currentWeapon);
        }
    }

    protected virtual void OnOnWeaponChange(IWeapon obj)
    {
        var handler = OnWeaponChange;
        if (handler != null) handler(obj);
    }

    public void Reset()
    {
    }

    protected virtual void WeaponKlick()
    {
        var handler = OnWeaponKlick;
        if (handler != null) handler();
    }

    protected virtual void WeaponReload()
    {
        var handler = OnWeaponReload;
        if (handler != null) handler();
    }

    protected virtual void WeaponFire()
    {
        var handler = OnWeaponFire;
        if (handler != null) handler();
    }
}
