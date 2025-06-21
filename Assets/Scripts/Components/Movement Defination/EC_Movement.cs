using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Movement", menuName = "Scriptable Object/Component/Player Movement")]
public class EC_Movement : AC_Component
{
    public override Enum_ComponentType componentType => Enum_ComponentType.Movement;
    [SerializeField] float LookSpeed = 0.8f;      

    [SerializeField] public SO_InputAccess inputAccessSO;
    [SerializeField] public EC_Rigidbody EC_Rigidbody;
    [SerializeReferenceDropdown]
    [SerializeReference] public StateManager stateManager;

    public Action<Vector2> OnCameraMove;
    public Action<Vector2> OnPlayerMove;


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
        //Debug.Log(inputAccessSO.Dash());
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
        Vector2 outp = Vector2.zero;
        inputAccessSO.Camera(out outp); 
        return outp;
    }

    public void MovePlayer(Vector2 moveVector) => OnPlayerMove?.Invoke(moveVector);
    public bool IsGrounded => EC_Rigidbody.isgrounded;
}
