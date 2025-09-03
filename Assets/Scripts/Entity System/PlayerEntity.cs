using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
class enumcomponentelement
{
    public Enum_ComponentType componentType;
    public AC_Component component;
    public Enum_ComponentType assignenum(Enum_ComponentType c)
    {
        return componentType = c;
    }
}


public class PlayerEntity : AC_Entity
{
    [SerializeField] private List<AC_Component> Components;
    [SerializeField] private Dictionary<Enum_ComponentType, AC_Component> ComponentDict = new();
    [SerializeField] private Def_Gun Gun;

    AC_Component[] components;

    private void Awake()
    {
        dictionaryCreation();
        MassAssign();
        EventLinker();
    }

    void dictionaryCreation()
    {
        foreach (AC_Component comp in Components)
        {
            if (comp == null) continue;
            if (!ComponentDict.ContainsKey(comp.componentType))
            {
                ComponentDict.Add(comp.componentType, comp);
            }
        }
    }


    public override void Start()
    {
        OnStartTick?.Invoke();
        ((EC_Movement)ComponentDict[Enum_ComponentType.Movement]).ComponentStart();
    }

    public override void Update()
    {
        if (movementSO != null)
        ST_debug.LogState(movementSO.stateManager.currentState.name);
        
        // Pass the current velocity from rigidbody to camera for FOV effects
        // This is now properly routed through the camera's ProcessVelocityForFOV method
        if (rigidbodySO != null && cameraSO != null)
        {
            cameraSO.ProcessVelocityForFOV(Vector3.Dot( rigidbodySO.PlayerPlaneVel, rigidbodySO.PlayerForward));
        }
        
        OnUpdateTick?.Invoke();
    }

    public override void EventLinker()
    {
        // Connect events here
        TryEvent(()=> movementSO.OnCameraMove += cameraSO.UpdateCameraTransform,movementSO,cameraSO);
        TryEvent(()=> movementSO.OnCameraMove += rigidbodySO.RotatePlayer, movementSO,rigidbodySO);

        TryEvent(() => movementSO.OnPlayerMove += rigidbodySO.Move, movementSO, rigidbodySO);

        TryEvent(()=> OnTriggerEnterTick    += Gun.TriggerEnter, this, Gun);
        TryEvent(()=> OnTriggerExitTick     += Gun.TriggerEnter, this, Gun);
    }

    public void MassAssign()
    {


        foreach (AC_Component component in ComponentDict.Values)
        {
            component.entity = this;
            component.ComponentAwake();

            TryEvent(()=> OnStartTick           += component.ComponentStart,        this, component);
            TryEvent(()=> OnUpdateTick          += component.ComponentUpdate,       this, component);
            TryEvent(()=> OnFixedUpdateTick     += component.ComponentFixedUpdate,  this, component);
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterTick?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitTick?.Invoke(other);
    }

    public override void FixedUpdate()
    {
        OnFixedUpdateTick?.Invoke();
    }

    public override void Ondeath()
    {
        OnDeathTrigger.Invoke();
    }

    private void OnDisable()
    {
        // Unsubscribe EVERYTHING
        foreach (AC_Component component in ComponentDict.Values)
        {

            TryEvent(() => OnStartTick          -= component.ComponentStart,        this, component);
            TryEvent(() => OnUpdateTick         -= component.ComponentUpdate,       this, component);
            TryEvent(() => OnFixedUpdateTick    -= component.ComponentFixedUpdate,  this, component);
        }

        // Disconnect movement events
            TryEvent(() => movementSO.OnCameraMove -= cameraSO.UpdateCameraTransform,   this, cameraSO);
            TryEvent(() => movementSO.OnCameraMove -= rigidbodySO.RotatePlayer,         this, rigidbodySO);
        
        ;

        // Disconnect trigger events

        TryEvent(() => OnTriggerEnterTick -= Gun.TriggerEnter, this, Gun);
        TryEvent(() => OnTriggerExitTick -= Gun.TriggerEnter, this, Gun);
    }
    
    AC_Component getComponenet (Enum_ComponentType type)
    {
        ComponentDict.TryGetValue(type, out AC_Component comp);
        return comp;
    }

    public EC_Movement movementSO      => getComponenet(Enum_ComponentType.Movement) as EC_Movement;
    public EC_Camera cameraSO          => getComponenet(Enum_ComponentType.Camera) as EC_Camera;
    public EC_Rigidbody rigidbodySO    => getComponenet(Enum_ComponentType.RigidBody) as EC_Rigidbody;

}
