using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash State", menuName = "Scriptable Object/State Machine/Dash State")]

public class SO_DashState : AC_BaseState
{

    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    bool canExit;
    Vector3 dashDirection;
    public SO_DashState(EC_Movement ctx) : base(ctx)
    {
    }

    public override async void EnterState()
    {
        dashDirection = ctx.EC_Rigidbody.DirectionRespectiveToPlayer(ctx.inputAccessSO.Movement(),true);
        await DashCoroutine(dashDirection);
    }

    async Task DashCoroutine(Vector3 direction)
    {
        canExit = false;
        ctx.EC_Rigidbody.setGravity(0);
        ctx.EC_Rigidbody.MoveInSpecifiedDirection(dashDirection * dashSpeed);
        await Task.Delay((int)(dashDuration *1000));
        ctx.EC_Rigidbody.setGravity(ctx.EC_Rigidbody.GRAVITY);
        canExit = true;
    }

    public override void ExitState()
    {
        ctx.EC_Rigidbody.Move(Vector2.zero);
    }

    public override bool SwitchCondintion()
    {
        return ctx.inputAccessSO.Dash();
    }

    public override bool CanExit() => canExit;

    public override void UpdateState()
    {
    }
}
