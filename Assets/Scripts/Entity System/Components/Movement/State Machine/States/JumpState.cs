using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jump State", menuName = "Scriptable Object/State Machine/Jump State")]

public class JumpState : BaseState
{
    [SerializeField] float JumpPower;
    public JumpState(EC_Movement ctx) : base(ctx)
    {
    }
    public override void EnterState()
    {
        ctx.EC_Rigidbody.Jump(JumpPower);
    }

    public override void ExitState()
    {
    }

    public override bool SwitchCondintion()
    {
        return ctx.inputAccessSO.Jump() && ctx.IsGrounded;
    }

    public override void UpdateState()
    {
    }

}
