using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Walk State", menuName = "Scriptable Object/State Machine/Walk State")]

public class WalkState : BaseState
{
    public WalkState(EC_Movement ctx) : base(ctx)
    {
    }

    public override void EnterState()
    {
        Debug.Log("entered Walk");  
    }

    public override void ExitState()
    {
        Debug.Log("exited Walk");
    }

    public override bool SwitchCondintion()
    {
        return ctx.move.WasPressedThisFrame();
    }

    public override void UpdateState()
    {
        Debug.Log("Walking");
    }
}
