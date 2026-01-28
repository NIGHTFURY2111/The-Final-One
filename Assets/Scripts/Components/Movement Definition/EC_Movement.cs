using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Player Movement", menuName = "Scriptable Object/Component/Player Movement")]
public class EC_Movement : AC_Component<EC_Movement>
{
    public override Enum_ComponentType componentType => Enum_ComponentType.Movement;
    [SerializeField] float LookSpeed = 0.8f;      

    [HideInInspector] public SO_InputAccess inputAccessSO;
    [HideInInspector] public EC_Rigidbody EC_Rigidbody;
    [HideInInspector] public SO_detector detector;
    [SerializeReferenceDropdown]
    [SerializeReference] public StateManager stateManager;
    
    [HideInInspector]public UnityEvent<Vector2> OnCameraMove;
    [HideInInspector]public UnityEvent<PlayerMovementValues,bool> OnPlayerMove;
    [HideInInspector] public UnityEvent<bool> OnCrouch;

    private Player_Entity Player_Entity => entity as Player_Entity;

    public override void ComponentAwake()
    {
        inputAccessSO = Player_Entity.inputSO;
        EC_Rigidbody = Player_Entity.rigidbodySO;
        detector = Player_Entity.detectorSO;
        stateManager.setPrerequisites(this);
    }


    public override void ComponentStart()
    {
        stateManager.giveCtx(this);
        //Debug.Log(stateManager.currentState.ctx.entity.name);
        stateManager.currentState.EnterState();
    }
    public override void ComponentUpdate()
    {
        stateManager.Update();
        ProcessMouseInput();
        //Debug.Log(detector.isGrounded);
    }

    public override void ComponentFixedUpdate()
    {
        stateManager.FixedUpdate();
    }


    public void ProcessMouseInput()
    {
        OnCameraMove?.Invoke(cameraInput() * LookSpeed);
    }


    Vector2 cameraInput()
    {
        inputAccessSO.Camera(out Vector2 outp); 
        return outp;
    }


    public void MovePlayer(PlayerMovementValues PMV,bool ChangeInputToLocalSpace = true) => OnPlayerMove?.Invoke(PMV, ChangeInputToLocalSpace);
    public void UpdateGoalVel() => EC_Rigidbody.UpdateGoalVel();
    public bool IsGrounded => detector.isGrounded;

    public void crouchCamera(bool iscrouching) => OnCrouch?.Invoke(iscrouching);
    public override void ComponentDisable() 
    {
        stateManager.destroystates();
        Destroy(this);
    }
}
