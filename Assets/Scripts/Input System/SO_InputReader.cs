using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Object/Input/Input Reader")]
public class SO_InputReader : ScriptableObject, PlayerInputAction.IPlayerActions
{
    [SerializeField] float defaultBufferTime;
    private InputBuffer _InputBuffer;
    public InputBuffer InputBuffer => _InputBuffer;

    PlayerInputAction input;
    private void OnEnable()
    {
        if (input == null)
        {
            input = new PlayerInputAction();
            input.Player.SetCallbacks(this);
        }
        input.Player.Enable();

        if (_InputBuffer == null)
            _InputBuffer = new();
    }

    private void OnDisable()
    {
        if (input != null) input.Player.Disable();
    }

    // --- Input Callbacks ---

    public void OnCamera(InputAction.CallbackContext context) => StoreInputVector2(context);
    public void OnMovement(InputAction.CallbackContext context) => StoreInputVector2(context);

    public void OnJump(InputAction.CallbackContext context) => StoreInputBool(context, defaultBufferTime);

    public void OnDash(InputAction.CallbackContext context) => StoreInputBool(context, defaultBufferTime);

    public void OnGrapple(InputAction.CallbackContext context) => StoreInputBool(context);
    public void OnGrappleHold(InputAction.CallbackContext context) => StoreInputBool(context);
    public void OnHatThrowDynamic(InputAction.CallbackContext context) => StoreInputBool(context);
    public void OnHatThrowStatic(InputAction.CallbackContext context) => StoreInputBool(context);
    public void OnShoot(InputAction.CallbackContext context) => StoreInputBool(context);
    public void OnSlide(InputAction.CallbackContext context) => StoreInputBool(context);


    // --- Input Storage Methods ---
    void StoreInputBool(InputAction.CallbackContext context, float bufferTime = float.NaN)
    {
            //there is no actual way to check if the input is a bool or not thanks to unity in all its glory
            _InputBuffer.AddInput(context.action, context.action.WasPressedThisFrame(), bufferTime == float.NaN ? defaultBufferTime : bufferTime);
    }

    void StoreInputVector2(InputAction.CallbackContext context, float bufferTime = float.NaN)
    {
        _InputBuffer.AddInput(context.action, context.ReadValue<Vector2>());
    }
}
