using System;
using System.Collections;
using Controllers;
using UnityEngine;

public class Gun : MonoBehaviour {
    public event Action OnWeaponKick;
    public event Action OnWeaponFire;
    public event Action<Vector3> OnWeaponFireTo;
    public event Action OnWeaponKlick;
    public event Action OnWeaponReload;
    public event Action<IWeapon> OnWeaponChange;

    public GameObject Flashlight;

    public bool FiringEnabled;

    private bool _isFiring = false;
    private IWeapon _currentWeapon;
    private int _shellsInMagazine;

    private GameObject _weaponContainer;

    void Awake ()
    {
        _weaponContainer = transform.FindChild("GunContainer").gameObject;
    }

    private bool _isCoroutineRunning = false;
    private IEnumerator FireCoroutine()
    {
        _isCoroutineRunning = true;
        while (_isFiring)
        {
            if (_shellsInMagazine > 0)
            {
                _shellsInMagazine--;
                Fire();
            }
            else
            {
                Klick();
            }
            yield return new WaitForSeconds(_currentWeapon.CooldownDelay/1000);
        }
        _isCoroutineRunning = false;
    }

    private void Klick()
    {
        WeaponKlick();
    }

    public void Fire()
    {
        Kick();
        WeaponFire();
        WeaponFireTo(transform.forward);
    }

    public void StartShooting()
    {
        if (FiringEnabled && _isFiring == false)
        {
            _isFiring = true;
            if (!_isCoroutineRunning)
            {
                StartCoroutine(FireCoroutine());
            }
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

    public void Reset()
    {
    }

    public void SetFlashlightEnabled(bool isEnabled)
    {
        Flashlight.SetActive(isEnabled);
    }

    #region Event invocators

    protected virtual void Kick()
    {
        var handler = OnWeaponKick;
        if (handler != null) handler();
    }

    protected virtual void OnOnWeaponChange(IWeapon obj)
    {
        var handler = OnWeaponChange;
        if (handler != null) handler(obj);
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

    protected virtual void WeaponFireTo(Vector3 obj)
    {
        var handler = OnWeaponFireTo;
        if (handler != null) handler(obj);
    }

    #endregion
}
