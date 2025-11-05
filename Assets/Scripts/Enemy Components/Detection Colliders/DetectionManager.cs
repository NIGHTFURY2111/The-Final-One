using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All possible States that the Enemy can be in based on detection of target Entities
/// </summary>
public enum Enum_DetectionState
{
    Idle,
    Alerted,
    Chasing,
}

[CreateAssetMenu(fileName = "Enemy Detection Manager", menuName = "Scriptable Object/Component/Enemy/Enemy Detection Manager")]
public class DetectionManager : AC_Component
{
    #region Nested Types

    /// <summary>
    /// Snapshot of current detection state across all colliders
    /// </summary>
    private struct DetectionSnapshot
    {
        public bool InVisionCone;
        public bool InCloseRange;
        public bool InChaseZone;
    }

    /// <summary>
    /// Handles state degradation timing and logic
    /// </summary>
    [Serializable]
    private class StateDegradationHandler
    {
        [SerializeField] private float alertedForgetTime = 3f;
        [SerializeField] private float chasingDegradeTime = 5f;

        private float timer = 0f;
        private bool isActive = false;

        public float AlertedForgetTime => alertedForgetTime;
        public float ChasingDegradeTime => chasingDegradeTime;
        public float Timer => timer;
        public bool IsActive => isActive;

        public void Start()
        {
            isActive = true;
            timer = 0f;
        }

        public void Reset()
        {
            isActive = false;
            timer = 0f;
        }

        public void Update()
        {
            if (isActive)
            {
                timer += Time.deltaTime;
            }
        }

        public bool ShouldDegradeFromChasing() => isActive && timer >= chasingDegradeTime;
        public bool ShouldForgetFromAlerted() => isActive && timer >= alertedForgetTime;
    }

    #endregion

    #region Serialized Fields

    [SerializeField] private TargetSelector targetSelector;
    [SerializeField] private StateDegradationHandler degradationHandler = new StateDegradationHandler();
    [SerializeField] private bool debugMode = false;

    #endregion

    #region Private Fields 

    private Dictionary<Enum_DetectionColliderType, DetectionCollider> DetectorDictionary = new();
    private Dictionary<Enum_DetectionColliderType, DetectedGameObjectList> ColliderListsDictionary = new();
    private Collider targetedCollider;
    private Enum_DetectionState _currentState;

    // Cached collider availability flags
    private bool hasVisionCone;
    private bool hasCloseRange;
    private bool hasChaseZone;

    #endregion

    #region Properties

    public override Enum_ComponentType componentType => Enum_ComponentType.Detector;

