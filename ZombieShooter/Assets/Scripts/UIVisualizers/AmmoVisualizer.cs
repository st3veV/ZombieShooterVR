using System;
using UnityEngine;
using UnityEngine.UI;

public class AmmoVisualizer : MonoBehaviour
{

    public Text AmmoCountOutput;
    public Text WeaponNameOutput;
    public Text TotalGunAmmoOutput;
    public GameObject ReloadingIndicatorOutput;
    public Slider SliderOutput;
    public RawImage BulletImageOutput;
    public RawImage WeaponImageOutput;
    [Space]
    public Gun GunInput;
    private IWeapon _currentWeapon;

    // Use this for initialization
	void Start () {
        GunInput.OnWeaponChange += GunInput_OnWeaponChange;
	    GunInput.OnWeaponFire += UpdateUi;
	    GunInput.OnWeaponReload += UpdateUi;
        GunInput.TriggerWeaponChange();
	}

    void GunInput_OnWeaponChange(IWeapon weapon)
    {
        _currentWeapon = weapon;
        SliderOutput.minValue = 0;
        SliderOutput.maxValue = _currentWeapon.MagazineSize;
        WeaponNameOutput.text = _currentWeapon.Name;
        BulletImageOutput.texture = _currentWeapon.BulletImage;
        BulletImageOutput.SetNativeSize();
        WeaponImageOutput.texture = _currentWeapon.WeaponImage;
        WeaponImageOutput.SetNativeSize();
        WeaponImageOutput.transform.localScale = new Vector3(.5f, .5f, .5f);

        UpdateUi();
    }

    private void UpdateUi()
    {
        AmmoCountOutput.text = GunInput.ShellsInMagazine + "";
        SliderOutput.value = GunInput.ShellsInMagazine;
        if(_currentWeapon != null)
            TotalGunAmmoOutput.text = _currentWeapon.AvailableAmmo + "";

        ReloadingIndicatorOutput.SetActive(GunInput.ShellsInMagazine == 0);
    }
    
}
