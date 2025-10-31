using UnityEngine;

/// <summary>
/// Condition: Checks if target is within attack/weapon range
/// IMPORTANT: This checks raw distance, not line of sight. 
/// If you need line of sight checks, those should be handled by the Vision Cone collider.
/// </summary>
[CreateAssetMenu(fileName = "InRange", menuName = "Behaviour Tree/Scriptable/Condition/In Range")]
public class SO_InRange : SO_BehaviourNode
{
    [Header("Range Settings")]
    [Tooltip("Maximum weapon range. If > vision range, enemy can shoot at last known positions.")]
    [SerializeField] private float attackRange = 50f;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
       
    public override void Initialize(EnemyTree tree)
    {
        base.Initialize(tree);
    }
    
    public override NodeState Evaluate()
    {
        
        // No target means we can't be in range
        if (detector.CurrentTarget == null || detector.CurrentState == Enum_DetectionState.Idle)
        {
            state = NodeState.Failure;
            return state;
        }
        Debug.Log("InRange");
        
        float distanceToTarget = Vector3.Distance(
            BT_Entity.transform.position, 
            detector.CurrentTarget.transform.position
        );
        
        
        bool inRange = distanceToTarget <= attackRange;

        state = inRange ? NodeState.Success : NodeState.Failure;
        
        if (debugMode)
        {
            Color lineColor = inRange ? Color.green : Color.red;
            Debug.DrawLine(
                BT_Entity.transform.position, 
                detector.CurrentTarget.transform.position, 
                lineColor, 
                0.1f
            );
            Debug.Log($"[InRange] Distance: {distanceToTarget:F2}m / {attackRange:F2}m, InRange: {inRange}, State: {detector.CurrentState}");
        }
        
        return state;
    }
    

}
