using UnityEngine;
using DG.Tweening;

/// <summary>
/// Component responsible for camera operations like rotation and applying FOV changes
/// </summary>
[CreateAssetMenu(fileName = "Player Camera", menuName = "Scriptable Object/Component/Player Camera")]
public class EC_Camera : AC_Component
{
    #region --- Variables ---
    [Header("Effect References")]

    [SerializeField, Tooltip("FOV Effects Controller")]
    private FOV_Effects fovController;

    [SerializeField, Tooltip("HeadBob Controller")]
    private Headbob_Effect Headbob;

    [SerializeField, Tooltip("camera Tilt Controller")]
    private CameraTilt cameraTilt;

    [Header("Camera Settings")]

    [SerializeField, Tooltip("Maximum angle the camera can look up or down")]
    private float lookXLimit = 45f;    
    
    [SerializeField, Tooltip("position of the camera when crouched")]
    private Vector3 CrouchCamPosition;
    

    // Camera state
    private float rotationX = 0f;
    public float defaultFOV { get; private set; }
    public float currentFOV { get; private set; }

    // Camera reference
    public Camera camera { get; private set; }
    public GameObject cameraParent => camera != null ? camera.transform.parent.gameObject : null;

    // Track if a tween is currently active to prevent conflicts
    private Tween currentFOVTween,crouchTween;

    #endregion
    public override Enum_ComponentType componentType => Enum_ComponentType.Camera;

    public override void ComponentAwake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get camera reference and cache default FOV
        if (camera == null) camera = Camera.main;

        if (Headbob != null)Headbob.Initialize(camera.transform.parent);
        #region FOV initialization
        // Initialize FOV controller if present
        if (fovController != null)
        {
            defaultFOV = camera.fieldOfView;
            fovController.ComponentAwake(defaultFOV);
            // Connect FOV events
            fovController.OnFOVChangeDirect += SetCameraFOVDirect;
            fovController.OnFOVChange += UpdateCameraFOVSmooth;
        }
        currentFOV = defaultFOV = camera.fieldOfView;
        #endregion
        
        if (cameraTilt != null) cameraTilt.initialize(camera.gameObject);
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
        cameraParent.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
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

    /// <summary> moves the camera to a crouch position or back to normal using dotween</summary>
    private void KillCurrentFOVTween()
    {
        if (currentFOVTween != null && currentFOVTween.IsActive())
        {
            currentFOVTween.Kill();
        }
    }

    #endregion

    #region Crouching Camera function
    public void crouchCameraPosition(bool isCrouched) 
    {
        KillCurrentCrouchTween();
        Vector3 targetPosition = isCrouched ? CrouchCamPosition : Vector3.zero;
        crouchTween = camera.transform.DOLocalMove(targetPosition, 0.1f);
    }
    private void KillCurrentCrouchTween()
    {
        if (crouchTween != null && crouchTween.IsActive())
        {
            crouchTween.Kill();
        }
    }

    #endregion

    public override void ComponentDisable()
    {
        // Clean up
        KillCurrentFOVTween();
        KillCurrentCrouchTween();

        // Unsubscribe from FOV controller events
        if (fovController != null)
        {
            fovController.OnFOVChangeDirect -= SetCameraFOVDirect;
            fovController.OnFOVChange -= UpdateCameraFOVSmooth;
        }

        if (Headbob != null)
        Headbob.terminate();
        Debug.Log("Camera Component Disabled");
        Destroy(this);
    }
}
