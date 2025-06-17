using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Jump State", menuName = "Scriptable Object/State Machine/Jump State")]

public class SO_JumpState : AC_BaseState
{
    [SerializeField] float JumpPower;
    [SerializeField] float JumpTime;
    bool canExit;
    public SO_JumpState(EC_Movement ctx) : base(ctx)
    {
        canExit = false;
    }
    public override void EnterState()
    {
        ctx.EC_Rigidbody._CHECK_GRAVITY = false;
        //ctx.EC_Rigidbody.Jump(JumpPower);
    }

    public override void ExitState()
    {
        ctx.EC_Rigidbody._CHECK_GRAVITY = true;
    }

    public override bool SwitchCondintion()
    {
        return ctx.inputAccessSO.Jump() && ctx.IsGrounded;
    }

    public override bool CanExit() => canExit;

    private async Task JumpTask()
    {
        ctx.EC_Rigidbody.Jump(JumpPower);
        await Task.Delay((int)(JumpTime * 1000));

    }

    public override void UpdateState()
    {
    }

    public override async void FixedUpdate()
    {
        await JumpTask();
        canExit = true;
    }
}
