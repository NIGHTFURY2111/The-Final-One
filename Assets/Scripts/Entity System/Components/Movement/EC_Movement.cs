using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Player Movement", menuName = "Scriptable Object/Component/Player Movement")]
public class EC_Movement : AC_Component
{
    public override ComponentType componentType => ComponentType.Movement;
    [SerializeField] float LookSpeed = 0.8f;      

    [SerializeField] public InputAccessSO inputAccessSO;
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
    }
    public override void ComponentUpdate()
    {
        stateManager.Update();
        ProcessMouseInput();
        Debug.Log(EC_Rigidbody.isgrounded);
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
