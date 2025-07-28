using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall Slide Down", menuName = "Scriptable Object/State Machine/Wall Slide Down")]

public class SO_WallSlideDown : AC_BaseState
{
    //[SerializeField] PlayerMovementValues FallValue;
    [SerializeField] float SlideGravityFactor, MinimumFallingVelocity;

    public SO_WallSlideDown(EC_Movement ctx) : base(ctx)
    {
    }

    public override void EnterState()
    {
        p_Rigidbody.setGravity(p_Rigidbody.GRAVITY*SlideGravityFactor);
    }

    public override void ExitState()
    {

        p_Rigidbody.setGravity(p_Rigidbody.GRAVITY);
    }

    public override bool SwitchCondintion()
    {
        return p_Rigidbody.PlayerDownVelocity> MinimumFallingVelocity;
    }

    public override bool CanExit()
    {
        return !p_Rigidbody.isWall || p_Rigidbody.isGrounded;
    }
    public override void UpdateState()
    {
    }

    public override void FixedUpdate()
    {
        //p_Rigidbody.Move(FallValue.UpdateDirection(p_Input.Movement()));
    }
}
