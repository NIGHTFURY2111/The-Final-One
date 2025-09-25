using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Component that handles camera FOV changes based on player velocity
/// </summary>
[CreateAssetMenu(fileName = "FOV Effect", menuName = "Scriptable Object/Camera Effects/FOV Effect")]
public class FOV_Effects : ScriptableObject
{
    [Serializable]  
    struct FOVValues
    {
        [Tooltip("velocity Range in which FOV will change")]
        public float2 VelocityThreshold;
        [Tooltip("Maximum additional FOV")]
        public float maxFOVIncrease;
        [Tooltip("Speed of FOV adjustment transition")]
        public float fovChangeSpeed;
        [Tooltip("Animation curve for FOV adjustment")]
        public AnimationCurve velocityToFOVCurve;

        public FOVValues(Vector2 velocityRange, float maxFOVIncrease, AnimationCurve velocityToFOVCurve, float fovChangeSpeed)
        {
            this.VelocityThreshold= velocityRange;
            this.maxFOVIncrease = maxFOVIncrease;
            this.velocityToFOVCurve = velocityToFOVCurve;
            this.fovChangeSpeed = fovChangeSpeed;
        }

        public float CalculateFOV(float currentVelocity, float baseFOV)
        {
            if (currentVelocity > VelocityThreshold.x)
            {
                float velocityRatio = Mathf.Clamp01((currentVelocity - VelocityThreshold.x) / (VelocityThreshold.y - VelocityThreshold.x));
                float curveEvaluated = velocityToFOVCurve.Evaluate(velocityRatio);
                return baseFOV + (maxFOVIncrease * curveEvaluated);
            }
            #region Negative Velocity Handling reduces FOV
            else if (currentVelocity < -VelocityThreshold.x)
            {
                float velocityRatio = Mathf.Clamp01((-currentVelocity - VelocityThreshold.x) / (VelocityThreshold.y - VelocityThreshold.x));
                float curveEvaluated = velocityToFOVCurve.Evaluate(velocityRatio);
                return baseFOV - (maxFOVIncrease * curveEvaluated*0.3f); //30% effect of +ve values keeping this subtle
            }
            #endregion
            return baseFOV;
        }
    }                                           
    
    
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
    [SerializeField] FOVValues fovValues = new FOVValues();
    
    // default values
    private float defaultFOV = 60f;
    private float currentFOV;
    
    public void ComponentAwake(float baseFOV)
    {
        currentFOV = defaultFOV;
        SetBaseFOV(baseFOV);
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