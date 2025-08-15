using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Slide State", menuName = "Scriptable Object/State Machine/Slide State")]

public class SO_SlideState : AC_BaseState
{
    [SerializeField] PlayerMovementValues slideValues;
    Vector3 SlideDirection;
    public SO_SlideState(EC_Movement ctx) : base(ctx)
    {
    }
    public override void EnterState()
    {
        slideValues.UpdateDirection(p_Rigidbody.DirectionRespectiveToPlayer( p_Input.Movement(), true).normalized);
    }
    public override void UpdateState()
    {
        p_Rigidbody.Move(slideValues, false);
    }

    public override void ExitState()
    {
    }

    public override bool SwitchCondintion()
    {
        return p_Input.Slide() && ctx.IsGrounded;
    }


    public override bool CanExit()
    {
        //Debug.Log("Can Exit Slide State: " + (!ctx.inputAccessSO.Slide() || ctx.inputAccessSO.Jump()));
        p_Input.Slide(out InputAction action);
        return action.WasReleasedThisFrame() || p_Input.Jump();
    }
}
