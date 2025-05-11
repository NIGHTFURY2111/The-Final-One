using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Entity : MonoBehaviour
{

    [SerializeField] protected EC_Movement movementSO;
    //TODO: public HealthSystemSO;
    //TODO: public MovementStateMachineSO;
    //TODO: public AnimationSystemSO;
    //TODO: public AudioSystemSO;

    public abstract void Start();
    public abstract void Update();
    public abstract void EventLinker();
}
