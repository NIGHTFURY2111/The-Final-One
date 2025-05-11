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
    [SerializeReferenceDropdown]
    [SerializeReference] public StateManager stateManager;

    public Action<Vector2> OnCameraMove;

    public override void ComponentAwake()
    {
        stateManager.setPrerequisites();
    }


    public override void ComponentStart()
    {
    }

    public void ProcessMouseInput()
    {

        OnCameraMove?.Invoke(cameraInput() * LookSpeed);
        Debug.Log(cameraInput());

    }


    Vector2 cameraInput()
    {
        Vector2 outp = Vector2.zero;
        inputAccessSO.Camera(out outp); 
        return outp;
    }

    public override void ComponentUpdate()
    {
        stateManager.Update();
        ProcessMouseInput();
    }
}
