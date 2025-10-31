using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Centralized movement settings for enemy navigation and rotation.
/// Configure all movement behavior from this single location.
/// </summary>
[Serializable]
public class MovementSettings
{
    [Header("Navigation")]
    [Tooltip("How often to update pathfinding destination (seconds)")]
    public float pathUpdateInterval = 0.2f;
    
    [Tooltip("Distance to consider destination as 'reached'")]
    public float destinationReachedThreshold = 0.5f;
    
    [Tooltip("Distance for waypoint arrival")]
    public float waypointReachedDistance = 2f;
    
    [Tooltip("Search radius for random patrol points")]
    public float patrolRadius = 15f;
    
    [Header("Rotation")]
    [Tooltip("Default rotation speed in degrees/second")]
    public float defaultRotationSpeed = 120f;
    
    [Tooltip("Slower rotation for precise aiming (degrees/second)")]
    public float aimRotationSpeed = 90f;
    
    [Tooltip("Fast rotation for quick look around (degrees/second)")]
    public float searchRotationSpeed = 180f;
    
    [Tooltip("Rotation completion threshold (degrees)")]
    public float rotationCompleteThreshold = 5f;
    
    [Tooltip("Lock rotation to Y axis by default (ground-based movement)")]
    public bool lockYAxisByDefault = true;
    
    [Header("NavMesh Agent Settings")]
    [Tooltip("Agent movement speed (units/second)")]
    public float moveSpeed = 3.5f;
    
    [Tooltip("Agent angular speed (degrees/second)")]
    public float angularSpeed = 120f;
    
    [Tooltip("Agent acceleration (units/second²)")]
    public float acceleration = 8f;
    
    [Tooltip("Agent stopping distance")]
    public float stoppingDistance = 0.5f;
}

/// <summary>
/// Centralized movement component for enemies.
/// Handles all NavMesh pathfinding, transform manipulation, and rotation.
/// All behavior tree nodes should use this instead of directly accessing NavMeshAgent or Transform.
/// </summary>
[CreateAssetMenu(fileName = "Enemy Movement", menuName = "Scriptable Object/Component/Enemy/Enemy Movement")]
public class Enemy_Movement : AC_Component
{
    [SerializeField] private MovementSettings settings = new MovementSettings();
    
    Enemy_Entity enemy_entity;
    private NavMeshAgent navMeshAgent;
    private Transform entityTransform;
    
    // Pathfinding update timing
    private float lastPathUpdateTime = 0f;
    
    public override Enum_ComponentType componentType => Enum_ComponentType.Movement;

    public override void ComponentAwake()
    {
        enemy_entity = (Enemy_Entity)entity;
        navMeshAgent = enemy_entity.navMeshAgent;
        entityTransform = enemy_entity.transform;
        
        ApplyNavMeshAgentSettings();
    }

    public override void ComponentStart() { }
    public override void ComponentUpdate() { }
    
    public override void ComponentDisable()
    {
        // Ensure agent is stopped when disabled
        StopMovement();
    }
    
    /// <summary>
    /// Applies movement settings to the NavMeshAgent.
    /// Called during initialization and when settings change.
    /// </summary>
    private void ApplyNavMeshAgentSettings()
    {
        if (!ValidateNavMeshAgent()) return;
        
        navMeshAgent.speed = settings.moveSpeed;
        navMeshAgent.angularSpeed = settings.angularSpeed;
        navMeshAgent.acceleration = settings.acceleration;
        navMeshAgent.stoppingDistance = settings.stoppingDistance;
    }

    #region NavMesh Movement

