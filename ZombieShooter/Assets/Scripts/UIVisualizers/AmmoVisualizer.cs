﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AmmoVisualizer : MonoBehaviour
{

    public Text AmmoCountOutput;
    public Text WeaponNameOutput;
    public Text TotalGunAmmoOutput;
    public GameObject ReloadingIndicatorOutput;
    public Slider SliderOutput;

    public Gun GunInput;
    private IWeapon _currentWeapon;

    // Use this for initialization
	void Start () {
        GunInput.OnWeaponChange += GunInput_OnWeaponChange;
        GunInput.TriggerWeaponChange();
	}

    void GunInput_OnWeaponChange(IWeapon weapon)
    {
        _currentWeapon = weapon;
        SliderOutput.minValue = 0;
        SliderOutput.maxValue = _currentWeapon.MagazineSize;
        WeaponNameOutput.text = _currentWeapon.Name;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    AmmoCountOutput.text = GunInput.ShellsInMagazine + "";
        SliderOutput.value = GunInput.ShellsInMagazine;
        TotalGunAmmoOutput.text = _currentWeapon.AvailableAmmo + "";

        ReloadingIndicatorOutput.SetActive(GunInput.ShellsInMagazine == 0);
	}
}
