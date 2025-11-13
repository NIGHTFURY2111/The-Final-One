using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall Enter", menuName = "Scriptable Object/State Machine/Wall Enter")]

public class SO_WallEnter : AC_BaseState
{
    [SerializeField] PlayerMovementValues FallValue;


    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override bool SwitchCondintion()
    {
     return /*!ctx.IsGrounded && */p_Detector.isWall;
    }

    public override void UpdateState()
    {
    }
    public override void FixedUpdate()
    {
        ctx.MovePlayer(FallValue.UpdateDirection(p_Input.Movement()));
    }

}
