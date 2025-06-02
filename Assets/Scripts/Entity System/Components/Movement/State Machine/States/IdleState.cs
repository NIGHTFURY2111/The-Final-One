using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle State", menuName = "Scriptable Object/State Machine/Idle State")]

public class IdleState : BaseState
{
    public IdleState(EC_Movement ctx) : base(ctx)
    {
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override bool SwitchCondintion()
    {
        return ctx.inputAccessSO.Movement() == Vector2.zero && ctx.IsGrounded;
    }

    public override void UpdateState()
    {
        //Debug.Log(ctx.move.IsPressed());

    }
}
