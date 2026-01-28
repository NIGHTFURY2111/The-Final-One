using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// Abstract base class for platform behaviors.
/// Each platform instance has its own component with unique data.
/// Follows the modular component pattern used throughout the project.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class AC_PlatformBehaviour : MonoBehaviour
{
    [Serializable]
    public class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale = Vector3.one;

        public TransformData() { }

        public TransformData(Transform t)
        {
            position = t.position;
            rotation = t.rotation;
            scale = t.localScale;
        }

        public void ApplyTo(Transform t)
        {
            t.SetPositionAndRotation(position, rotation);
            t.localScale = scale;
        }
    }

    [Header("Movement Settings")]
    [SerializeField] protected float moveDuration = 1f;
    [SerializeField] protected Ease easeType = Ease.InOutQuad;

    protected Rigidbody rb;
    protected DG.Tweening.Sequence activeSequence;

    protected virtual void Awake()
    {
        if (!TryGetComponent(out rb))
        {
            Debug.LogError($"Platform {gameObject.name} requires a Rigidbody component!");
        }
    }

    protected virtual void OnDisable()
    {
        activeSequence?.Kill();
    }

    protected virtual void OnDestroy()
    {
        activeSequence?.Kill();
    }

    /// <summary>
    /// Moves the platform to a target transform data with position, rotation, and scale
    /// </summary>
    protected void MoveTo(TransformData target, System.Action onComplete = null)
    {
        activeSequence?.Kill();
        
        // Create a sequence that tweens position, rotation, and scale together
        activeSequence = DOTween.Sequence();
        
        // Position tween with velocity application for physics
        activeSequence.Append(DOTween.To(() => rb.position, x => 
        {
            Vector3 velocity = (x - rb.position) / Time.fixedDeltaTime;
            rb.velocity = velocity;
        }, target.position, moveDuration)
        .SetEase(easeType)
        .SetUpdate(UpdateType.Fixed));
        
        // Rotation tween
        activeSequence.Join(transform.DORotateQuaternion(target.rotation, moveDuration)
        .SetEase(easeType));
        
        // Scale tween
        activeSequence.Join(transform.DOScale(target.scale, moveDuration)
        .SetEase(easeType));

        if (onComplete != null)
        {
            activeSequence.OnComplete(() => onComplete());
        }
    }

    /// <summary>
    /// Immediately applies transform data without animation
    /// </summary>
    protected void ApplyTransformImmediate(TransformData data)
    {
        activeSequence?.Kill();
        data.ApplyTo(transform);
    }
}
