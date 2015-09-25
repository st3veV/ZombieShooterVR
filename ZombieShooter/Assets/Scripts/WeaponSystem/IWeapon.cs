public interface IWeapon
{
    void SetValues(string name, float damage, float cooldownDelay, int magazineSize, int bulletType);

    int BulletType { get; }
    int MagazineSize { get; }
    float CooldownDelay { get; }
    float Damage { get; }
    string Name { get; }
    int AvailableAmmo { get; set; }
}

public interface IAmmo
{
    int Type { get; }
    int Amount { get; set; }
}