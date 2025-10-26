using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Entity : AC_Entity
{
    [SerializeField] private float PathFindingTickDelay;
                     private float nextTickTime = 0f;
    [SerializeField] private List<AC_Component> Components;
    [SerializeField] private Dictionary<Enum_ComponentType, AC_Component> ComponentDict = new();

    public Action enemyPathFindingTick;

    public Enum_Tag targetTags;
    [HideInInspector]public NavMeshAgent navMeshAgent;
    public float shootDistance;

    private void Awake()
    {
        BuildDictionary();
        MassAssign();
        EventSubscribe();
        // Only invoke OnAwakeTick after everything is properly set up
        OnAwakeTick?.Invoke();
    }

    void BuildDictionary()
    {
        ComponentDict.Clear();

        foreach (AC_Component compCopy in Components)
        {
            var comp = Instantiate(compCopy);

            if (comp == null) continue;
            if (!ComponentDict.ContainsKey(comp.componentType))
            {
                ComponentDict.Add(comp.componentType, comp);
            }
        }
    }


    public override void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        shootDistance = navMeshAgent.stoppingDistance;
        OnStartTick?.Invoke();
    }

    public override void Update()
    {
        OnUpdateTick?.Invoke();
        pathFindingtimer();
    }

    void pathFindingtimer()
    {
        if (Time.deltaTime > nextTickTime)
        {
            enemyPathFindingTick?.Invoke();
            nextTickTime = Time.deltaTime + PathFindingTickDelay;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterTick?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitTick?.Invoke(other);
    }

    public override void FixedUpdate()
    {
        OnFixedUpdateTick?.Invoke();
    }


    private void OnDisable()
    {
        // Call component disable first
        OnDisableTick?.Invoke();

        // Then unsubscribe everything to prevent memory leaks
        UnsubscribeAllEvents();
    }

    public override void Ondeath()
    {
        OnDeathTrigger.Invoke();
    }
    public override void EventSubscribe()
    {
    }
    public override void EventUnsubscribe()
    {
    }

    public void MassAssign()
    {
        foreach (AC_Component component in ComponentDict.Values)
        {
            if (component == null) continue;

            component.entity = this;

            // Subscribe to events - these should only happen once per component
            TryEvent(() => OnAwakeTick += component.ComponentAwake, this, component);
            TryEvent(() => OnDisableTick += component.ComponentDisable, this, component);
            TryEvent(() => OnStartTick += component.ComponentStart, this, component);
            TryEvent(() => OnUpdateTick += component.ComponentUpdate, this, component);
            TryEvent(() => OnFixedUpdateTick += component.ComponentFixedUpdate, this, component);
        }
    }

    private void UnsubscribeAllEvents()
    {
        // Unsubscribe component events
        foreach (AC_Component component in ComponentDict.Values)
        {
            if (component == null) continue;

            TryEvent(() => OnAwakeTick -= component.ComponentAwake, this, component);
            TryEvent(() => OnDisableTick -= component.ComponentDisable, this, component);
            TryEvent(() => OnStartTick -= component.ComponentStart, this, component);
            TryEvent(() => OnUpdateTick -= component.ComponentUpdate, this, component);
            TryEvent(() => OnFixedUpdateTick -= component.ComponentFixedUpdate, this, component);
        }

        EventUnsubscribe();
    }

    AC_Component getComponenet(Enum_ComponentType type)
    {
        ComponentDict.TryGetValue(type, out AC_Component comp);
        return comp;
    }

    public EC_Movement movementSO => getComponenet(Enum_ComponentType.Movement) as EC_Movement;
}