    /// <summary>
    /// Sets the destination for the NavMeshAgent to move towards.
    /// Automatically resumes movement if agent was stopped.
    /// Uses pathUpdateInterval to prevent excessive updates.
    /// </summary>
    /// <param name="target">World position to move to</param>
    /// <param name="forceUpdate">Force update even if interval hasn't elapsed</param>
    /// <returns>True if destination was set successfully</returns>
    public bool SetDestination(Vector3 target, bool forceUpdate = false)
    {
        if (!ValidateNavMeshAgent()) return false;

        // Check if we should update based on interval
        if (!forceUpdate && Time.time - lastPathUpdateTime < settings.pathUpdateInterval)
        {
            return true; // Path is still valid, no need to update
        }

        navMeshAgent.isStopped = false;
        bool success = navMeshAgent.SetDestination(target);
        
        if (success)
        {
            lastPathUpdateTime = Time.time;
        }
        
        return success;
    }

    /// <summary>
    /// Stops all NavMeshAgent movement.
    /// </summary>
    public void StopMovement()
    {
        if (!ValidateNavMeshAgent()) return;
        
        navMeshAgent.isStopped = true;
    }

    /// <summary>
    /// Checks if the NavMeshAgent has reached its destination.
    /// Uses configured threshold from settings.
    /// </summary>
    /// <param name="customThreshold">Optional custom threshold, uses settings default if null</param>
    /// <returns>True if remaining distance is less than threshold</returns>
    public bool IsPathComplete(float? customThreshold = null)
    {
        if (!ValidateNavMeshAgent()) return false;
        
        // Check if path is being calculated
        if (navMeshAgent.pathPending) return false;
        
        float threshold = customThreshold ?? settings.destinationReachedThreshold;
        
        // Check remaining distance
        return navMeshAgent.remainingDistance <= threshold;
    }

    /// <summary>
    /// Gets the remaining distance to the current destination.
    /// </summary>
    /// <returns>Remaining distance, or float.MaxValue if no valid path</returns>
    public float GetRemainingDistance()
    {
        if (!ValidateNavMeshAgent()) return float.MaxValue;
        
        if (navMeshAgent.pathPending) return float.MaxValue;
        
        return navMeshAgent.remainingDistance;
    }

    /// <summary>
    /// Checks if the entity is at a specific position.
    /// Uses configured waypoint reached distance from settings.
    /// </summary>
    /// <param name="position">Position to check against</param>
    /// <param name="customThreshold">Optional custom threshold, uses settings default if null</param>
    /// <returns>True if within threshold distance</returns>
    public bool IsAtPosition(Vector3 position, float? customThreshold = null)
    {
        float threshold = customThreshold ?? settings.waypointReachedDistance;
        return Vector3.Distance(entityTransform.position, position) <= threshold;
    }

    /// <summary>
    /// Finds a random navigable point on the NavMesh within a radius.
    /// Uses configured patrol radius from settings.
    /// </summary>
    /// <param name="origin">Center point to search from</param>
    /// <param name="customRadius">Optional custom radius, uses settings default if null</param>
    /// <returns>Random NavMesh position, or origin if none found</returns>
    public Vector3 GetRandomNavMeshPoint(Vector3 origin, float? customRadius = null)
    {
        float radius = customRadius ?? settings.patrolRadius;
        
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += origin;
        randomDirection.y = origin.y; // Keep on same height level
        
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        // Fallback to origin if no valid point found
        return origin;
    }

    /// <summary>
    /// Validates that NavMeshAgent exists and is on NavMesh.
    /// </summary>
    private bool ValidateNavMeshAgent()
    {
        return navMeshAgent != null && navMeshAgent.isOnNavMesh;
    }

    #endregion

    #region Transform Rotation

