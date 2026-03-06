public enum BulletTypeEnum
{
    hitscan
}

public abstract class AbsBulletClass
{
    public DamageStruct damageInfo;
    public BulletTypeEnum bulletType;
    public float speed;
}
