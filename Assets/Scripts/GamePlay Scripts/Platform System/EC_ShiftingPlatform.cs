using UnityEngine;

/// <summary>
/// Platform that shifts between two recorded positions when triggered.
/// Each instance has its own unique transform data.
/// Can be triggered programmatically or via Unity Events.
/// </summary>
[AddComponentMenu("Platform/Shifting Platform")]
public class EC_ShiftingPlatform : AC_PlatformBehaviour
{
    [Header("Recorded Transforms")]
    [SerializeField] private TransformData originalTransform = new TransformData();
    [SerializeField] private TransformData targetTransform = new TransformData();

    [Header("Auto-Start Settings")]
    [SerializeField] private bool autoShiftOnStart = false;
    [SerializeField] private bool shiftToTargetOnStart = true;

    protected virtual void Start()
    {
        if (autoShiftOnStart)
        {
            if (shiftToTargetOnStart)
                ShiftToTarget();
            else
                ShiftToOriginal();
        }
    }

    #region Recording Methods (Editor)

    /// <summary>
    /// Records the current platform transform as the original position
    /// </summary>
    public void RecordOriginalTransform()
    {
        originalTransform = new TransformData(transform);
    }

    /// <summary>
    /// Records the current platform transform as the target position
    /// </summary>
    public void RecordTargetTransform()
    {
        targetTransform = new TransformData(transform);
    }

    #endregion

    #region Preview Methods (Editor)

    /// <summary>
    /// Immediately applies the original transform (for editor preview)
    /// </summary>
    public void PreviewOriginalTransform()
    {
        ApplyTransformImmediate(originalTransform);
    }

    /// <summary>
    /// Immediately applies the target transform (for editor preview)
    /// </summary>
    public void PreviewTargetTransform()
    {
        ApplyTransformImmediate(targetTransform);
    }

    #endregion

    #region Shift Methods

    /// <summary>
    /// Shifts the platform to the target transform
    /// </summary>
    public void ShiftToTarget()
    {
        MoveTo(targetTransform,StopPlatformMovement);
    }

    public void StopPlatformMovement()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Shifts the platform to the original transform
    /// </summary>
    public void ShiftToOriginal()
    {
        MoveTo(originalTransform, StopPlatformMovement);
    }

    /// <summary>
    /// Toggles between original and target transforms
    /// </summary>
    public void Toggle()
    {
        // Determine which transform we're closer to
        float distToOriginal = Vector3.Distance(transform.position, originalTransform.position);
        float distToTarget = Vector3.Distance(transform.position, targetTransform.position);

        if (distToOriginal < distToTarget)
            ShiftToTarget();
        else
            ShiftToOriginal();
    }

    #endregion
}
