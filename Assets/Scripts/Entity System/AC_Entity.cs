using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AC_Entity : MonoBehaviour
{
    public Action OnStartTick;
    public Action OnUpdateTick;
    public Action OnFixedUpdateTick;
    public Action<Collider> OnTriggerEnterTick;
    public Action<Collider> OnTriggerExitsTick;

    //TODO: public HealthSystemSO;
    //TODO: public MovementStateMachineSO;
    //TODO: public AnimationSystemSO;
    //TODO: public AudioSystemSO;

    public abstract void Start();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void EventLinker();
}
