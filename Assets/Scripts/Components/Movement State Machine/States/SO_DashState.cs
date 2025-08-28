using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash State", menuName = "Scriptable Object/State Machine/Dash State")]
public class SO_DashState : AC_BaseState
{
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 0.5f;
    
    private bool canExit;
    private Vector3 dashDirection;

    public SO_DashState(EC_Movement ctx) : base(ctx)
    {
    }

    public override async void EnterState()
    {
        dashDirection = p_Rigidbody.DirectionRespectiveToPlayer(p_Input.Movement(), true);
        await DashTask(dashDirection);
    }

    async Task DashTask(Vector3 direction)
    {
        canExit = false;
        p_Rigidbody.setGravity(0);
        p_Rigidbody.MoveInSpecifiedDirection(dashDirection, p_Rigidbody.PlayerPlaneVel.magnitude + dashSpeed);

        // FOV will be handled automatically by the centralized velocity-based system
        // No need for manual FOV changes since dash increases velocity
        
        await Task.Delay((int)(dashDuration * 1000));

        p_Rigidbody.setGravity(ctx.EC_Rigidbody.GRAVITY);
        canExit = true;
    }

    public override void ExitState()
    {
        // Clean exit - no FOV cleanup needed since we're not manually controlling it
    }

    public override bool SwitchCondintion()
    {
        return p_Input.Dash();
    }

    public override bool CanExit() => canExit;

    public override void UpdateState()
    {
        // Nothing to update
    }
}
