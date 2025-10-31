using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Action: Moves to the last known position of the target
/// </summary>
[CreateAssetMenu(fileName = "MoveToLastKnown", menuName = "Behaviour Tree/Scriptable/Action/MoveToLastKnown")]
public class SO_MoveToLastKnown : SO_BehaviourNode
{
    [Header("Override Movement Settings (optional)")]
    [SerializeField] private bool useCustomReachedDistance = false;
    [SerializeField] private float customReachedDistance = 2f;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    // Runtime state
    private Vector3 lastKnownPosition;
    private bool hasStoredPosition = false;
    
    public override void Reset()
    {
        base.Reset();
        hasStoredPosition = false;
    }
    
    public override NodeState Evaluate()
    {
        // Only run if in Alerted state
        if (detector.CurrentState != Enum_DetectionState.Alerted)
        {
            Reset();
            state = NodeState.Failure;
            return state;
        }
        Debug.Log("move to last known");
        
        // Update last known position if we have a current target
        if (detector.CurrentTarget != null)
        {
            lastKnownPosition = detector.CurrentTarget.transform.position;
            hasStoredPosition = true;
        }
        
        // Can't navigate if we don't have a position stored
        if (!hasStoredPosition)
        {
            state = NodeState.Failure;
            return state;
        }
        
        // Get threshold from custom value or movement settings
        float reachedDistance = useCustomReachedDistance 
            ? customReachedDistance 
            : movement.Settings.waypointReachedDistance;
        
        // Check if we've reached the position
        if (movement.IsAtPosition(lastKnownPosition, reachedDistance))
        {
            // Stop at the position
            movement.StopMovement();
            
            if (debugMode)
                Debug.Log($"[SO_MoveToLastKnown] Reached last known position");
            
            state = NodeState.Success;
            return state;
        }
        
        // Still moving to position
        movement.SetDestination(lastKnownPosition);
        
        if (debugMode)
            Debug.DrawLine(BT_Entity.transform.position, lastKnownPosition, Color.yellow);
        
        state = NodeState.Running;
        return state;
    }
}
