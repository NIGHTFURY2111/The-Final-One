using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Player Movement", menuName = "Scriptable Object/Component/Player Movement")]
public class EC_Movement : AC_Component
{
    public override ComponentType componentType => ComponentType.Movement;


    PlayerInputAction PlayerIA;
    public InputAction move;
    public InputAction jump;
    public InputAction dash;
    public InputAction camera;
    InputAction grappleAction;
    InputAction grappleHoldAction;
    InputAction hatThrowStaticAction;
    InputAction hatThrowDynamicAction;



    public MonoBehaviour entity;

    [SerializeReferenceDropdown]
    [SerializeReference] public StateManager stateManager;


    public void ComponentAwake()
    {
        stateManager.setPrerequisites();
        Debug.Log("StateManager set");

        move = PlayerIA.Player.Movement;
        jump = PlayerIA.Player.Jump;
        dash = PlayerIA.Player.Dash;
        camera = PlayerIA.Player.Camera;
    }

    public void OnEnable()
    {
        move.Enable();
        jump.Enable();
        dash.Enable();
        camera.Enable();
    }

    public void OnDisable()
    {
        move.Disable();
        jump.Disable();
        dash.Disable();
        camera.Disable();
    }



    public void Start()
    {

    }

    public void Update()
    {
        stateManager.Update();
        //Debug.Log(playerinput.ListInputs());
    }
}
