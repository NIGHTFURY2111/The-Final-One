using UnityEngine;
using DG.Tweening;
using System;

public class ShiftingPlatform : MonoBehaviour
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

    [Header("Recorded Transforms")]
    [SerializeField] private TransformData originalTransform = new TransformData();
    [SerializeField] private TransformData targetTransform = new TransformData();

    [Header("Movement Settings")]
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;

    private Tween activeTween;

    private void OnDisable() => activeTween?.Kill();
    private void OnDestroy() => activeTween?.Kill();

    public void RecordOriginalTransform()
    {
        originalTransform = new TransformData(transform);
    }

    public void RecordTargetTransform()
    {
        targetTransform = new TransformData(transform);
    }

    public void PreviewOriginalTransform()
    {
        activeTween?.Kill();
        originalTransform.ApplyTo(transform);
    }

    public void PreviewTargetTransform()
    {
        activeTween?.Kill();
        targetTransform.ApplyTo(transform);
    }

    public void ShiftToTarget() => MoveTo(targetTransform);
    public void ShiftToOriginal() => MoveTo(originalTransform);

    private void MoveTo(TransformData target)
    {
        activeTween?.Kill();

        activeTween = DOTween.Sequence()
            .Append(transform.DOMove(target.position, moveDuration))
            .Join(transform.DORotateQuaternion(target.rotation, moveDuration))
            .Join(transform.DOScale(target.scale, moveDuration))
            .SetEase(easeType)
            .SetUpdate(UpdateType.Fixed);
    }
}
