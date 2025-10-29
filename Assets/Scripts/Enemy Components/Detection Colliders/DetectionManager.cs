using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All possible States that the Enemy can be in based on detection of target Entities
/// </summary>
public enum Enum_DetectionState
{//all possible states the enemy can be in based on detection of target entities
    Idle,
    Alerted,
    Chasing,
}


[CreateAssetMenu(fileName = "Enemy Detection Manager", menuName = "Scriptable Object/Component/Enemy/Enemy Detection Manager")]
public class DetectionManager : AC_Component
{
    [SerializeField] private bool useNearestTarget = true; // Toggle in inspector
    [SerializeField] private float alertedForgetTime = 3f; // Time to forget when Alerted
    [SerializeField] private float chasingDegradeTime = 5f; // Time to downgrade from Chasing to Alerted

    private float stateTimer = 0f; // Timer for current state
    private bool isInDegradationMode = false; // Are we in a degrading state?
    public override Enum_ComponentType componentType => Enum_ComponentType.Detector;
    Dictionary<Enum_DetectionColliderType, DetectionCollider> DetectorDictionary = new();
    Dictionary<Enum_DetectionColliderType, DetectedGameObjectList> ColliderListsDictionary = new();
    public Action<Enum_DetectionState, Collider> OnStateChanged;
    private Collider targetedCollider;
    //fire an event whenever the value of currentstate is changed
    private Enum_DetectionState _currentState;
    public Enum_DetectionState CurrentState { get => _currentState;
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnStateChanged?.Invoke(_currentState, targetedCollider);
            }
        }
    }



    public override void ComponentAwake() { }
    public override void ComponentDisable() 
    {
        foreach (DetectionCollider col in DetectorDictionary.Values)
        {
            col.OnColliderUpdate -= HandleDetectorUpdate;
        }

    }
    public override void ComponentStart()
    {
        BuildDictionary();
        AssignTagsAndSubscribe();
        CurrentState = Enum_DetectionState.Idle;
    }

    public override void ComponentUpdate()
    {
        UpdateStateTimer(); // Update timer for degradation
        DetermineState();
        Debug.Log($"{CurrentState} - Timer: {stateTimer:F2}");
    }
    void BuildDictionary()
    {
        DetectorDictionary.Clear();
        ColliderListsDictionary.Clear();

        List<DetectionCollider> colliders = new();
        entity.gameObject.GetComponentsInChildren<DetectionCollider>(colliders);
        foreach (DetectionCollider col in colliders)
        {
            if (col.detectionCollider == null) continue;

            DetectorDictionary.Add(col.detectionColliderType, col);
            ColliderListsDictionary.Add(col.detectionColliderType, col.triggerList);

        }
    }
    private void AssignTagsAndSubscribe()
    {
        Enum_Tag tags = ((Enemy_Entity)entity).targetTags;
        foreach (DetectionCollider col in DetectorDictionary.Values)
        {
            col.SetDetectionTags(tags);
            col.OnColliderUpdate += HandleDetectorUpdate;
        }
    }

    void HandleDetectorUpdate(Enum_DetectionColliderType type)
    {
        DetermineState();
    }

    /// <summary>
    /// Determines the current state based on priority hierarchy of all detection colliders
    /// Handles edge cases for state transitions
    /// </summary>
    /// <summary>
    /// Determines the current state based on priority hierarchy of all detection colliders
    /// Handles edge cases for state transitions and time-based degradation
    /// </summary>
    void DetermineState()
    {
        bool inVisionCone = HasTargetsInCollider(Enum_DetectionColliderType.Vision_Cone);
        bool inCloseRange = HasTargetsInCollider(Enum_DetectionColliderType.Close_Range);
        bool inChaseZone = HasTargetsInCollider(Enum_DetectionColliderType.Chase_Zone);

        // Priority 1: Vision Cone - Always means Chasing
        if (inVisionCone)
        {
            targetedCollider = SelectTarget(Enum_DetectionColliderType.Vision_Cone);
            CurrentState = Enum_DetectionState.Chasing;
            ResetStateTimer(); // Reset timer, no degradation
            return;
        }

        // Priority 2: Close Range while already Chasing
        // (Target got behind enemy but is still close - keep chasing)
        if (inCloseRange && CurrentState == Enum_DetectionState.Chasing)
        {
            targetedCollider = SelectTarget(Enum_DetectionColliderType.Close_Range);
            CurrentState = Enum_DetectionState.Chasing;
            ResetStateTimer(); // Reset timer, no degradation
            return;
        }

        // Priority 3: Close Range while not chasing - Alert
        if (inCloseRange)
        {
            targetedCollider = SelectTarget(Enum_DetectionColliderType.Close_Range);
            CurrentState = Enum_DetectionState.Alerted;
            ResetStateTimer(); // Reset timer, no degradation
            return;
        }

        // Priority 4: Chase Zone Only - Enter degradation mode
        if (inChaseZone)
        {
            targetedCollider = SelectTarget(Enum_DetectionColliderType.Chase_Zone);

            if (CurrentState == Enum_DetectionState.Chasing)
            {
                // Chasing but only in chase zone - start degradation timer
                if (!isInDegradationMode)
                {
                    StartDegradation();
                }

                // Check if enough time has passed to downgrade to Alerted
                if (stateTimer >= chasingDegradeTime)
                {
                    CurrentState = Enum_DetectionState.Alerted;
                    ResetStateTimer(); // Reset for next degradation phase
                }
                else
                {
                    CurrentState = Enum_DetectionState.Chasing; // Maintain state
                }
            }
            else if (CurrentState == Enum_DetectionState.Alerted)
            {
                // Alerted but only in chase zone - start degradation timer
                if (!isInDegradationMode)
                {
                    StartDegradation();
                }

                // Check if enough time has passed to forget (go Idle)
                if (stateTimer >= alertedForgetTime)
                {
                    CurrentState = Enum_DetectionState.Idle;
                    targetedCollider = null;
                    ResetStateTimer();
                }
                else
                {
                    CurrentState = Enum_DetectionState.Alerted; // Maintain state
                }
            }
            else if (CurrentState == Enum_DetectionState.Idle)
            {
                // Already idle, stay idle
                CurrentState = Enum_DetectionState.Idle;
                ResetStateTimer();
            }
            return;
        }

        // Priority 5: No targets detected anywhere - Go Idle immediately
        targetedCollider = null;
        CurrentState = Enum_DetectionState.Idle;
        ResetStateTimer();
    }


    /// <summary>
    /// Starts the degradation timer
    /// </summary>
    void StartDegradation()
    {
        isInDegradationMode = true;
        stateTimer = 0f;
    }

    /// <summary>
    /// Resets the state timer and exits degradation mode
    /// </summary>
    void ResetStateTimer()
    {
        stateTimer = 0f;
        isInDegradationMode = false;
    }

    /// <summary>
    /// Updates the state timer when in degradation mode
    /// </summary>
    void UpdateStateTimer()
    {
        if (isInDegradationMode)
        {
            stateTimer += Time.deltaTime;
        }
    }
    /// <summary>
    /// Selects the primary target from a specific collider type
    /// Currently selects the nearest target
    /// </summary>
    Collider SelectTarget(Enum_DetectionColliderType type)
    {
        if (useNearestTarget)
        {
            return SelectTargetNearest(type);
        }
        else
        {
            return SelectTargetFirstSeen(type);
        }
    }

    Collider SelectTargetNearest(Enum_DetectionColliderType type)
    {
        if (!ColliderListsDictionary.ContainsKey(type))
            return null;

        List<Collider> targets = ColliderListsDictionary[type].list;

        if (targets == null || targets.Count == 0)
            return null;

        // If only one target, return it
        if (targets.Count == 1)
            return targets[0];

        // Find nearest target
        Collider nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider target in targets)
        {
            if (target == null) continue;

            float distance = Vector3.Distance(entity.transform.position, target.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = target;
            }
        }

        return nearest;
    }


    /// <summary>
    /// Selects the primary target from a specific collider type
    /// Selects the FIRST DETECTED target (sticky targeting)
    /// </summary>
    Collider SelectTargetFirstSeen(Enum_DetectionColliderType type)
    {
        if (!ColliderListsDictionary.ContainsKey(type))
            return null;

        List<Collider> targets = ColliderListsDictionary[type].list;

        if (targets == null || targets.Count == 0)
            return null;

        // Return the first valid target in the list
        foreach (Collider target in targets)
        {
            if (target != null)
                return target;
        }

        return null;
    }

    /// <summary>
    /// Checks if there are any targets in the specified collider type
    /// </summary>
    bool HasTargetsInCollider(Enum_DetectionColliderType type)
    {
        if (!ColliderListsDictionary.ContainsKey(type))
            return false;

        return ColliderListsDictionary[type].list.Count > 0;
    }
    /// <summary>
    /// Gets the current targeted collider
    /// </summary>
    public Collider GetCurrentTarget() => targetedCollider;
}

