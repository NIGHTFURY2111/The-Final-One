using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AC_Component: ScriptableObject
{
    [HideInInspector]
    public Entity entity;
    [SerializeField] public abstract ComponentType componentType { get; }


    public abstract void ComponentAwake();
    public abstract void ComponentStart();
    public abstract void ComponentUpdate();

}
