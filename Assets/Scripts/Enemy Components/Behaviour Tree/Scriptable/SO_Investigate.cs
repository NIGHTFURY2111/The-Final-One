//using UnityEngine;
//using UnityEngine.AI;

///// <summary>
///// DEPRECATED: This node has been split into two separate nodes for better modularity:
///// - SO_MoveToLastKnown: Handles moving to the last known position
///// - SO_LookAround: Handles looking around in different directions
///// 
///// See EnemyTree.cs for the new implementation using a Sequence of these two nodes.
///// </summary>
//[CreateAssetMenu(fileName = "Investigate (DEPRECATED)", menuName = "Behaviour Tree/Scriptable/Action/Investigate (DEPRECATED)")]
//public class SO_Investigate : SO_BehaviourNode
//{
//    [Header("Investigation Settings")]
//    [SerializeField] private float investigateReachedDistance = 2f;
//    [SerializeField] private float lookAroundDuration = 3f;
//    [SerializeField] private int numberOfLookDirections = 4;
//    [SerializeField] private float rotationSpeed = 120f;
//    [SerializeField] private float pauseBetweenLooks = 0.5f;
    
//    [Header("Debug")]
//    [SerializeField] private bool debugMode = false;
    
//    // Runtime state
//    private Vector3 lastKnownPosition;
//    private bool hasReachedPosition = false;
//    private float lookTimer = 0f;
//    private float pauseTimer = 0f;
//    private int currentLookIndex = 0;
//    private Quaternion startRotation;
    
//    public override void Reset()
//    {
//        base.Reset();
//        hasReachedPosition = false;
//        lookTimer = 0f;
//        pauseTimer = 0f;
//        currentLookIndex = 0;
//    }
    
//    public override NodeState Evaluate()
//    {
//        // Only run if in Alerted state
//        if (detector.CurrentState != Enum_DetectionState.Alerted)
//        {
//            Reset();
//            state = NodeState.Failure;
//            return state;
//        }
        
//        // Update last known position if we have a current target
//        if (detector.CurrentTarget != null)
//        {
//            lastKnownPosition = detector.CurrentTarget.transform.position;
//            hasReachedPosition = false;
//        }
        
//        // Phase 1: Move to last known position
//        if (!hasReachedPosition)
//        {
//            return MoveToInvestigatePosition();
//        }
        
//        // Phase 2: Look around
//        return LookAround();
//    }
    
//    private NodeState MoveToInvestigatePosition()
//    {
//        float distanceToTarget = Vector3.Distance(BT_Entity.transform.position, lastKnownPosition);
        
//        if (distanceToTarget <= investigateReachedDistance)
//        {
//            // Reached position - stop and start looking around
//            if (BT_Entity.navMeshAgent != null && BT_Entity.navMeshAgent.isOnNavMesh)
//            {
//                BT_Entity.navMeshAgent.isStopped = true;
//            }
            
//            hasReachedPosition = true;
//            lookTimer = 0f;
//            currentLookIndex = 0;
//            startRotation = BT_Entity.transform.rotation;
            
//            if (debugMode)
//                Debug.Log($"[SO_Investigate] Reached position, starting look around");
            
//            state = NodeState.Running;
//            return state;
//        }
        
//        // Still moving to position
//        if (BT_Entity.navMeshAgent != null && BT_Entity.navMeshAgent.isOnNavMesh)
//        {
//            BT_Entity.navMeshAgent.isStopped = false;
//            BT_Entity.navMeshAgent.SetDestination(lastKnownPosition);
//        }
        
//        if (debugMode)
//            Debug.DrawLine(BT_Entity.transform.position, lastKnownPosition, Color.yellow);
        
//        state = NodeState.Running;
//        return state;
//    }
    
//    private NodeState LookAround()
//    {
//        lookTimer += Time.deltaTime;
        
//        // Finished looking around
//        if (lookTimer >= lookAroundDuration)
//        {
//            if (debugMode)
//                Debug.Log($"[SO_Investigate] Finished investigating");
            
//            Reset();
//            state = NodeState.Success;
//            return state;
//        }
        
//        // Handle pause between look directions
//        if (pauseTimer > 0f)
//        {
//            pauseTimer -= Time.deltaTime;
//            state = NodeState.Running;
//            return state;
//        }
        
//        // Calculate look direction
//        float anglePerLook = 360f / numberOfLookDirections;
//        float targetAngle = startRotation.eulerAngles.y + (anglePerLook * currentLookIndex);
//        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        
//        // Rotate towards target direction
//        Quaternion currentRotation = BT_Entity.transform.rotation;
//        Quaternion newRotation = Quaternion.RotateTowards(
//            currentRotation,
//            targetRotation,
//            rotationSpeed * Time.deltaTime
//        );
//        BT_Entity.transform.rotation = newRotation;
        
//        // Check if we've completed this look direction
//        if (Quaternion.Angle(newRotation, targetRotation) < 5f)
//        {
//            currentLookIndex++;
//            pauseTimer = pauseBetweenLooks;
            
//            if (debugMode)
//            {
//                Debug.Log($"[SO_Investigate] Looked {currentLookIndex}/{numberOfLookDirections}");
//                Debug.DrawRay(BT_Entity.transform.position, BT_Entity.transform.forward * 5f, Color.cyan, pauseBetweenLooks);
//            }
            
//            // If we've looked in all directions, wrap around
//            if (currentLookIndex >= numberOfLookDirections)
//            {
//                currentLookIndex = 0;
//            }
//        }
        
//        state = NodeState.Running;
//        return state;
//    }
//}
