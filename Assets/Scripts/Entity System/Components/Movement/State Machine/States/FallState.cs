using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fall State", menuName = "Scriptable Object/State Machine/Fall State")]

public class FallState : BaseState
{
    public FallState(EC_Movement ctx) : base(ctx)
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
        return !ctx.IsGrounded;
    }

    public override void UpdateState()
    {
    }
}
