using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fall State", menuName = "Scriptable Object/State Machine/Fall State")]

public class SO_FallState : AC_BaseState
{
    public float AirSpeed;
    public float MaxAdditionVel;
    float currentVel;


    public SO_FallState(EC_Movement ctx) : base(ctx)
    {
        currentVel = ctx.EC_Rigidbody.PlayerVelocity.magnitude;
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

    public override void FixedUpdate()
    {
        ctx.EC_Rigidbody.AirMovement(ctx.inputAccessSO.Movement() * AirSpeed, currentVel, MaxAdditionVel);
    }
}
