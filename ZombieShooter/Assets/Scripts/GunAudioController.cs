using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunAudioController : MonoBehaviour {

    private AudioSource _gunAudioSource;
    public Gun UserGun;

    private AudioClip shoot;
    private AudioClip klick;
    private AudioClip reload;
	
	void Start ()
	{
	    _gunAudioSource = gameObject.GetComponent<AudioSource>();
	    
        UserGun.OnWeaponChange += UserGun_OnWeaponChange;
        UserGun.OnWeaponKlick += UserGun_OnWeaponKlick;
        UserGun.OnWeaponReload += UserGun_OnWeaponReload;
        UserGun.OnWeaponFire += UserGun_OnWeaponFire;
	}

    void UserGun_OnWeaponFire()
    {
        Fire();
    }

    void UserGun_OnWeaponReload()
    {
        Reload();
    }

    void UserGun_OnWeaponKlick()
    {
        Klick();
    }

    void UserGun_OnWeaponChange(IWeapon obj)
    {
        AssignSounds(obj);
    }

    private void Reload()
    {
        _gunAudioSource.PlayOneShot(reload);
    }

    private void Fire()
    {
        _gunAudioSource.PlayOneShot(shoot);
    }

    private void Klick()
    {
        _gunAudioSource.PlayOneShot(klick);
    }

    private void AssignSounds(IWeapon weapon)
    {
        shoot = weapon.ShootSound;
        klick = weapon.KlickSound;
        reload = weapon.ReloadSound;
    }

}
