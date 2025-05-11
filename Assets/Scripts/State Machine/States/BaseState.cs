using System;
using UnityEngine;

public abstract class BaseState: ScriptableObject
{
    [SerializeField] protected EC_Movement ctx;

    //idk if this needs to be [SerializeField] or not, it was to begin with but i dont see a use for it
    public StateListEnum next { get; protected set; }
    public BaseState(EC_Movement ctx)
    {

    }
    public abstract bool SwitchCondintion();
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();


    public virtual void LateUpdateState() { }

    public virtual void FixedUpdate() { }

    //public void SwitchState(BaseState next)
    //{
    //    //factory._currentState.ExitState();
    //    //factory._currentState = next;
    //    //factory._currentState.EnterState();
    //}

    public void prerequisites(EC_Movement ctx)
    {
        this.ctx = ctx;
    }

}
