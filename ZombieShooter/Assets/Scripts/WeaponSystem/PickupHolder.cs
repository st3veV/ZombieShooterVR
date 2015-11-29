using UnityEngine;

[RequireComponent(typeof (LifetimeComponent))]
public class PickupHolder : MonoBehaviour
{
    public IPickable Pickable;

    public MeshRenderer PickableVisualizer;

    private LifetimeComponent _lifetime;
    private InternalTimer _timer;

    void Awake()
    {
        Debug.Log("awaken");
    }

    void Start()
    {
        Debug.Log("started");
        _lifetime = GetComponent<LifetimeComponent>();
        AssertTimer();
        _timer.Reset();
    }

    void Update()
    {
        if (_timer.Update())
        {
            _lifetime.ReceiveDamage(BalancingData.WEAPON_TARGET_HEALTH);
        }
    }

    public void Activate()
    {
        AssertTimer();
        _timer.Reset();

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

    private void AssertTimer()
    {
        if (_timer == null)
        {
            _timer = new InternalTimer();
            _timer.Set(BalancingData.WEAPON_TARGET_AUTOMATIC_PICKUP_DELAY * 1000);
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
