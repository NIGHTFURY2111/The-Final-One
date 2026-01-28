using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for all components that can be added to an AC_Entity.
/// Non-generic base for Unity serialization compatibility.
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
        entity.OnDestroyTick.RemoveListener(ComponentDestroy);
        Destroy(this);
    }
}

/// <summary>
/// Generic CRTP version of AC_Component for type-safe component implementations.
/// </summary>
public abstract class AC_Component<T> : AC_Component where T : AC_Component<T>
{
    // Inherits all functionality from non-generic base
    // Provides compile-time type safety for derived classes
}