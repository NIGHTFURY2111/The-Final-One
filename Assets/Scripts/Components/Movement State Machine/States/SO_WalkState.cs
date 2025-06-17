using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Walk State", menuName = "Scriptable Object/State Machine/Walk State")]

public class SO_WalkState : AC_BaseState
{
    [SerializeField] float walkSpeed;
    public SO_WalkState(EC_Movement ctx) : base(ctx)
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
        return ctx.inputAccessSO.Movement() != Vector2.zero && ctx.IsGrounded;
    }

    public override void UpdateState()
    {
        //Debug.Log("Walking with input: " + ctx.inputAccessSO.Movement());
        ctx.MovePlayer(ctx.inputAccessSO.Movement() * walkSpeed);
    }
}
