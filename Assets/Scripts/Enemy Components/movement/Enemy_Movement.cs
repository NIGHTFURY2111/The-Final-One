using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class MovementSettings
{
    public float pathUpdateInterval = 0.2f;
    public float destinationReachedThreshold = 0.5f;
    public float waypointReachedDistance = 2f;
    public float patrolRadius = 15f;
    public float defaultRotationSpeed = 120f;
    public float aimRotationSpeed = 90f;
    public float searchRotationSpeed = 180f;
    public float rotationCompleteThreshold = 5f;
    public bool lockYAxisByDefault = true;
    public float moveSpeed = 3.5f;
    public float angularSpeed = 120f;
    public float acceleration = 8f;
    public float stoppingDistance = 0.5f;
}

[CreateAssetMenu(fileName = "Enemy Movement", menuName = "Scriptable Object/Component/Enemy/Enemy Movement")]
public class Enemy_Movement : AC_Component<Enemy_Movement>
{
    [SerializeField] private MovementSettings settings = new MovementSettings();
    
    Enemy_Entity enemy_entity;
    private NavMeshAgent navMeshAgent;
    private Transform entityTransform;
    
    private float lastPathUpdateTime = 0f;
    
    public override Enum_ComponentType componentType => Enum_ComponentType.Movement;

    public override void ComponentAwake()
    {
        enemy_entity = entity as Enemy_Entity;
        navMeshAgent = enemy_entity.navMeshAgent;
        entityTransform = enemy_entity.transform;
        
        ApplyNavMeshAgentSettings();
    }

    public override void ComponentStart() { }
    public override void ComponentUpdate() { }
    public override void ComponentDisable() { StopMovement(); }
    private void ApplyNavMeshAgentSettings()
    {
        if (!ValidateNavMeshAgent) return;
        
        navMeshAgent.speed = settings.moveSpeed;
        navMeshAgent.angularSpeed = settings.angularSpeed;
        navMeshAgent.acceleration = settings.acceleration;
        navMeshAgent.stoppingDistance = settings.stoppingDistance;
    }
    public void StopMovement()
    {
        if (!ValidateNavMeshAgent) return;

        navMeshAgent.isStopped = true;
    }

    public void StartMovement()
    {
        if (!ValidateNavMeshAgent) return;

        navMeshAgent.isStopped = false;
    }

    public bool SetDestination(Vector3 target, bool forceUpdate = false)
    {
        if (!ValidateNavMeshAgent) return false;

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

    public bool IsPathComplete(float? customThreshold = null)
    {
        if (!ValidateNavMeshAgent) return false;
        
        // Check if path is being calculated
        if (navMeshAgent.pathPending) return false;
        
        float threshold = customThreshold ?? settings.destinationReachedThreshold;
        
        // Check remaining distance
        return navMeshAgent.remainingDistance <= threshold;
    }

    public float GetRemainingDistance()
    {
        if (!ValidateNavMeshAgent) return float.MaxValue;
        
        if (navMeshAgent.pathPending) return float.MaxValue;
        
        return navMeshAgent.remainingDistance;
    }

    public bool IsAtPosition(Vector3 position, float? customThreshold = null)
    {
        float threshold = customThreshold ?? settings.waypointReachedDistance;
        return Vector3.Distance(entityTransform.position, position) <= threshold;
    }
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

    private bool ValidateNavMeshAgent => navMeshAgent != null && navMeshAgent.isOnNavMesh;
    public Quaternion GetRotation => entityTransform.rotation;
    public Vector3 GetForward => entityTransform.forward;
    public void SetRotation(Quaternion rotation) => entityTransform.rotation = rotation;
    public MovementSettings Settings => settings;
    public Vector3 Position => entityTransform.position;
    public bool IsMoving => ValidateNavMeshAgent && !navMeshAgent.isStopped && navMeshAgent.velocity.sqrMagnitude > 0.01f;
    public bool IsStopped => !ValidateNavMeshAgent || navMeshAgent.isStopped;
}
