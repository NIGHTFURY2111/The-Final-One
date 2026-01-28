using UnityEngine;
using DG.Tweening;

/// <summary>
/// Platform that continuously moves between two points in a loop.
/// Each instance has its own unique transform data.
/// Supports ping-pong and loop modes.
/// </summary>
[AddComponentMenu("Platform/Cyclic Platform")]
public class EC_CyclicPlatform : AC_PlatformBehaviour
{
    public enum CycleMode
    {
        PingPong,   // A -> B -> A -> B
        Loop        // A -> B -> (instant snap to A) -> B
    }

    [Header("Recorded Transforms")]
    [SerializeField] private TransformData pointA = new TransformData();
    [SerializeField] private TransformData pointB = new TransformData();

    [Header("Cycle Settings")]
    [SerializeField] private CycleMode cycleMode = CycleMode.PingPong;
    [SerializeField] private float waitTimeAtPoints = 0.5f;
    [SerializeField] private bool waitAtPointA = true;
    [SerializeField] private bool startImmediately = true;
    [SerializeField] private bool startAtPointA = true;

    protected virtual void Start()
    {
        // Set initial position
        if (startAtPointA)
            ApplyTransformImmediate(pointA);
        else
            ApplyTransformImmediate(pointB);

        if (startImmediately)
        {
            StartCycle();
        }
    }

    #region Recording Methods

    public void RecordPointA()
    {
        pointA = new TransformData(transform);
    }

    public void RecordPointB()
    {
        pointB = new TransformData(transform);
    }

    #endregion

    #region Preview Methods (Editor)

    public void PreviewPointA()
    {
        ApplyTransformImmediate(pointA);
    }

    public void PreviewPointB()
    {
        ApplyTransformImmediate(pointB);
    }

    #endregion

    #region Cycle Control

    public void StartCycle()
    {
        if (activeSequence != null && activeSequence.IsActive()) return;
        
        CreateCycleSequence();
    }

    public void StopCycle()
    {
        activeSequence?.Kill();

        rb.velocity = Vector3.zero;
    }

    public void PauseCycle()
    {
        activeSequence?.Pause();
    }

    public void ResumeCycle()
    {
        activeSequence?.Play();
    }

    public void ResetToPointA()
    {
        StopCycle();
        ApplyTransformImmediate(pointA);
    }

    #endregion

    #region Movement Logic

    private void CreateCycleSequence()
    {
        activeSequence?.Kill();
        
        activeSequence = DOTween.Sequence();

        if (cycleMode == CycleMode.PingPong)
        {
            // Move to Point B
            AddMovementToSequence(pointB);
            
            // Wait at Point B
            if (waitTimeAtPoints > 0)
                activeSequence.AppendInterval(waitTimeAtPoints);
            
            // Move back to Point A
            AddMovementToSequence(pointA);
            
            // Wait at Point A (optional)
            if (waitTimeAtPoints > 0 && waitAtPointA)
                activeSequence.AppendInterval(waitTimeAtPoints);
            
            // Loop infinitely
            activeSequence.SetLoops(-1, LoopType.Restart);
        }
        else // Loop mode
        {
            // Move to Point B
            AddMovementToSequence(pointB);
            
            // Wait at Point B
            if (waitTimeAtPoints > 0)
                activeSequence.AppendInterval(waitTimeAtPoints);
            
            // Snap back to Point A (instant)
            activeSequence.AppendCallback(() => 
            {
                pointA.ApplyTo(transform);
                rb.velocity = Vector3.zero;
            });
            
            // Wait at Point A before next loop (optional)
            if (waitTimeAtPoints > 0 && waitAtPointA)
                activeSequence.AppendInterval(waitTimeAtPoints);
            
            // Loop infinitely
            activeSequence.SetLoops(-1, LoopType.Restart);
        }
    }

    private void AddMovementToSequence(TransformData target)
    {
        // Position tween with physics velocity
        activeSequence.Append(
            DOTween.To(() => rb.position, x => 
            {
                Vector3 velocity = (x - rb.position) / Time.fixedDeltaTime;
                rb.velocity = velocity;
            }, target.position, moveDuration)
            .SetEase(easeType)
            .SetUpdate(UpdateType.Fixed)
        );

        // Rotation tween (runs simultaneously with position)
        activeSequence.Join(
            transform.DORotateQuaternion(target.rotation, moveDuration)
            .SetEase(easeType)
        );

        // Scale tween (runs simultaneously with position)
        activeSequence.Join(
            transform.DOScale(target.scale, moveDuration)
            .SetEase(easeType)
        );
        
        // Stop velocity at end of movement
        activeSequence.AppendCallback(() => 
        {
            if (rb != null) rb.velocity = Vector3.zero;
        });
    }

    #endregion

    protected override void OnDisable()
    {
        StopCycle();
        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        StopCycle();
        base.OnDestroy();
    }
}
