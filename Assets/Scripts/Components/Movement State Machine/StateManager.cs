using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class stateholder
{
    [SerializeReferenceDropdown]
    [SerializeReference]
    public AC_BaseState state;
    public Enum_StateList stateEnum;
}

[Serializable]
public class StateManager
{
    [SerializeField] EC_Movement _context;
    [SerializeField] List<AC_BaseState> stateholders = new();
    [SerializeField] Enum_StateList initial;
    [SerializeField] public AC_BaseState currentState { get; private set; }
    private Dictionary<Enum_StateList, AC_BaseState> _stateMap;

    private void BuildStateMap()
    {
        _stateMap = new Dictionary<Enum_StateList, AC_BaseState>();
        foreach (var state in stateholders)
        {
            //Debug.Log();
            if (!_stateMap.ContainsKey(state.stateTypeEnum))
                _stateMap.Add(state.stateTypeEnum, state);
        }
    }
    public void Update()
    {
        currentState.UpdateState();
        if (currentState.CanExit())
            switchCheck(currentState.next);
    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate();
    }
    protected void switchCheck(List<Enum_StateList> nextStates)
    {

        if (nextStates == null) return;
        foreach (var next in nextStates)
        {
            //Debug.Log("Checking state: " + _stateMap.TryGetValue(next, out var statess));
            if (_stateMap.TryGetValue(next, out var state) && state.SwitchCondintion())
            {
                SwitchState(state);
                return;
            }
        }
    }


    public void SwitchState(AC_BaseState next)
    {
        currentState.ExitState();
        currentState = next;
        currentState.EnterState();
        _context.UpdateGoalVel();
    }

    protected AC_BaseState fetch(Enum_StateList exitStates)
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
        BuildStateMap();
        currentState = fetch(initial);
    }

    
    //public BaseState currentState { get => currentState; set => currentState

}
