using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Jump State", menuName = "Scriptable Object/State Machine/Jump State")]

public class SO_JumpState : AC_BaseState
{
    [SerializeField] AnimationCurve JumpCurve;
    [SerializeField] float JumpPower;
    [SerializeField] ForceMode forceType;
    float JumpTime;
    bool canExit;
    public SO_JumpState(EC_Movement ctx) : base(ctx)
    {
    }
    public override void EnterState()
    {
        canExit = false;
        p_Rigidbody._CHECK_GRAVITY = false;
        JumpTime = JumpCurve.keys[^1].time;
        p_Rigidbody.MoveInSpecifiedDirection(p_Rigidbody.PlayerPlaneVel,1f);
        //Debug.Log(p_Rigidbody.PlayerVelocity);
    }

    public override void ExitState()
    {
        p_Rigidbody._CHECK_GRAVITY = true;
    }

    public override bool SwitchCondintion()
    {
        return p_Input.Jump() && ctx.IsGrounded;
    }

    public override bool CanExit() => canExit;

    private async Task JumpTask()
    {
        p_Rigidbody.Jump(JumpCurve,JumpPower,forceType);
        await Task.Delay((int)(JumpTime * 1000));
        canExit = true;
    }

    public override void UpdateState()
    {
    }

    public override async void FixedUpdate()
    {
        await JumpTask();
    }
}
