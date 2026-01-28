using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AC_BaseState: ScriptableObject
{
    protected EC_Movement ctx;

    protected EC_Rigidbody p_Rigidbody => ctx.EC_Rigidbody;
    protected SO_detector p_Detector=> ctx.detector;
    protected SO_InputAccess p_Input => ctx.inputAccessSO;

    //idk if this needs to be [SerializeField] or not, it was to begin with but i dont see a use for it
    [SerializeReferenceDropdown]
    [SerializeField] public Enum_StateList stateTypeEnum;
    [SerializeField] public List<Enum_StateList> next;
    public void setupCtx(EC_Movement context)
    {
        //Debug.Log(context.EC_Rigidbody._RB);
        ctx = context;
        //Debug.Log(ctx.EC_Rigidbody._RB);
        //Debug.Log(this.name);
    }
    public abstract bool SwitchCondintion();
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();


    public virtual void LateUpdateState() { }

    public virtual void FixedUpdate() { }
    public virtual bool CanExit()
    {
        return true; // By default, states allow exit
    }

    //public void SwitchState(BaseState next)
    //{
    //    //factory._currentState.ExitState();
    //    //factory._currentState = next;
    //    //factory._currentState.EnterState();
    //}
    public void Destroy()
    {
        Destroy(this);
    }
}
