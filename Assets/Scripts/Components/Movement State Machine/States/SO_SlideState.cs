using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Slide State", menuName = "Scriptable Object/State Machine/Slide State")]

public class SO_SlideState : AC_BaseState
{
    [SerializeField] float SlideSpeed;
    Vector3 SlideDirection;
    public SO_SlideState(EC_Movement ctx) : base(ctx)
    {
    }
    public override void EnterState()
    {
        SlideDirection = ctx.EC_Rigidbody.DirectionRespectiveToPlayer(ctx.inputAccessSO.Movement(), true).normalized * SlideSpeed;
    }

    public override void ExitState()
    {
    }

    public override bool SwitchCondintion()
    {
        return ctx.inputAccessSO.Slide() && ctx.IsGrounded;
    }

    public override void UpdateState()
    {
        ctx.EC_Rigidbody.MoveInSpecifiedDirection(SlideDirection);
    }

    public override bool CanExit()
    {
        //Debug.Log("Can Exit Slide State: " + (!ctx.inputAccessSO.Slide() || ctx.inputAccessSO.Jump()));
        ctx.inputAccessSO.Slide(out InputAction action);
        return action.WasReleasedThisFrame() || ctx.inputAccessSO.Jump();
    }
}
