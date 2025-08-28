using System;
using UnityEngine;

/// <summary>
/// Component that handles camera FOV changes based on player velocity
/// </summary>
[CreateAssetMenu(fileName = "FOV Effect", menuName = "Scriptable Object/Camera Effects/FOV Effect")]
public class FOV_Effects : ScriptableObject
{
    // Events for FOV updates
    public event Action<float> OnFOVChangeDirect; // Direct FOV change without animation
    public event Action<float, float> OnFOVChange; // Smooth FOV change with animation

    [Header("Velocity Detection")]
    [SerializeField, Tooltip("Minimum change in velocity magnitude to trigger FOV recalculation")] 
    private float velocityChangeThreshold = 0.1f;
    
    [SerializeField, Tooltip("Use smooth FOV transitions for velocity-based effects (prevents jarring changes)")]
    private bool useSmoothFOVForVelocity = true;
    
    [SerializeField, Tooltip("Duration for smooth FOV transitions (lower = more responsive)")]
    private float smoothFOVDuration = 0.2f;
    
    [Header("FOV Settings")]
    [SerializeField] FOVValues fovValues = new FOVValues(
        5f,   // minVelocityThreshold
        20f,  // maxVelocityThreshold
        15f,  // maxFOVIncrease
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f),  // velocityToFOVCurve
        5f    // fovChangeSpeed
    );
    
    [Serializable]
    struct FOVValues
    {
        [Tooltip("Minimum velocity required to start FOV change")]
        public float minVelocityThreshold;
        [Tooltip("Velocity at which FOV reaches maximum")]
        public float maxVelocityThreshold;
        [Tooltip("Maximum additional FOV")]
        public float maxFOVIncrease;
        [Tooltip("Animation curve for FOV adjustment")]
        public AnimationCurve velocityToFOVCurve;
        [Tooltip("Speed of FOV adjustment transition")]
        public float fovChangeSpeed;

        public FOVValues(float minVelocityThreshold, float maxVelocityThreshold, float maxFOVIncrease, 
                        AnimationCurve velocityToFOVCurve, float fovChangeSpeed)
        {
            this.minVelocityThreshold = minVelocityThreshold;
            this.maxVelocityThreshold = maxVelocityThreshold;
            this.maxFOVIncrease = maxFOVIncrease;
            this.velocityToFOVCurve = velocityToFOVCurve;
            this.fovChangeSpeed = fovChangeSpeed;
        }

        public float CalculateFOV(float currentVelocity, float baseFOV)
        {
            if (currentVelocity > minVelocityThreshold)
            {
                float velocityRatio = Mathf.Clamp01((currentVelocity - minVelocityThreshold) / (maxVelocityThreshold - minVelocityThreshold));
                float curveEvaluated = velocityToFOVCurve.Evaluate(velocityRatio);
                return baseFOV + (maxFOVIncrease * curveEvaluated);
            }
            #region Negative Velocity Handling reduces FOV
            else if (currentVelocity < -minVelocityThreshold)
            {
                float velocityRatio = Mathf.Clamp01((-currentVelocity - minVelocityThreshold) / (maxVelocityThreshold - minVelocityThreshold));
                float curveEvaluated = velocityToFOVCurve.Evaluate(velocityRatio);
                return baseFOV - (maxFOVIncrease * curveEvaluated*0.3f); //30% effect of +ve values keeping this subtle
            }
            #endregion
            return baseFOV;
        }
    }
    
    // default values
    private float defaultFOV = 60f;
    private float currentFOV;
    
    public void ComponentAwake()
    {
        currentFOV = defaultFOV;
    }
    
    public void SetBaseFOV(float cameraBaseFOV)
    {
        defaultFOV = cameraBaseFOV;
        currentFOV = defaultFOV;
    }
    
    public float GetBaseFOV() => defaultFOV;
    public float GetCurrentFOV() => currentFOV;
    
    /// <summary>
    /// Processes velocity changes and triggers FOV updates
    /// </summary>
    public void ProcessVelocity(float currentVelocity)
    {
        float targetFOV = fovValues.CalculateFOV(currentVelocity, defaultFOV);
        
        if (Mathf.Abs(currentFOV - targetFOV) > 0.01f)
        {
            currentFOV = targetFOV;
            
            if (useSmoothFOVForVelocity)
            {
                // Use smooth FOV change to prevent jarring transitions
                OnFOVChange?.Invoke(targetFOV, smoothFOVDuration);
            }
            else
            {
                // Use direct FOV change for immediate response
                OnFOVChangeDirect?.Invoke(targetFOV);
            }
        }
    }
    
    /// <summary>
    /// Manually set FOV with smooth animation
    /// </summary>
    public void SetFOVSmooth(float targetFOV, float duration)
    {
        currentFOV = targetFOV;
        OnFOVChange?.Invoke(targetFOV, duration);
    }
    
    /// <summary>
    /// Manually set FOV directly without animation
    /// </summary>
    public void SetFOVDirect(float targetFOV)
    {
        currentFOV = targetFOV;
        OnFOVChangeDirect?.Invoke(targetFOV);
    }
}