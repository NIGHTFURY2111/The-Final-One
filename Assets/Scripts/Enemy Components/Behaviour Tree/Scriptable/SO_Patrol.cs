using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// Action: Patrols using either random wandering or predefined waypoints
/// </summary>
[CreateAssetMenu(fileName = "Patrol", menuName = "Behaviour Tree/Scriptable/Action/Patrol")]
public class SO_Patrol : SO_BehaviourNode
{
    [Header("Patrol Mode")]
    [SerializeField] private PatrolMode patrolMode = PatrolMode.Random;
    
    [Header("Waypoint Behavior Settings")]
    [SerializeField] private float waitTimeAtWaypoint = 2f;
    
    [Header("Waypoint Patrol Settings")]
    [SerializeField] private List<Transform> patrolWaypoints = new List<Transform>();
    [SerializeField] private bool loopWaypoints = true;
    [SerializeField] private bool reverseOnEnd = false;
    
    [Header("Override Movement Settings (optional)")]
    [SerializeField] private bool useCustomRadius = false;
    [SerializeField] private float customPatrolRadius = 15f;
    
    [SerializeField] private bool useCustomReachedDistance = false;
    [SerializeField] private float customWaypointReachedDistance = 2f;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    // Runtime state
    private Vector3 currentWaypoint;
    private bool hasWaypoint = false;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool movingForward = true;
    
    private enum PatrolMode
    {
        Random,
        Waypoints
    }
    
    public override void Reset()
    {
        base.Reset();
        isWaiting = false;
        waitTimer = 0f;
    }
    
    public override NodeState Evaluate()
    {
        if (movement == null || detector.CurrentState != Enum_DetectionState.Idle )
        {
            state = NodeState.Failure;
            return state;
        }

        // Handle waiting at waypoint
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                hasWaypoint = false;
                
                if (debugMode)
                    Debug.Log("[SO_Patrol] Finished waiting");
            }
            
            state = NodeState.Running;
            return state;
        }
        
        // Get threshold from settings or custom value
        float reachedDistance = useCustomReachedDistance 
            ? customWaypointReachedDistance 
            : movement.Settings.waypointReachedDistance;
        
        // Check if reached current waypoint
        if (hasWaypoint && movement.IsAtPosition(currentWaypoint, reachedDistance))
        {
            if (waitTimeAtWaypoint > 0f)
            {
                isWaiting = true;
                waitTimer = waitTimeAtWaypoint;
                
                movement.StopMovement();
                
                if (debugMode)
                    Debug.Log($"[SO_Patrol] Reached waypoint, waiting {waitTimeAtWaypoint}s");
            }
            else
            {
                hasWaypoint = false;
            }
        }
        
        // Get next waypoint if needed
        if (!hasWaypoint)
        {
            currentWaypoint = patrolMode == PatrolMode.Waypoints 
                ? GetNextWaypointPosition() 
                : GetRandomPatrolPoint();
            hasWaypoint = true;
            
            if (debugMode)
                Debug.DrawLine(BT_Entity.transform.position, currentWaypoint, Color.blue, waitTimeAtWaypoint);
        }
        
        // Move to waypoint
        movement.SetDestination(currentWaypoint);
        if (debugMode)
            ST_debug.DrawSphere(currentWaypoint, 0.5f, Color.green);    

        if (debugMode)
            Debug.DrawLine(BT_Entity.transform.position, currentWaypoint, Color.cyan);
        
        state = NodeState.Running;
        return state;
    }
    
    private Vector3 GetRandomPatrolPoint()
    {
        // Use custom radius if specified, otherwise use settings default
        float radius = useCustomRadius ? customPatrolRadius : movement.Settings.patrolRadius;
        return movement.GetRandomNavMeshPoint(BT_Entity.transform.position, radius);
    }
    
    private Vector3 GetNextWaypointPosition()
    {
        if (patrolWaypoints == null || patrolWaypoints.Count == 0)
        {
            if (debugMode)
                Debug.LogWarning("[SO_Patrol] No waypoints, using random");
            return GetRandomPatrolPoint();
        }
        
        // Get current waypoint position
        Vector3 waypointPos = patrolWaypoints[currentWaypointIndex].position;
        
        // Move to next waypoint
        if (movingForward)
        {
            currentWaypointIndex++;
            
            if (currentWaypointIndex >= patrolWaypoints.Count)
            {
                if (loopWaypoints)
                {
                    currentWaypointIndex = 0;
                }
                else if (reverseOnEnd)
                {
                    movingForward = false;
                    currentWaypointIndex = patrolWaypoints.Count - 2;
                }
                else
                {
                    currentWaypointIndex = patrolWaypoints.Count - 1;
                }
            }
        }
        else
        {
            currentWaypointIndex--;
            
            if (currentWaypointIndex < 0)
            {
                movingForward = true;
                currentWaypointIndex = 1;
            }
        }
        
        return waypointPos;
    }
}
