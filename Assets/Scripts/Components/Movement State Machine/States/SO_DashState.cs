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
        dashDirection = p_Rigidbody.DirectionRespectiveToPlayer(p_Input.Movement(),true);
        await DashTask(dashDirection);
    }

    async Task DashTask(Vector3 direction)
    {
        canExit = false;
        p_Rigidbody.setGravity(0);
        p_Rigidbody.MoveInSpecifiedDirection(dashDirection, p_Rigidbody.PlayerPlaneVel.magnitude+dashSpeed);
        await Task.Delay((int)(dashDuration *1000));
        p_Rigidbody.setGravity(ctx.EC_Rigidbody.GRAVITY);
        canExit = true;
    }

    public override void ExitState()
    {
        //p_Rigidbody.MoveInSpecifiedDirection(Vector3.zero, 0f);
        //p_Rigidbody.MoveInSpecifiedDirection(Vector3.zero, 0f);
    }

    public override bool SwitchCondintion()
    {
        return p_Input.Dash();
    }

    public override bool CanExit() => canExit;

    public override void UpdateState()
    {
    }
}
