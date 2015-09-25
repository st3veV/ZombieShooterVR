public class BasicGun : IWeapon
{
    public void SetValues(string name, float damage, float cooldownDelay, int magazineSize, int bulletType)
    {
    }

    public int BulletType
    {
        get { return 1; }
    }

    public int MagazineSize
    {
        get { return 7; }
    }

    public float CooldownDelay
    {
        get { return 400.0f; }
    }

    public float Damage
    {
        get { return BalancingData.BULLET_DAMAGE; }
    }

    public string Name
    {
        get { return "Basic gun"; }
    }

    public int AvailableAmmo { get; set; }
}

public class BasicWeaponAmmo : IAmmo
{
    public BasicWeaponAmmo(int initialAmount)
    {
        Amount = initialAmount;
    }

    public int Type
    {
        get { return 1; }
    }

    public int Amount { get; set; }
}