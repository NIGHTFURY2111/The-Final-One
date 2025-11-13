using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Walk State", menuName = "Scriptable Object/State Machine/Walk State")]

public class SO_WalkState : AC_BaseState
{
    [SerializeField] PlayerMovementValues WalkValue;

    [SerializeField] Headbob_Effect headbob;

    
    public override void EnterState()
    {
    }

    public override void ExitState()
    {
        headbob.OnHeadbobStop?.Invoke();
    }

    public override bool SwitchCondintion()
    {
        return !p_Input.Movement().Equals(Vector2.zero) && ctx.IsGrounded;
    }

    public override void UpdateState()
    {
        //Debug.Log("Walking with input: " + ctx.inputAccessSO.Movement());
        headbob.OnHeadbobUpdate?.Invoke();
    }

    public override void FixedUpdate()
    {
        ctx.MovePlayer(WalkValue.UpdateDirection(p_Input.Movement()));
    }
}
