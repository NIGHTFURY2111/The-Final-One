using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle State", menuName = "Scriptable Object/State Machine/Idle State")]

public class SO_IdleState : AC_BaseState
{
    public SO_IdleState(EC_Movement ctx) : base(ctx)
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
        return p_Input.Movement() == Vector2.zero && ctx.IsGrounded;
    }

    public override void UpdateState()
    {
        //Debug.Log(ctx.move.IsPressed());
        p_Rigidbody.Move(Vector3.zero,0f,0f);
    }
}
