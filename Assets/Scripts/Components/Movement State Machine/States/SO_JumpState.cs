using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Jump State", menuName = "Scriptable Object/State Machine/Jump State")]

public class SO_JumpState : AC_BaseState
{
    [SerializeField] AnimationCurve JumpCurve;
    [SerializeField] float JumpHeight;
    [SerializeField] float CoyoteTiming;
    [SerializeField] ForceMode forceType;
    float JumpTime;
    bool canExit;
    bool canJump;
    float t ;
    Vector3 j;
    public SO_JumpState(EC_Movement ctx) : base(ctx)
    {
    }
    public override void EnterState()
    {
        canExit = false;
        canJump = false;
        p_Rigidbody._CHECK_GRAVITY = false;
        JumpTime = JumpCurve.keys[^1].time;
        
        j = p_Rigidbody.PlayerPlaneVel;
        p_Rigidbody.OverrideVelocity(p_Rigidbody.PlayerPlaneVel,1f);
        
        //Debug.Log(p_Rigidbody.PlayerVelocity);
        t = Time.time;
    }

    public override void ExitState()
    {
        p_Rigidbody._CHECK_GRAVITY = true;
        //Debug.Log($"Jump Time: {Time.time - t}\t vel at start {j}\t max height {p_Rigidbody.PlayerTransform.position}");
    }

    public override bool SwitchCondintion()
    {
        canJump = ctx.IsGrounded? true : canJump;

        return  p_Input.Jump()  && 
                canJump         &&
                    (ctx.IsGrounded || 
                    (Time.time - p_Detector.lastGrounded <CoyoteTiming)
                    );
    }

    public override bool CanExit() => canExit;

    private async Task JumpTask()
    {
        p_Rigidbody.Jump(JumpCurve,JumpHeight,forceType);
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
