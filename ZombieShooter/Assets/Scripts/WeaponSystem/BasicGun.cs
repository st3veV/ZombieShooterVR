
class BasicGun : IWeapon
{
    public int BulletType
    {
        get { return 0; }
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
}
