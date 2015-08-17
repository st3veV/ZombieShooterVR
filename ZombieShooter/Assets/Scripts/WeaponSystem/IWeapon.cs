public interface IWeapon
{
    int BulletType { get; }
    int MagazineSize { get; }
    float CooldownDelay { get; }
    float Damage { get; }
    string Name { get; }
}

public interface IAmmo
{
    int Type { get; }
}