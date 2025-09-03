using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public abstract class AC_Entity : MonoBehaviour
{
    public Action OnStartTick;
    public Action OnUpdateTick;
    public Action OnFixedUpdateTick;
    public Action<Collider> OnTriggerEnterTick;
    public Action<Collider> OnTriggerExitTick;
    public UnityEvent OnDeathTrigger;

    //TODO: public HealthSystemSO;
    //TODO: public MovementStateMachineSO;
    //TODO: public AnimationSystemSO;
    //TODO: public AudioSystemSO;

    public abstract void Start();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void EventLinker();
    public abstract void Ondeath();

    public static void TryEvent(Action subscribeAction, params object[] sourceObjects)
    {
        if (sourceObjects.All(obj => obj != null))
        {
            subscribeAction?.Invoke();
        }
    }


}
