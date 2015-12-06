using System.Collections;
using Controllers;
using UnityEngine;

[RequireComponent(typeof (LifetimeComponent))]
public class PickupHolder : MonoBehaviour
{
    public IPickable Pickable;

    public MeshRenderer PickableVisualizer;

    private LifetimeComponent _lifetime;
    private bool _scheduleLifetimeListener = false;
    private bool _shouldKill = false;
    
    void Start()
    {
        _lifetime = GetComponent<LifetimeComponent>();
        if (_scheduleLifetimeListener)
        {
            _scheduleLifetimeListener = false;
            _lifetime.OnDie += _lifetime_OnDie;
        }
    }

    private void _lifetime_OnDie(LifetimeComponent obj)
    {
        _shouldKill = false;
        _lifetime.OnDie -= _lifetime_OnDie;
    }

    public void Activate()
    {
        if (_lifetime == null)
        {
            _scheduleLifetimeListener = true;
        }
        else
        {
            _lifetime.OnDie += _lifetime_OnDie;
        }
        _shouldKill = true;
        StartCoroutine(AutoPickup());

        Texture decal = Pickable.Weapon.BulletImage;

        PickableVisualizer.material.mainTexture = decal;
        
        Vector3 scale = PickableVisualizer.transform.localScale;
        float ratio = (float)decal.width / decal.height;
        
        scale.y = scale.x / ratio;
        scale.x = scale.y * ratio;
        if (scale.x != scale.y * ratio)
        {
            scale.x = scale.y * ratio;
        }
        scale.Normalize();
        PickableVisualizer.transform.localScale = scale;
        
    }

    private IEnumerator AutoPickup()
    {
        yield return new WaitForSeconds(BalancingData.WeaponTargetAutomaticPickupDelay);
        if (_shouldKill)
        {
            _lifetime.ReceiveDamage(BalancingData.WeaponTargetHealth);
        }
    }
    
    public void Clear()
    {
        this.Pickable = null;
        _shouldKill = false;
    }
}

public interface IPickable
{
    IAmmo Ammo { get; }
    IWeapon Weapon { get; }

    void SetWeapon(IWeapon weapon);
    void SetAmmo(IAmmo ammo);

    bool ContainsWeapon { get; set; }
    bool ContainsAmmo { get; set; }
}

public class Pickable : IPickable
{
    private IAmmo _ammo;
    private IWeapon _weapon;

    public void SetWeapon(IWeapon weapon)
    {
        _weapon = weapon;
    }

    public void SetAmmo(IAmmo ammo)
    {
        _ammo = ammo;
    }

    public bool ContainsWeapon { get; set; }

    public bool ContainsAmmo { get; set; }

    public IAmmo Ammo
    {
        get { return _ammo; }
    }

    public IWeapon Weapon
    {
        get { return _weapon; }
    }
    

}
