using UnityEngine;

[RequireComponent(typeof (LifetimeComponent))]
public class PickupHolder : MonoBehaviour
{
    public IPickable Pickable;

    public MeshRenderer PickableVisualizer;

    private LifetimeComponent _lifetime;
    private InternalTimer timer;

    void Awake()
    {
        Debug.Log("awaken");
    }

    void Start()
    {
        Debug.Log("started");
        _lifetime = GetComponent<LifetimeComponent>();
        AssertTimer();
        timer.Reset();
    }

    void Update()
    {
        if (timer.Update())
        {
            _lifetime.ReceiveDamage(BalancingData.WEAPON_TARGET_HEALTH);
        }
    }

    public void Activate()
    {
        AssertTimer();
        timer.Reset();
        //PickableVisualizer.material.mainTexture = Pickable.Weapon.BulletImage;
    }

    private void AssertTimer()
    {
        if (timer == null)
        {
            timer = new InternalTimer();
            timer.Set(BalancingData.WEAPON_TARGET_AUTOMATIC_PICKUP_DELAY * 1000);
        }
    }

    public void Clear()
    {
        this.Pickable = null;
    }
}

public interface IPickable
{
    IAmmo Ammo { get; }
    IWeapon Weapon { get; }

    void SetItem(IAmmo ammo);
    void SetItem(IWeapon weapon);
}

public class Pickable : IPickable
{
    private IAmmo _ammo;
    private IWeapon _weapon;

    public IAmmo Ammo
    {
        get { return _ammo; }
    }

    public IWeapon Weapon
    {
        get { return _weapon; }
    }

    public void SetItem(IAmmo ammo)
    {
        _ammo = ammo;
    }

    public void SetItem(IWeapon weapon)
    {
        _weapon = weapon;
    }

}
