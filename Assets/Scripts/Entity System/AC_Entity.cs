using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public abstract class AC_Entity : MonoBehaviour
{
    [HideInInspector]public UnityEvent OnStartTick;
    [HideInInspector]public UnityEvent OnAwakeTick;
    [HideInInspector]public UnityEvent OnDisableTick;
    [HideInInspector]public UnityEvent OnDestroyTick;
    [HideInInspector]public UnityEvent OnUpdateTick;
    [HideInInspector]public UnityEvent OnFixedUpdateTick;
    [HideInInspector]public UnityEvent<Collider> OnTriggerEnterTick;
    [HideInInspector]public UnityEvent<Collider> OnTriggerExitTick;
    [HideInInspector]public UnityEvent OnDeathTrigger;

    //TODO: public HealthSystemSO;
    //TODO: public MovementStateMachineSO;
    //TODO: public AnimationSystemSO;
    //TODO: public AudioSystemSO;

    public abstract void Start();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void EventSubscribe();
    public abstract void EventUnsubscribe();
    public abstract void Ondeath();

    public static void TryEvent(Action subscribeAction, params object[] sourceObjects)
    {
        if (sourceObjects.All(obj => obj != null))
        {
            
            subscribeAction?.Invoke();
        }
    }


}