    /// <summary>
    /// Current detection state of the enemy
    /// </summary>
    public Enum_DetectionState CurrentState
    {
        get => _currentState;
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                InvokeStateChanged();
            }
        }
    }

    /// <summary>
    /// Gets the currently targeted collider
    /// </summary>
    public Collider CurrentTarget => targetedCollider;

    /// <summary>
    /// Gets the current degradation timer value (for debugging)
    /// </summary>
    public float StateTimer => degradationHandler.Timer;

    /// <summary>
    /// Gets whether the enemy is in degradation mode (for debugging)
    /// </summary>
    public bool IsInDegradation => degradationHandler.IsActive;

    #endregion

    #region Events

    /// <summary>
    /// Event fired when detection state changes. Passes the new state and target collider.
    /// </summary>
    public Action<Enum_DetectionState, Collider> OnStateChanged;

    #endregion

    #region Unity/Component Lifecycle

    public override void ComponentAwake() { }

    public override void ComponentStart()
    {
        BuildDictionary();
        AssignTagsAndSubscribe();
        CurrentState = Enum_DetectionState.Idle;
    }

    public override void ComponentUpdate()
    {
        degradationHandler.Update();
        DetermineState();

        if (debugMode)
        {
            Debug.Log($"[DetectionManager] State: {CurrentState} - Target: {(CurrentTarget != null ? CurrentTarget.name : "None")}");
        }
    }

    public override void ComponentDisable()
    {
        UnsubscribeFromColliders();
        ClearStickyTarget();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Builds dictionaries of detection colliders and caches availability flags
    /// </summary>
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

        // Cache collider availability
        hasVisionCone = DetectorDictionary.ContainsKey(Enum_DetectionColliderType.Vision_Cone);
        hasCloseRange = DetectorDictionary.ContainsKey(Enum_DetectionColliderType.Close_Range);
        hasChaseZone = DetectorDictionary.ContainsKey(Enum_DetectionColliderType.Chase_Zone);
    }

    /// <summary>
    /// Assigns detection tags to colliders and subscribes to their update events
    /// </summary>
    private void AssignTagsAndSubscribe()
    {
        if (!(entity is Enemy_Entity enemyEntity))
        {
            Debug.LogError($"[DetectionManager] Requires Enemy_Entity but got {entity?.GetType().Name}");
            return;
        }

        Enum_Tag tags = enemyEntity.targetTags;
        foreach (DetectionCollider col in DetectorDictionary.Values)
        {
            col.SetDetectionTags(tags);
            col.OnColliderUpdate += HandleDetectorUpdate;
        }
    }

    /// <summary>
    /// Unsubscribes from all detection collider events
    /// </summary>
    private void UnsubscribeFromColliders()
    {
        foreach (DetectionCollider col in DetectorDictionary.Values)
        {
            col.OnColliderUpdate -= HandleDetectorUpdate;
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles updates from individual detection colliders
    /// </summary>
    void HandleDetectorUpdate(Enum_DetectionColliderType type)
    {
        DetermineState();
    }

    /// <summary>
    /// Invokes the state changed event
    /// </summary>
    private void InvokeStateChanged()
    {
        OnStateChanged?.Invoke(_currentState, targetedCollider);
    }

    #endregion

    #region State Management

    /// <summary>
    /// Determines the current state based on priority hierarchy of all detection colliders
    /// Handles edge cases for state transitions and time-based degradation
    /// </summary>
    void DetermineState()
    {
        DetectionSnapshot snapshot = GatherDetectionData();

        // Try each priority level in order
        if (TryHandleVisionCone(snapshot)) return;
        if (TryHandleCloseRange(snapshot)) return;
        if (TryHandleChaseZone(snapshot)) return;

        HandleNoDetection();
    }

    /// <summary>
    /// Gathers current detection data from all colliders
    /// </summary>
    DetectionSnapshot GatherDetectionData()
    {
        return new DetectionSnapshot
        {
            InVisionCone = HasTargetsInCollider(Enum_DetectionColliderType.Vision_Cone),
            InCloseRange = HasTargetsInCollider(Enum_DetectionColliderType.Close_Range),
            InChaseZone = HasTargetsInCollider(Enum_DetectionColliderType.Chase_Zone)
        };
    }

    /// <summary>
    /// Handles vision cone detection - highest priority, always means chasing
    /// </summary>
    bool TryHandleVisionCone(DetectionSnapshot snapshot)
    {
        if (!snapshot.InVisionCone) return false;

        targetedCollider = SelectTarget(Enum_DetectionColliderType.Vision_Cone);
        CurrentState = Enum_DetectionState.Chasing;
        degradationHandler.Reset();
        return true;
    }

    /// <summary>
    /// Handles close range detection with state-dependent behavior
    /// </summary>
    bool TryHandleCloseRange(DetectionSnapshot snapshot)
    {
        if (!snapshot.InCloseRange) return false;

        targetedCollider = SelectTarget(Enum_DetectionColliderType.Close_Range);

        // If already chasing, keep chasing (target got behind but is still close)
        if (CurrentState == Enum_DetectionState.Chasing)
        {
            CurrentState = Enum_DetectionState.Chasing;
        }
        else
        {
            CurrentState = Enum_DetectionState.Alerted;
        }

        degradationHandler.Reset();
        return true;
    }

    /// <summary>
    /// Handles chase zone detection with degradation logic
    /// </summary>
    bool TryHandleChaseZone(DetectionSnapshot snapshot)
    {
        if (!snapshot.InChaseZone) return false;

        targetedCollider = SelectTarget(Enum_DetectionColliderType.Chase_Zone);

        if (CurrentState == Enum_DetectionState.Chasing)
        {
            HandleChasingInChaseZone();
        }
        else if (CurrentState == Enum_DetectionState.Alerted)
        {
            HandleAlertedInChaseZone();
        }
        else if (CurrentState == Enum_DetectionState.Idle)
        {
            CurrentState = Enum_DetectionState.Idle;
            degradationHandler.Reset();
        }

        return true;
    }

    /// <summary>
    /// Handles chasing state when target is only in chase zone (degradation to alerted)
    /// </summary>
    void HandleChasingInChaseZone()
    {
        if (!degradationHandler.IsActive)
        {
            degradationHandler.Start();
        }

        if (degradationHandler.ShouldDegradeFromChasing())
        {
            CurrentState = Enum_DetectionState.Alerted;
            degradationHandler.Reset();
        }
        else
        {
            CurrentState = Enum_DetectionState.Chasing;
        }
    }

    /// <summary>
    /// Handles alerted state when target is only in chase zone (degradation to idle)
    /// </summary>
    void HandleAlertedInChaseZone()
    {
        if (!degradationHandler.IsActive)
        {
            degradationHandler.Start();
        }

        if (degradationHandler.ShouldForgetFromAlerted())
        {
            CurrentState = Enum_DetectionState.Idle;
            targetedCollider = null;
            degradationHandler.Reset();
        }
        else
        {
            CurrentState = Enum_DetectionState.Alerted;
        }
    }

    /// <summary>
    /// Handles no detection - immediately go idle
    /// </summary>
    void HandleNoDetection()
    {
        targetedCollider = null;
        CurrentState = Enum_DetectionState.Idle;
        degradationHandler.Reset();
        ClearStickyTarget();
    }

    #endregion

    #region Target Selection

    /// <summary>
    /// Selects the primary target from a specific collider type based on configured strategy
    /// </summary>
    Collider SelectTarget(Enum_DetectionColliderType type)
    {
        if (targetSelector == null)
        {
            Debug.LogWarning("[DetectionManager] No target selector assigned! Using first available target.");
            return SelectTargetFallback(type);
        }

        if (!ColliderListsDictionary.ContainsKey(type))
            return null;

        List<Collider> targets = ColliderListsDictionary[type].list;
        return targetSelector.SelectTarget(targets, entity.transform.position);
    }

    /// <summary>
    /// Fallback target selection when no strategy is assigned
    /// </summary>
    Collider SelectTargetFallback(Enum_DetectionColliderType type)
    {
        if (!ColliderListsDictionary.ContainsKey(type))
            return null;

        List<Collider> targets = ColliderListsDictionary[type].list;

        if (targets == null || targets.Count == 0)
            return null;

        foreach (Collider target in targets)
        {
            if (target != null)
                return target;
        }

        return null;
    }

    /// <summary>
    /// Clears locked target if using sticky target selector
    /// </summary>
    void ClearStickyTarget()
    {
        if (targetSelector is StickyTargetSelector stickySelector)
        {
            stickySelector.ClearLockedTarget();
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Checks if there are any targets in the specified collider type
    /// </summary>
    bool HasTargetsInCollider(Enum_DetectionColliderType type)
    {
        // Fast path - check cached flags first
        switch (type)
        {
            case Enum_DetectionColliderType.Vision_Cone when !hasVisionCone:
            case Enum_DetectionColliderType.Close_Range when !hasCloseRange:
            case Enum_DetectionColliderType.Chase_Zone when !hasChaseZone:
                return false;
        }

        if (!ColliderListsDictionary.ContainsKey(type))
            return false;

        return ColliderListsDictionary[type].list.Count > 0;
    }

    #endregion
}

