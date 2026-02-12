using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class Player_Entity : AC_Entity
{
    [SerializeField] private List<AC_Component> Components;
    [SerializeField] private Dictionary<Enum_ComponentType, AC_Component> ComponentDict = new();
    [SerializeField] private Def_Gun Gun;

    [HideInInspector] public UnityEvent OnRespawnTrigger;

    private GameObject currentRespawnPoint;

    public override void Start()
    {
        BuildDictionary();
        InitializeInput();
        MassAssign();
        EventSubscribe();
        
        OnAwakeTick?.Invoke();
        OnStartTick?.Invoke();
    }

    private void InitializeInput()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null && inputSO != null)
        {
            inputSO.Initialize(playerInput);
        }
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

        if (currentRespawnPoint != null) {OnRespawnTrigger?.Invoke(); }

        else { OnDeathTrigger?.Invoke(); }
            
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

    T GetComponent<T>(Enum_ComponentType type) where T : AC_Component
    {
        ComponentDict.TryGetValue(type, out AC_Component comp);
        return comp as T;
    }
    
    public EC_Movement movementSO => GetComponent<EC_Movement>(Enum_ComponentType.Movement);
    public EC_Camera cameraSO => GetComponent<EC_Camera>(Enum_ComponentType.Camera);
    public EC_Rigidbody rigidbodySO => GetComponent<EC_Rigidbody>(Enum_ComponentType.RigidBody);
    public SO_detector detectorSO => GetComponent<SO_detector>(Enum_ComponentType.Detector);
    public SO_InputAccess inputSO => GetComponent<SO_InputAccess>(Enum_ComponentType.Input);
}
