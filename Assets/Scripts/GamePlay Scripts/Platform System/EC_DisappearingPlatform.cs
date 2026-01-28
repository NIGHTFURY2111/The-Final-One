using UnityEngine;
using DG.Tweening;

/// <summary>
/// Platform that disappears after the player leaves it.
/// Each instance has its own unique state and settings.
/// Can respawn after a delay and supports visual feedback.
/// </summary>
[AddComponentMenu("Platform/Disappearing Platform")]
public class EC_DisappearingPlatform : AC_PlatformBehaviour
{
    [Header("Disappearing Settings")]
    [SerializeField] private float disappearDelay = 0.5f;
    [SerializeField] private float disappearDuration = 0.3f;
    [SerializeField] private bool respawn = true;
    [SerializeField] private float respawnDelay = 3f;

    [Header("Visual Feedback")]
    [SerializeField] private bool useScaleAnimation = true;
    [SerializeField] private bool useFadeAnimation = true;
    [SerializeField] private AnimationCurve disappearCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField] private Color warningColor = new Color(1f, 0.5f, 0f, 1f); // Orange
    [SerializeField] private float warningFlashSpeed = 5f;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask playerLayer = -1;
    [SerializeField] private float detectionDistance = 0.5f;
    [SerializeField] private Vector3 detectionBoxSize = new Vector3(1f, 0.2f, 1f);

    private bool isPlayerOn = false;
    private bool isDisappeared = false;
    private float disappearTimer = 0f;
    private Vector3 originalScale;
    private MeshRenderer platformRenderer;
    private Collider platformCollider;
    private MaterialPropertyBlock propertyBlock;
    private Color originalColor;

    protected override void Awake()
    {
        base.Awake();
        
        originalScale = transform.localScale;
        platformRenderer = GetComponent<MeshRenderer>();
        platformCollider = GetComponent<Collider>();
        
        if (platformRenderer != null && useFadeAnimation)
        {
            propertyBlock = new MaterialPropertyBlock();
            platformRenderer.GetPropertyBlock(propertyBlock);
            if (propertyBlock.isEmpty)
            {
                originalColor = platformRenderer.material.color;
            }
            else
            {
                originalColor = propertyBlock.GetColor("_Color");
            }
        }
    }

    private void Update()
    {
        if (isDisappeared) return;

        CheckPlayerPresence();

        if (isPlayerOn)
        {
            disappearTimer += Time.deltaTime;

            // Warning visual feedback
            if (disappearTimer > 0 && disappearTimer < disappearDelay)
            {
                ShowWarning();
            }

            if (disappearTimer >= disappearDelay)
            {
                Disappear();
            }
        }
    }

    private void CheckPlayerPresence()
    {
        // Check if player is on platform using box cast
        Vector3 checkPosition = transform.position + transform.up * detectionDistance;
        bool wasPlayerOn = isPlayerOn;
        
        isPlayerOn = Physics.CheckBox(
            checkPosition,
            detectionBoxSize * 0.5f,
            transform.rotation,
            playerLayer
        );

        // If player just left, start disappear sequence
        if (wasPlayerOn && !isPlayerOn && disappearTimer > 0)
        {
            Disappear();
        }

        // Reset timer if player gets back on quickly
        if (!isPlayerOn)
        {
            disappearTimer = 0f;
        }
    }

    private void ShowWarning()
    {
        if (platformRenderer != null && useFadeAnimation)
        {
            // Flash between original color and warning color
            float t = Mathf.PingPong(Time.time * warningFlashSpeed, 1f);
            Color flashColor = Color.Lerp(originalColor, warningColor, t);
            
            propertyBlock.SetColor("_Color", flashColor);
            platformRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void Disappear()
    {
        if (isDisappeared) return;

        isDisappeared = true;
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        // Disable collision immediately
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }

        // Scale animation
        if (useScaleAnimation)
        {
            activeSequence.Append(
                transform.DOScale(Vector3.zero, disappearDuration)
                .SetEase(disappearCurve)
            );
        }

        // Fade animation
        if (useFadeAnimation && platformRenderer != null)
        {
            activeSequence.Join(
                DOTween.To(
                    () => 1f,
                    alpha =>
                    {
                        Color fadedColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                        propertyBlock.SetColor("_Color", fadedColor);
                        platformRenderer.SetPropertyBlock(propertyBlock);
                    },
                    0f,
                    disappearDuration
                ).SetEase(disappearCurve)
            );
        }

        // Handle respawn
        if (respawn)
        {
            activeSequence.AppendInterval(respawnDelay);
            activeSequence.AppendCallback(Respawn);
        }
    }

    private void Respawn()
    {
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        // Re-enable collision
        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }

        // Scale animation
        if (useScaleAnimation)
        {
            activeSequence.Append(
                transform.DOScale(originalScale, disappearDuration)
                .SetEase(Ease.OutBack)
            );
        }
        else
        {
            transform.localScale = originalScale;
        }

        // Fade animation
        if (useFadeAnimation && platformRenderer != null)
        {
            activeSequence.Join(
                DOTween.To(
                    () => 0f,
                    alpha =>
                    {
                        Color fadedColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                        propertyBlock.SetColor("_Color", fadedColor);
                        platformRenderer.SetPropertyBlock(propertyBlock);
                    },
                    1f,
                    disappearDuration
                ).SetEase(Ease.OutQuad)
            );
        }
        else if (platformRenderer != null)
        {
            propertyBlock.SetColor("_Color", originalColor);
            platformRenderer.SetPropertyBlock(propertyBlock);
        }

        // Reset state
        isDisappeared = false;
        disappearTimer = 0f;
        isPlayerOn = false;
    }

    #region Public Control Methods

    /// <summary>
    /// Manually trigger disappear
    /// </summary>
    public void TriggerDisappear()
    {
        Disappear();
    }

    /// <summary>
    /// Manually trigger respawn
    /// </summary>
    public void TriggerRespawn()
    {
        if (isDisappeared)
        {
            Respawn();
        }
    }

    /// <summary>
    /// Reset the platform to initial state
    /// </summary>
    public void ResetPlatform()
    {
        activeSequence?.Kill();
        
        if (platformCollider != null)
            platformCollider.enabled = true;
            
        transform.localScale = originalScale;
        
        if (platformRenderer != null)
        {
            propertyBlock.SetColor("_Color", originalColor);
            platformRenderer.SetPropertyBlock(propertyBlock);
        }

        isDisappeared = false;
        disappearTimer = 0f;
        isPlayerOn = false;
    }

    #endregion

    protected override void OnDisable()
    {
        ResetPlatform();
        base.OnDisable();
    }

    // Draw gizmos for detection area
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Vector3 checkPosition = transform.position + transform.up * detectionDistance;
        Gizmos.matrix = Matrix4x4.TRS(checkPosition, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, detectionBoxSize);
    }
}
