using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Action: Chases current target using NavMesh
/// </summary>
[CreateAssetMenu(fileName = "Chase", menuName = "Behaviour Tree/Scriptable/Action/Chase")]
public class SO_Chase : SO_BehaviourNode
{
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    private bool wasChasing = false;

    public override void Reset()
    {
        base.Reset();
        wasChasing = false;
    }
    
    public override NodeState Evaluate()
    {
        if (detector.CurrentState != Enum_DetectionState.Chasing || detector.CurrentTarget == null)
        {
            if (wasChasing)
                movement.StopMovement();
            
            wasChasing = false;
            state = NodeState.Failure;
            return state;
        }
        Debug.Log("chase");
        
        // SetDestination now handles update intervals automatically via MovementSettings
        movement.SetDestination(detector.CurrentTarget.transform.position);
        
        if (debugMode)
        {
            Debug.Log($"[SO_Chase] Chasing {detector.CurrentTarget.name}");
            Debug.DrawLine(BT_Entity.transform.position, detector.CurrentTarget.transform.position, Color.green);
        }
        wasChasing = true;
        state = NodeState.Success;
        return state;
    }
}
