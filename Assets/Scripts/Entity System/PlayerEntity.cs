using TMPro;
using UnityEngine;

public class PlayerEntity : AC_Entity
{
    [SerializeField] protected EC_Movement movementSO;
    [SerializeField] private EC_Camera cameraSO;
    [SerializeField] private EC_Rigidbody rigidbodySO;
    [SerializeField] private Def_Gun Gun;

    AC_Component[] components;

    private void Awake()
    {
        MassAssign();
        EventLinker();
    }
    
    public override void Start()
    {
        OnStartTick?.Invoke();
        movementSO.ComponentStart();
    }

    public override void Update()
    {
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
        // Connect movement events to camera and rigidbody
        movementSO.OnCameraMove += cameraSO.UpdateCameraTransform;
        movementSO.OnCameraMove += rigidbodySO.RotatePlayer;
        
        // Connect trigger events
        OnTriggerEnterTick += Gun.TriggerEnter;
        OnTriggerExitTick += Gun.TriggerExit;
    }

    public void MassAssign()
    {
        components = new AC_Component[]
        {
            cameraSO,
            rigidbodySO,
            movementSO,
        };

        foreach (AC_Component component in components)
        {
            component.entity = this;
            component.ComponentAwake();

            OnStartTick += component.ComponentStart;
            OnUpdateTick += component.ComponentUpdate;
            OnFixedUpdateTick += component.ComponentFixedUpdate;
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
        foreach (AC_Component component in components)
        {
            OnStartTick -= component.ComponentStart;
            OnUpdateTick -= component.ComponentUpdate;
            OnFixedUpdateTick -= component.ComponentFixedUpdate;
        }

        // Disconnect movement events
        movementSO.OnCameraMove -= cameraSO.UpdateCameraTransform;
        movementSO.OnCameraMove -= rigidbodySO.RotatePlayer;

        // Disconnect trigger events
        OnTriggerEnterTick -= Gun.TriggerEnter;
        OnTriggerExitTick -= Gun.TriggerExit;
    }
}
