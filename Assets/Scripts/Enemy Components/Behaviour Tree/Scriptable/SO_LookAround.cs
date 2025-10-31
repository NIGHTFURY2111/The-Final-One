using UnityEngine;

/// <summary>
/// Action: Looks around by rotating in different directions
/// </summary>
[CreateAssetMenu(fileName = "LookAround", menuName = "Behaviour Tree/Scriptable/Action/LookAround")]
public class SO_LookAround : SO_BehaviourNode
{
    [Header("Look Around Behavior Settings")]
    [SerializeField] private float lookAroundDuration = 3f;
    [SerializeField] private int numberOfLookDirections = 4;
    [SerializeField] private float pauseBetweenLooks = 0.5f;
    
    [Header("Override Movement Settings (optional)")]
    [SerializeField] private bool useCustomRotationSpeed = false;
    [SerializeField] private float customRotationSpeed = 120f;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    // Runtime state
    private float lookTimer = 0f;
    private float pauseTimer = 0f;
    private int currentLookIndex = 0;
    private Quaternion startRotation;
    private bool isInitialized = false;
    
    public override void Reset()
    {
        base.Reset();
        lookTimer = 0f;
        pauseTimer = 0f;
        currentLookIndex = 0;
        isInitialized = false;
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
        Debug.Log("look around");
        
        // Initialize on first run
        if (!isInitialized)
        {
            // Make sure agent is stopped
            movement.StopMovement();
            
            lookTimer = 0f;
            currentLookIndex = 0;
            startRotation = movement.GetRotation();
            isInitialized = true;
            
            if (debugMode)
                Debug.Log($"[SO_LookAround] Starting look around");
        }
        
        lookTimer += Time.deltaTime;
        
        // Finished looking around
        if (lookTimer >= lookAroundDuration)
        {
            if (debugMode)
                Debug.Log($"[SO_LookAround] Finished looking around");
            
            Reset();
            state = NodeState.Success;
            return state;
        }
        
        // Handle pause between look directions
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            state = NodeState.Running;
            return state;
        }
        Debug.Log("looking");
        // Calculate look direction
        float anglePerLook = 360f / numberOfLookDirections;
        float targetAngle = startRotation.eulerAngles.y + (anglePerLook * currentLookIndex);
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        
        // Use custom rotation speed if specified, otherwise use search speed from settings
        float rotationSpeed = useCustomRotationSpeed 
            ? customRotationSpeed 
            : movement.Settings.searchRotationSpeed;
        
        // Rotate towards target direction using centralized movement
        float remainingAngle = movement.RotateTowards(targetRotation, rotationSpeed);
        
        // Use rotation threshold from movement settings
        if (remainingAngle < movement.Settings.rotationCompleteThreshold)
        {
            currentLookIndex++;
            pauseTimer = pauseBetweenLooks;
            
            if (debugMode)
            {
                Debug.Log($"[SO_LookAround] Looked {currentLookIndex}/{numberOfLookDirections}");
                Debug.DrawRay(BT_Entity.transform.position, movement.GetForward() * 5f, Color.cyan, pauseBetweenLooks);
            }
            
            // If we've looked in all directions, wrap around
            if (currentLookIndex >= numberOfLookDirections)
            {
                currentLookIndex = 0;
                startRotation = movement.GetRotation();
            }
        }
        
        state = NodeState.Running;
        return state;
    }
}
