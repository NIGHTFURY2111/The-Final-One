using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class stateholder
{
    [SerializeReferenceDropdown]
    [SerializeReference]
    public BaseState state;
    public StateListEnum stateEnum;
}

[Serializable]
public class StateManager
{
    [SerializeField] EC_Movement _context;
    [SerializeField] List<BaseState> stateholders = new();
    [SerializeField] StateListEnum initial;
    [SerializeField] public BaseState currentState { get; private set; }

    public void Update()
    {
        currentState.UpdateState();
        switchCheck(currentState.next);
    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate();
    }
    protected void switchCheck(StateListEnum next)
    {
        foreach (var holder in stateholders)
        {
            //if (holder.state == currentState)
            //{
            //    return;
            //}
            //Debug.Log(holder.name + "   "  + (next.HasFlag(holder.stateTypeEnum)) +"   "+holder.SwitchCondintion() );
            if (next.HasFlag(holder.stateTypeEnum) && holder.SwitchCondintion())
            {
                SwitchState(holder);
                return;
            }
        }
    }


    public void SwitchState(BaseState next)
    {
        currentState.ExitState();
        currentState = next;
        currentState.EnterState();
    }

    protected BaseState fetch(StateListEnum exitStates)
    {
        foreach (var holder in stateholders)
        {
            if (exitStates.HasFlag(holder.stateTypeEnum))
            {
                return holder;
            }
        }
        return null;
    }
    public void setPrerequisites()
    {
        currentState = fetch(initial);
    }

    //public BaseState currentState { get => currentState; set => currentState

}
