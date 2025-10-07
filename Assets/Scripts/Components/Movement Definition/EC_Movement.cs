using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Movement", menuName = "Scriptable Object/Component/Player Movement")]
public class EC_Movement : AC_Component
{
    public override Enum_ComponentType componentType => Enum_ComponentType.Movement;
    [SerializeField] float LookSpeed = 0.8f;      

    [SerializeField] public SO_InputAccess inputAccessSO;
    [SerializeField] public EC_Rigidbody EC_Rigidbody;
    [SerializeField] public SO_detector detector;
    [SerializeReferenceDropdown]
    [SerializeReference] public StateManager stateManager;

    public Action<Vector2> OnCameraMove;
    public Action<PlayerMovementValues,bool> OnPlayerMove;
    public Action<bool> OnCrouch;


    public override void ComponentAwake()
    {
        stateManager.setPrerequisites();
    }


    public override void ComponentStart()
    {
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
    public override void ComponentDisable() { }
}
