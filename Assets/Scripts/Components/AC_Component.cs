using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for all components that can be added to an AC_Entity.
/// </summary>
public abstract class AC_Component: ScriptableObject
{
    [HideInInspector]
    public AC_Entity entity;
    [SerializeField] public abstract Enum_ComponentType componentType { get; }


    public abstract void ComponentAwake();
    public abstract void ComponentStart();
    public abstract void ComponentUpdate();
    public virtual void ComponentFixedUpdate() { }
    public abstract void ComponentDisable();
    public virtual void ComponentDestroy() 
    { 
        Debug.Log(name + " Component Destroyed");
        entity.OnDestroyTick -= ComponentDestroy;
        Destroy(this);
    }

}