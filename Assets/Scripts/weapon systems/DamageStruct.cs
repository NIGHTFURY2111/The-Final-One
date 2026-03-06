using System;
using UnityEngine;

[Flags]
public enum DamageTypeEnum
{
    Hit
}

public struct DamageStruct
{
    public DamageTypeEnum damageType;
    public int amount;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
    public AC_Entity source;
    public bool isCritical;

    public DamageStruct(DamageTypeEnum damageType, int amount, Vector3 hitPoint, Vector3 hitNormal, AC_Entity source, bool isCritical = false)
    {
        this.damageType = damageType;
        this.amount = amount;
        this.hitPoint = hitPoint;
        this.hitNormal = hitNormal;
        this.source = source;
        this.isCritical = isCritical;
    }
}
