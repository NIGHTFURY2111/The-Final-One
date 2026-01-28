using UnityEngine;

[CreateAssetMenu(fileName = "Wall Jump", menuName = "Scriptable Object/State Machine/Wall Jump")]
public class SO_WallJump : AC_BaseState
{
    public float WallJumpHeight, WallJumpAwayForce;
    public ForceMode forceMode;
    public override void EnterState()
    {
        //Vector3 jumpDir =  ;
        p_Rigidbody.OverrideVelocity(p_Rigidbody.PlayerPlaneVel,1f);

        p_Rigidbody.ApplyForce(p_Rigidbody.PlayerUp * WallJumpHeight, forceMode);
        p_Rigidbody.ApplyForce(p_Detector.wallHit.normal * WallJumpAwayForce, forceMode);
    }

    public override void ExitState()
    {
    }

    public override bool SwitchCondintion()
    {
        return p_Detector.isWall && p_Input.Jump();
    }

    public override void UpdateState()
    {
    }
}
