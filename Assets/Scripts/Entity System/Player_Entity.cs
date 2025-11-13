using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Player_Entity : AC_Entity
{
    [SerializeField] private List<AC_Component> Components;
    [SerializeField] private Dictionary<Enum_ComponentType, AC_Component> ComponentDict = new();
    [SerializeField] private Def_Gun Gun;

    [HideInInspector]public new UnityEvent OnStartTick;
    [HideInInspector]public new UnityEvent OnAwakeTick;
    [HideInInspector]public new UnityEvent OnDisableTick;
    [HideInInspector]public new UnityEvent OnDestroyTick;
    [HideInInspector]public new UnityEvent OnUpdateTick;
    [HideInInspector]public new UnityEvent OnFixedUpdateTick;
    [HideInInspector]public new UnityEvent<Collider> OnTriggerEnterTick;
    [HideInInspector]public new UnityEvent<Collider> OnTriggerExitTick;






    public UnityEvent OnRespawnTrigger;

    private GameObject currentRespawnPoint;

    public override void Start()
    {
        BuildDictionary();
        MassAssign();
        EventSubscribe();
        
        OnAwakeTick?.Invoke();
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
            float processed_vel = Vector3.Dot(rigidbodySO.PlayerPlaneVel, rigidbodySO.PlayerForward);
            cameraSO.ProcessVelocityForFOV(
                rigidbodySO.PlayerPlaneVel.magnitude * Mathf.Clamp01(Vector3.Dot(rigidbodySO.PlayerPlaneVel.normalized, rigidbodySO.PlayerForward) + 1f));
        }
        
        if (!PauseMenu.isPaused) OnUpdateTick?.Invoke();
    }

    public override void FixedUpdate()
    {
        if (!PauseMenu.isPaused) OnFixedUpdateTick?.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterTick?.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitTick?.Invoke(other);
    }



    private void OnDisable()
    {
        // Call component disable first
        OnDisableTick?.Invoke();
        UnsubscribeAllEvents();
        Debug.Log("Player Disabled");
        // Then unsubscribe everything to prevent memory leaks
        ComponentDict.Clear();
        Destroy(this);
        
    }

    private void OnDestroy()
    {
        Debug.Log("Player Destroyed");
        OnDestroyTick?.Invoke();
    }
    void BuildDictionary()
    {
        ComponentDict.Clear();
        
        foreach (AC_Component compref in Components)
        {
            AC_Component comp = Instantiate(compref);

            if (comp == null) continue;
            if (!ComponentDict.ContainsKey(comp.componentType))
            {
                Debug.Log(comp.name + " added to Component Dictionary");
                ComponentDict.Add(comp.componentType, comp);
            }
        }
    }
    public void MassAssign()
    {

        foreach (AC_Component component in ComponentDict.Values)
        {
            if (component == null) continue;
            component.entity = this;

            // Subscribe to events - these should only happen once per component
            TryEvent(() => OnAwakeTick.AddListener(component.ComponentAwake), this, component);
            TryEvent(() => OnDisableTick.AddListener(component.ComponentDisable), this, component);
            TryEvent(() => OnDestroyTick.AddListener(component.ComponentDestroy), this, component);
            TryEvent(() => OnStartTick.AddListener(component.ComponentStart), this, component);
            TryEvent(() => OnUpdateTick.AddListener(component.ComponentUpdate), this, component);
            TryEvent(() => OnFixedUpdateTick.AddListener(component.ComponentFixedUpdate), this, component);
        }
    }
    public override void EventSubscribe()
    {
        Debug.Log("Player Event Subscribe");
        // Connect events here - using safe event linking
        TryEvent(() => movementSO.OnCameraMove.AddListener(cameraSO.UpdateCameraTransform), movementSO, cameraSO);
        TryEvent(() => movementSO.OnCrouch.AddListener(cameraSO.crouchCameraPosition), movementSO, cameraSO);
        TryEvent(() => movementSO.OnCameraMove.AddListener(rigidbodySO.RotatePlayer), movementSO, rigidbodySO);
        TryEvent(() => movementSO.OnPlayerMove.AddListener(rigidbodySO.Move), movementSO, rigidbodySO);
        TryEvent(() => OnTriggerEnterTick.AddListener(Gun.TriggerEnter), this, Gun);
        TryEvent(() => OnTriggerExitTick.AddListener(Gun.TriggerExit), this, Gun); // Fixed: was TriggerEnter
    }
    public override void EventUnsubscribe()
    {
        // Connect events here - using safe event linking
        movementSO.OnCameraMove.RemoveAllListeners();
        movementSO.OnCrouch.RemoveAllListeners();
        movementSO.OnCameraMove.RemoveAllListeners();
        movementSO.OnPlayerMove.RemoveAllListeners();
        TryEvent(() => OnTriggerEnterTick.RemoveListener(Gun.TriggerEnter), this, Gun);
        TryEvent(() => OnTriggerExitTick.RemoveListener(Gun.TriggerExit), this, Gun); // Fixed: was TriggerEnter
    }
    private void UnsubscribeAllEvents()
    {
        // Unsubscribe component events
        foreach (AC_Component component in ComponentDict.Values)
        {
            if (component == null) continue;
            
            OnStartTick.RemoveAllListeners();
            OnUpdateTick.RemoveAllListeners();
            OnFixedUpdateTick.RemoveAllListeners();
            OnDisableTick.RemoveAllListeners();
        }

        EventUnsubscribe();
    }

    public override void Ondeath()
    {
        Debug.Log("died");

        if (currentRespawnPoint != null) { Debug.Log("respawn"); OnRespawnTrigger?.Invoke(); }

        else { Debug.Log("dies"); OnDeathTrigger?.Invoke(); }
            
    }

    public void UpdateRespawnPoint(GameObject newRespawnPoint)
    {
        Debug.Log("set resp");
        currentRespawnPoint = newRespawnPoint;
    }

    public void respawnPlayer()
    {
        if (currentRespawnPoint != null)
        {
            transform.position = currentRespawnPoint.transform.position;
            transform.rotation = currentRespawnPoint.transform.rotation;
        }
        else { OnDeathTrigger?.Invoke(); }
    }

    AC_Component getComponenet(Enum_ComponentType type)
    {
        ComponentDict.TryGetValue(type, out AC_Component comp);
        return comp;
    }
    public EC_Movement movementSO => getComponenet(Enum_ComponentType.Movement) as EC_Movement;
    public EC_Camera cameraSO => getComponenet(Enum_ComponentType.Camera) as EC_Camera;
    public EC_Rigidbody rigidbodySO => getComponenet(Enum_ComponentType.RigidBody) as EC_Rigidbody;
    public SO_detector detectorSO=> getComponenet(Enum_ComponentType.Detector) as SO_detector;
    public SO_InputAccess inputSO=> getComponenet(Enum_ComponentType.Input) as SO_InputAccess;
}
