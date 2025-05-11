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
    [SerializeField] List<stateholder> stateholders = new();
    [SerializeField] StateListEnum initial;
    [SerializeField] BaseState currentState;

    public void Update()
    {
        currentState.UpdateState();
        switchCheck(currentState.next);
        Debug.Log(currentState);
    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate();
    }
    protected void switchCheck(StateListEnum next)
    {

        foreach (var holder in stateholders)
        {
            if (next.HasFlag(holder.stateEnum) && holder.state.SwitchCondintion())
            {
                SwitchState(holder.state);
                return;
            }
        }

    }
    protected BaseState fetch(StateListEnum exitStates)
    {
        foreach (var holder in stateholders)
        {
            if (exitStates.HasFlag(holder.stateEnum))
            {
                return holder.state;
            }
        }
        return null;
    }


    public void SwitchState(BaseState next)
    {
        currentState.ExitState();
        currentState = next;
        currentState.EnterState();
    }

    public void setPrerequisites()
    {
        foreach (stateholder holder in stateholders)
        {
            holder.state.prerequisites(_context);
        }
        currentState = fetch(initial);
    }

    public BaseState _currentState { get => currentState; set => currentState = value; }

}
