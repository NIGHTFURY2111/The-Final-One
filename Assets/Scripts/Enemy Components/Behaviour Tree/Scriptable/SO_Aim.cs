using UnityEngine;

/// <summary>
/// Action: Aims enemy towards current target
/// </summary>
[CreateAssetMenu(fileName = "Aim", menuName = "Behaviour Tree/Scriptable/Action/Aim")]
public class SO_Aim : SO_BehaviourNode
{
    [Header("Aim Behavior Settings")]
    [SerializeField] private bool instantRotation = false;
    
    [Header("Override Movement Settings (optional)")]
    [SerializeField] private bool useCustomRotationSpeed = false;
    [SerializeField] private float customRotationSpeed = 5f;
    
    [SerializeField] private bool customLockY = true;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    public override NodeState Evaluate()
    {
        if (detector.CurrentTarget == null || detector.CurrentState == Enum_DetectionState.Idle)
        {
            state = NodeState.Failure;
            return state;
        }
        Debug.Log("aim");
        
        Vector3 targetPosition = detector.CurrentTarget.transform.position;
                
        // Use centralized movement for rotation
        if (instantRotation)
        {
            movement.LookAt(targetPosition, customLockY);
        }
        else
        {
            // Use custom speed if specified, otherwise use aim speed from settings
            float rotationSpeed = useCustomRotationSpeed 
                ? customRotationSpeed 
                : movement.Settings.aimRotationSpeed;
            
            movement.SmoothLookAt(targetPosition, rotationSpeed, customLockY);
        }
        
        if (debugMode)
        {
            Vector3 direction = targetPosition - BT_Entity.transform.position;
            if (customLockY) direction.y = 0;
            Debug.DrawRay(BT_Entity.transform.position, direction.normalized * 5f, Color.red);
        }
        
        state = NodeState.Success;
        return state;
    }
}
