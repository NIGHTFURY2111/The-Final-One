using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall Jump", menuName = "Scriptable Object/State Machine/Wall Jump")]
public class SO_WallJump : AC_BaseState
{
    public float WallJumpHeight, WallJumpAwayForce;
    public SO_WallJump(EC_Movement ctx) : base(ctx)
    {
    }

    public override void EnterState()
    {
        Vector3 jumpDir = p_Rigidbody.wallHit.normal * WallJumpAwayForce +p_Rigidbody.PlayerUp * WallJumpHeight;
        p_Rigidbody.ApplyForce(jumpDir, ForceMode.Impulse);
    }

    public override void ExitState()
    {
    }

    public override bool SwitchCondintion()
    {
        return p_Rigidbody.isWall && p_Input.Jump();
    }

    public override void UpdateState()
    {
    }
}
