public interface Abs_WeaponClass
{
    public string weaponName { get; }
    public int clipSize { get; }
    public float fireRate { get; }
    public float reloadTime { get; }
    public AbsBulletClass Bullet { get; }
    public abstract void onShoot();
}
