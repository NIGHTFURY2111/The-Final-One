using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fall State", menuName = "Scriptable Object/State Machine/Fall State")]

public class SO_FallState : AC_BaseState
{
    [SerializeField]float AirMoveFactor, overrideFactor;


    public SO_FallState(EC_Movement ctx) : base(ctx)
    {
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
        p_Rigidbody.AirMovement(ctx.inputAccessSO.Movement(), AirMoveFactor, overrideFactor);
    }

    //public override void FixedUpdate()
    //{
    //}
}
