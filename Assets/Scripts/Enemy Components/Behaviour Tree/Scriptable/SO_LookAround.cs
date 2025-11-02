using UnityEngine;

/// <summary>
/// Action: Looks around by rotating in different directions
/// Investigation pattern: Look left/right, turn around, look left/right, return to initial direction
/// </summary>
[CreateAssetMenu(fileName = "LookAround", menuName = "Behaviour Tree/Scriptable/Action/LookAround")]
public class SO_LookAround : SO_BehaviourNode
{
    [Header("Look Around Behavior Settings")]
    [SerializeField] private float leftRightLookAngle = 90f;  // How far to look left/right
    [SerializeField] private float pauseBetweenLooks = 0.5f;
    [SerializeField] private float timeoutDuration = 15f; // Safety timeout
    
    [Header("Override Movement Settings (optional)")]
    [SerializeField] private bool useCustomRotationSpeed = false;
    [SerializeField] private float customRotationSpeed = 120f;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    // Runtime state
    private enum LookPhase
    {
        LookLeft,           // 0: Look left from start
        ReturnCenter1,      // 1: Return to center
        LookRight,          // 2: Look right from start
        ReturnCenter2,      // 3: Return to center
        TurnAround,         // 4: Turn 180 degrees
        LookLeftBack,       // 5: Look left from back position
        ReturnCenterBack1,  // 6: Return to back center
        LookRightBack,      // 7: Look right from back position
        ReturnCenterBack2,  // 8: Return to back center
        ReturnToStart,      // 9: Turn back to initial direction
        Complete            // 10: Finished
    }
    
    private LookPhase currentPhase = LookPhase.LookLeft;
    private float pauseTimer = 0f;
    private float totalTimer = 0f;
    private Quaternion initialRotation;
    private bool isInitialized = false;
    
    public override void Reset()
    {
        base.Reset();
        currentPhase = LookPhase.LookLeft;
        pauseTimer = 0f;
        totalTimer = 0f;
        isInitialized = false;
    }
    
    public override NodeState Evaluate()
    {
        // Only run if in Idle state (after investigation)
        if (detector.CurrentState != Enum_DetectionState.Alerted)
        {
            Reset();
            state = NodeState.Failure;
            return state;
        }
        
        // Initialize on first run
        if (!isInitialized)
        {
            movement.StopMovement();
            initialRotation = movement.GetRotation;
            currentPhase = LookPhase.LookLeft;
            pauseTimer = 0f;
            totalTimer = 0f;
            isInitialized = true;
            
            if (debugMode)
                Debug.Log($"[SO_LookAround] Starting look around sequence from {initialRotation.eulerAngles.y}°");
        }
        
        totalTimer += Time.deltaTime;
        
        // Safety timeout
        if (totalTimer >= timeoutDuration)
        {
            if (debugMode)
                Debug.Log($"[SO_LookAround] Timeout reached, completing");
            
            Reset();
            state = NodeState.Success;
            return state;
        }
        
        // Handle pause between phases
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            state = NodeState.Running;
            return state;
        }
        
        // Execute current phase
        bool phaseComplete = ExecuteCurrentPhase();
        
        if (phaseComplete)
        {
            // Move to next phase
            if (currentPhase == LookPhase.Complete)
            {
                if (debugMode)
                    Debug.Log($"[SO_LookAround] Look around sequence complete");
                
                Reset();
                state = NodeState.Success;
                return state;
            }
            
            // Advance to next phase and pause
            currentPhase++;
            pauseTimer = pauseBetweenLooks;
            
            if (debugMode)
                Debug.Log($"[SO_LookAround] Phase complete, advancing to: {currentPhase}");
        }
        
        state = NodeState.Running;
        return state;
    }
    
    private bool ExecuteCurrentPhase()
    {
        float rotationSpeed = useCustomRotationSpeed 
            ? customRotationSpeed 
            : movement.Settings.searchRotationSpeed;
        
        Quaternion targetRotation;
        float remainingAngle;
        
        switch (currentPhase)
        {
            case LookPhase.LookLeft:
                // Look left from initial direction
                targetRotation = initialRotation * Quaternion.Euler(0, -leftRightLookAngle, 0);
                remainingAngle = movement.RotateTowards(targetRotation, rotationSpeed);
                
                if (debugMode && remainingAngle < movement.Settings.rotationCompleteThreshold)
                    Debug.DrawRay(BT_Entity.transform.position, movement.GetForward * 5f, Color.cyan, pauseBetweenLooks);
                
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.ReturnCenter1:
                // Return to center (initial rotation)
                remainingAngle = movement.RotateTowards(initialRotation, rotationSpeed);
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.LookRight:
                // Look right from initial direction
                targetRotation = initialRotation * Quaternion.Euler(0, leftRightLookAngle, 0);
                remainingAngle = movement.RotateTowards(targetRotation, rotationSpeed);
                
                if (debugMode && remainingAngle < movement.Settings.rotationCompleteThreshold)
                    Debug.DrawRay(BT_Entity.transform.position, movement.GetForward * 5f, Color.yellow, pauseBetweenLooks);
                
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.ReturnCenter2:
                // Return to center again before turning around
                remainingAngle = movement.RotateTowards(initialRotation, rotationSpeed);
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.TurnAround:
                // Turn 180 degrees from initial direction
                targetRotation = initialRotation * Quaternion.Euler(0, 180f, 0);
                remainingAngle = movement.RotateTowards(targetRotation, rotationSpeed);
                
                if (debugMode && remainingAngle < movement.Settings.rotationCompleteThreshold)
                    Debug.DrawRay(BT_Entity.transform.position, movement.GetForward * 5f, Color.red, pauseBetweenLooks);
                
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.LookLeftBack:
                // Look left from the back position (180° - left angle)
                targetRotation = initialRotation * Quaternion.Euler(0, 180f - leftRightLookAngle, 0);
                remainingAngle = movement.RotateTowards(targetRotation, rotationSpeed);
                
                if (debugMode && remainingAngle < movement.Settings.rotationCompleteThreshold)
                    Debug.DrawRay(BT_Entity.transform.position, movement.GetForward * 5f, Color.magenta, pauseBetweenLooks);
                
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.ReturnCenterBack1:
                // Return to back center (180°)
                targetRotation = initialRotation * Quaternion.Euler(0, 180f, 0);
                remainingAngle = movement.RotateTowards(targetRotation, rotationSpeed);
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.LookRightBack:
                // Look right from the back position (180° + right angle)
                targetRotation = initialRotation * Quaternion.Euler(0, 180f + leftRightLookAngle, 0);
                remainingAngle = movement.RotateTowards(targetRotation, rotationSpeed);
                
                if (debugMode && remainingAngle < movement.Settings.rotationCompleteThreshold)
                    Debug.DrawRay(BT_Entity.transform.position, movement.GetForward * 5f, Color.blue, pauseBetweenLooks);
                
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.ReturnCenterBack2:
                // Return to back center again before returning to start
                targetRotation = initialRotation * Quaternion.Euler(0, 180f, 0);
                remainingAngle = movement.RotateTowards(targetRotation, rotationSpeed);
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.ReturnToStart:
                // Return to initial direction
                remainingAngle = movement.RotateTowards(initialRotation, rotationSpeed);
                
                if (debugMode && remainingAngle < movement.Settings.rotationCompleteThreshold)
                    Debug.DrawRay(BT_Entity.transform.position, movement.GetForward * 5f, Color.green, pauseBetweenLooks);
                
                return remainingAngle < movement.Settings.rotationCompleteThreshold;
            
            case LookPhase.Complete:
                return true;
            
            default:
                return false;
        }
    }
}
