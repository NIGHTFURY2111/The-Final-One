using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Object/Input/Input Reader")]
public class SO_InputReader : ScriptableObject
{
    [SerializeField] float defaultBufferTime;
    private InputBuffer _InputBuffer;
    public InputBuffer InputBuffer => _InputBuffer;

    private PlayerInput _playerInput;

    public void Initialize(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        _InputBuffer ??= new();

        // Subscribe to the specific actions provided by this PlayerInput instance
        _playerInput.onActionTriggered += OnActionTriggered;
    }

    private void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.onActionTriggered -= OnActionTriggered;
        }
    }

    private void OnActionTriggered(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        // Route inputs based on action name to maintain existing buffer logic
        switch (actionName)
        {
            case "Camera":
            case "Movement":
                StoreInputVector2(context);
                break;
            case "Jump":
            case "Dash":
            case "OpenMenu":
            case "Grapple":
            case "GrappleHold":
            case "HatThrowDynamic":
            case "HatThrowStatic":
            case "Shoot":
            case "Slide":
                StoreInputBool(context);
                break;
        }
    }

    // --- Input Storage Methods ---
    void StoreInputBool(InputAction.CallbackContext context, float bufferTime = float.NaN)
    {
        _InputBuffer.AddInput(context.action, context.action.WasPressedThisFrame(), float.IsNaN(bufferTime) ? defaultBufferTime : bufferTime);
    }

    void StoreInputVector2(InputAction.CallbackContext context)
    {
        _InputBuffer.AddInput(context.action, context.ReadValue<Vector2>());
    }

    public void DisableInput() => _playerInput?.actions.Disable();
    public void EnableInput() => _playerInput?.actions.Enable();
}
