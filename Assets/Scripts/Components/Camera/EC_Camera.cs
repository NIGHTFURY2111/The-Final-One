using UnityEngine;
using DG.Tweening;

/// <summary>
/// Component responsible for camera operations like rotation and applying FOV changes
/// </summary>
[CreateAssetMenu(fileName = "Player Camera", menuName = "Scriptable Object/Component/Player Camera")]
public class EC_Camera : AC_Component
{
    [Header("Camera Settings")]
    [SerializeField, Tooltip("Maximum angle the camera can look up or down")]
    private float lookXLimit = 45f;

    [SerializeField, Tooltip("FOV Effects Controller")]
    private FOV_Effects fovController;
    [SerializeField, Tooltip("HeadBob Controller")]
    private Headbob_Effect Headbob;
    
    // Camera state
    private float rotationX = 0f;
    public float defaultFOV { get; private set; }
    public float currentFOV { get; private set; }

    // Camera reference
    public Camera camera { get; private set; }
    
    // Track if a FOV tween is currently active to prevent conflicts
    private Tween currentFOVTween;

    public override Enum_ComponentType componentType => Enum_ComponentType.Camera;

    private void OnEnable()
    {
        if (camera == null)
        {
        camera = Camera.main;
        }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
    }
    
    public override void ComponentAwake()
    {
        // Get camera reference and cache default FOV
        if (camera == null) camera = Camera.main;
        currentFOV = defaultFOV = camera.fieldOfView;

        #region FOV initialization
        // Initialize FOV controller if present
        if (fovController != null)
        {
            fovController.ComponentAwake();
            fovController.SetBaseFOV(defaultFOV);
            
            // Connect FOV events
            fovController.OnFOVChangeDirect += SetCameraFOVDirect;
            fovController.OnFOVChange += UpdateCameraFOVSmooth;
        }
        #endregion
        Headbob.Initialize(camera.transform.parent);
    }

    public override void ComponentStart() { }
    public override void ComponentUpdate() { }
    public override void ComponentFixedUpdate() { }

    /// <summary>
    /// Updates the camera's rotation based on input
    /// </summary>
    public void UpdateCameraTransform(Vector2 rotation)
    {
        rotationX += -rotation.y;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    #region FOV Management
    /// <summary>
    /// Sets the camera FOV directly without animation - used for velocity-based FOV
    /// </summary>
    public void SetCameraFOVDirect(float newFOV)
    {
        // Kill any existing FOV tween to prevent conflicts
        KillCurrentFOVTween();

        // Update state and apply immediately
        currentFOV = newFOV;
        if (camera != null)
        {
            camera.fieldOfView = newFOV;
        }
    }

    /// <summary>
    /// Smoothly transitions the camera FOV with DOTween - used for special effects
    /// </summary>
    public void UpdateCameraFOVSmooth(float newFOV, float duration = 0.01f)
    {
        // Kill any existing FOV tween to prevent conflicts
        KillCurrentFOVTween();

        // Update state and apply with animation
        currentFOV = newFOV;
        if (camera != null)
        {
            currentFOVTween = camera.DOFieldOfView(newFOV, duration);
        }
    }

    /// <summary>
    /// Processes velocity for FOV effects - called from PlayerEntity
    /// </summary>
    public void ProcessVelocityForFOV(float velocity)
    {
        fovController?.ProcessVelocity(velocity);
    }

    private void KillCurrentFOVTween()
    {
        if (currentFOVTween != null && currentFOVTween.IsActive())
        {
            currentFOVTween.Kill();
        }
    }
    #endregion
    private void OnDisable()
    {
        // Clean up
        KillCurrentFOVTween();
        
        // Unsubscribe from FOV controller events
        if (fovController != null)
        {
            fovController.OnFOVChangeDirect -= SetCameraFOVDirect;
            fovController.OnFOVChange -= UpdateCameraFOVSmooth;
        }

        Headbob.terminate();
    }
}
