using UnityEngine;

[RequireComponent(typeof (LifetimeComponent))]
public class PickupHolder : MonoBehaviour
{
    public IPickable Pickable;

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
        timer = timer ?? new InternalTimer();
        timer.Set(5000);
    }

    void Update()
    {
        if (timer.Update())
        {
            _lifetime.ReceiveDamage(100);
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
