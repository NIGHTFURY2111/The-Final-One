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
        ST_debug.displayString = movementSO.stateManager.currentState.name + "\n" + movementSO.inputAccessSO.ListInputs();
        //playerNameText.text = movementSO.stateManager.currentState.name +"\n"+ movementSO.inputAccessSO.ListInputs();
        //movementSO.ComponentUpdate();
        OnUpdateTick?.Invoke();
    }

    public override void EventLinker()
    {
        movementSO.OnCameraMove += cameraSO.UpdateCameraTransform;
        movementSO.OnCameraMove += rigidbodySO.RotatePlayer;
        movementSO.OnPlayerMove += rigidbodySO.Move;
        OnTriggerEnterTick += Gun.TriggerEnter;
        OnTriggerExitsTick += Gun.TriggerExit;


    }

    public void MassAssign()
    {
        components = new AC_Component[]
        {
            cameraSO,
            rigidbodySO,
            movementSO
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
        OnTriggerExitsTick?.Invoke(other);
    }

    public override void FixedUpdate()
    {
        OnFixedUpdateTick?.Invoke();
    }
}