    /// <summary>
    /// Smoothly rotates towards a target position.
    /// Uses default rotation speed from settings.
    /// </summary>
    /// <param name="target">Position to look at</param>
    /// <param name="customSpeed">Optional custom speed, uses settings default if null</param>
    /// <param name="customLockY">Optional Y-lock override, uses settings default if null</param>
    /// <returns>Remaining angle to target in degrees</returns>
    public float RotateTowards(Vector3 target, float? customSpeed = null, bool? customLockY = null)
    {
        float rotationSpeed = customSpeed ?? settings.defaultRotationSpeed;
        bool lockY = customLockY ?? settings.lockYAxisByDefault;
        
        Vector3 direction = target - entityTransform.position;
        
        if (lockY)
            direction.y = 0;
        
        if (direction == Vector3.zero)
            return 0f;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion newRotation = Quaternion.RotateTowards(
            entityTransform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
        
        entityTransform.rotation = newRotation;
        
        return Quaternion.Angle(newRotation, targetRotation);
    }

    /// <summary>
    /// Smoothly rotates towards a target quaternion rotation.
    /// Uses default rotation speed from settings.
    /// </summary>
    /// <param name="targetRotation">Target rotation</param>
    /// <param name="customSpeed">Optional custom speed, uses settings default if null</param>
    /// <returns>Remaining angle to target in degrees</returns>
    public float RotateTowards(Quaternion targetRotation, float? customSpeed = null)
    {
        float rotationSpeed = customSpeed ?? settings.defaultRotationSpeed;
        
        Quaternion newRotation = Quaternion.RotateTowards(
            entityTransform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
        
        entityTransform.rotation = newRotation;
        
        return Quaternion.Angle(newRotation, targetRotation);
    }

    /// <summary>
    /// Instantly sets the rotation to a specific quaternion.
    /// </summary>
    /// <param name="rotation">Target rotation</param>
    public void SetRotation(Quaternion rotation)
    {
        entityTransform.rotation = rotation;
    }

    /// <summary>
    /// Instantly looks at a target position.
    /// Uses Y-lock setting from configuration.
    /// </summary>
    /// <param name="target">Position to look at</param>
    /// <param name="customLockY">Optional Y-lock override, uses settings default if null</param>
    public void LookAt(Vector3 target, bool? customLockY = null)
    {
        bool lockY = customLockY ?? settings.lockYAxisByDefault;
        
        Vector3 direction = target - entityTransform.position;
        
        if (lockY)
            direction.y = 0;
        
        if (direction == Vector3.zero)
            return;
        
        entityTransform.rotation = Quaternion.LookRotation(direction);
    }

    /// <summary>
    /// Smoothly looks at a target position using Slerp.
    /// Uses aim rotation speed from settings for precise aiming.
    /// </summary>
    /// <param name="target">Position to look at</param>
    /// <param name="customSpeed">Optional custom speed, uses aimRotationSpeed if null</param>
    /// <param name="customLockY">Optional Y-lock override, uses settings default if null</param>
    public void SmoothLookAt(Vector3 target, float? customSpeed = null, bool? customLockY = null)
    {
        float rotationSpeed = customSpeed ?? settings.aimRotationSpeed;
        bool lockY = customLockY ?? settings.lockYAxisByDefault;
        
        Vector3 direction = target - entityTransform.position;
        
        if (lockY)
            direction.y = 0;
        
        if (direction == Vector3.zero)
            return;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        entityTransform.rotation = Quaternion.Slerp(
            entityTransform.rotation,
            targetRotation,
            rotationSpeed /** Time.deltaTime*/
        );
    }

    /// <summary>
    /// Gets the current rotation of the entity.
    /// </summary>
    public Quaternion GetRotation()
    {
        return entityTransform.rotation;
    }

    /// <summary>
    /// Gets the current forward direction of the entity.
    /// </summary>
    public Vector3 GetForward()
    {
        return entityTransform.forward;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Public access to movement settings for nodes that need to read configuration.
    /// </summary>
    public MovementSettings Settings => settings;

    /// <summary>
    /// Gets the entity's current position.
    /// </summary>
    public Vector3 Position => entityTransform.position;

    /// <summary>
    /// Checks if the NavMeshAgent is currently moving.
    /// </summary>
    public bool IsMoving => ValidateNavMeshAgent() && !navMeshAgent.isStopped && navMeshAgent.velocity.sqrMagnitude > 0.01f;

    /// <summary>
    /// Checks if the NavMeshAgent is currently stopped.
    /// </summary>
    public bool IsStopped => !ValidateNavMeshAgent() || navMeshAgent.isStopped;

    #endregion
}
