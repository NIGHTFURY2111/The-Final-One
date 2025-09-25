using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerEntity : AC_Entity
{
    [SerializeField] private List<AC_Component> Components;
    [SerializeField] private Dictionary<Enum_ComponentType, AC_Component> ComponentDict = new();
    [SerializeField] private Def_Gun Gun;


    private void Awake()
    {
        BuildDictionary();
        MassAssign();
        EventSubscribe();
        
        // Only invoke OnAwakeTick after everything is properly set up
        OnAwakeTick?.Invoke();
    }

    void BuildDictionary()
    {
        ComponentDict.Clear();
        
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
    }

    public override void Update()
    {
        if (movementSO != null && movementSO.stateManager?.currentState != null)
        {
            ST_debug.LogState(movementSO.stateManager.currentState.name);
        }
        
        // Pass the current velocity from rigidbody to camera for FOV effects
        if (rigidbodySO != null && cameraSO != null)
        {
            cameraSO.ProcessVelocityForFOV(Vector3.Dot(rigidbodySO.PlayerPlaneVel, rigidbodySO.PlayerForward));
        }
        
        OnUpdateTick?.Invoke();
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


    private void OnDisable()
    {
        // Call component disable first
        OnDisableTick?.Invoke();
        
        // Then unsubscribe everything to prevent memory leaks
        UnsubscribeAllEvents();
    }

    public override void Ondeath()
    {
        OnDeathTrigger.Invoke();
    }
    public override void EventSubscribe()
    {
        // Connect events here - using safe event linking
        TryEvent(() => movementSO.OnCameraMove += cameraSO.UpdateCameraTransform, movementSO, cameraSO);
        TryEvent(() => movementSO.OnCrouch += cameraSO.crouchCameraPosition, movementSO, cameraSO);
        TryEvent(() => movementSO.OnCameraMove += rigidbodySO.RotatePlayer, movementSO, rigidbodySO);
        TryEvent(() => movementSO.OnPlayerMove += rigidbodySO.Move, movementSO, rigidbodySO);
        TryEvent(() => OnTriggerEnterTick += Gun.TriggerEnter, this, Gun);
        TryEvent(() => OnTriggerExitTick += Gun.TriggerExit, this, Gun); // Fixed: was TriggerEnter
    }
    public override void EventUnsubscribe()
    {
        // Connect events here - using safe event linking
        TryEvent(() => movementSO.OnCameraMove -= cameraSO.UpdateCameraTransform, movementSO, cameraSO);
        TryEvent(() => movementSO.OnCrouch -= cameraSO.crouchCameraPosition, movementSO, cameraSO);
        TryEvent(() => movementSO.OnCameraMove -= rigidbodySO.RotatePlayer, movementSO, rigidbodySO);
        TryEvent(() => movementSO.OnPlayerMove -= rigidbodySO.Move, movementSO, rigidbodySO);
        TryEvent(() => OnTriggerEnterTick -= Gun.TriggerEnter, this, Gun);
        TryEvent(() => OnTriggerExitTick -= Gun.TriggerExit, this, Gun); // Fixed: was TriggerEnter
    }

    public void MassAssign()
    {
        foreach (AC_Component component in ComponentDict.Values)
        {
            if (component == null) continue;

            component.entity = this;

            // Subscribe to events - these should only happen once per component
            TryEvent(() => OnAwakeTick += component.ComponentAwake, this, component);
            TryEvent(() => OnDisableTick += component.ComponentDisable, this, component);
            TryEvent(() => OnStartTick += component.ComponentStart, this, component);
            TryEvent(() => OnUpdateTick += component.ComponentUpdate, this, component);
            TryEvent(() => OnFixedUpdateTick += component.ComponentFixedUpdate, this, component);
        }
    }

    private void UnsubscribeAllEvents()
    {
        // Unsubscribe component events
        foreach (AC_Component component in ComponentDict.Values)
        {
            if (component == null) continue;
            
            TryEvent(() => OnAwakeTick -= component.ComponentAwake, this, component);
            TryEvent(() => OnDisableTick -= component.ComponentDisable, this, component);
            TryEvent(() => OnStartTick -= component.ComponentStart, this, component);
            TryEvent(() => OnUpdateTick -= component.ComponentUpdate, this, component);
            TryEvent(() => OnFixedUpdateTick -= component.ComponentFixedUpdate, this, component);
        }

        EventUnsubscribe();
    }

    AC_Component getComponenet(Enum_ComponentType type)
    {
        ComponentDict.TryGetValue(type, out AC_Component comp);
        return comp;
    }

    public EC_Movement movementSO => getComponenet(Enum_ComponentType.Movement) as EC_Movement;
    public EC_Camera cameraSO => getComponenet(Enum_ComponentType.Camera) as EC_Camera;
    public EC_Rigidbody rigidbodySO => getComponenet(Enum_ComponentType.RigidBody) as EC_Rigidbody;
}
