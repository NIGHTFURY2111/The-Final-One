using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall Enter", menuName = "Scriptable Object/State Machine/Wall Enter")]

public class SO_WallEnter : AC_BaseState
{
    [SerializeField] PlayerMovementValues FallValue;

    public SO_WallEnter(EC_Movement ctx) : base(ctx)
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
     return /*!ctx.IsGrounded && */p_Rigidbody.isWall;
    }

    public override void UpdateState()
    {
    }
    public override void FixedUpdate()
    {
        p_Rigidbody.Move(FallValue.UpdateDirection(p_Input.Movement()));
    }

}
