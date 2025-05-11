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
        Debug.Log("entered Idle");
    }

    public override void ExitState()
    {
        Debug.Log("exited Idle");
    }

    public override bool SwitchCondintion()
    {
        return (!ctx.move.IsInProgress());
    }

    public override void UpdateState()
    {
        //Debug.Log(ctx.move.IsPressed());

    }
}
