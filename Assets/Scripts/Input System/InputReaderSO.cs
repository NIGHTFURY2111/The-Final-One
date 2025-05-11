using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Object/Input/Input Reader")]
public class InputReaderSO : ScriptableObject, PlayerInputAction.IPlayerActions
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

    public void OnCamera(InputAction.CallbackContext context)
    {
        StoreInputVector2(context);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        StoreInputBool(context);
    }

    public void OnGrapple(InputAction.CallbackContext context)
    {
        StoreInputBool(context);
    }

    public void OnGrappleHold(InputAction.CallbackContext context)
    {
        StoreInputBool(context);
    }

    public void OnHatThrowDynamic(InputAction.CallbackContext context)
    {
        StoreInputBool(context);
    }

    public void OnHatThrowStatic(InputAction.CallbackContext context)
    {
        StoreInputBool(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        StoreInputBool(context);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        StoreInputVector2(context);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        StoreInputFloat(context);
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        StoreInputBool(context);
    }

    void StoreInputBool(InputAction.CallbackContext context, float bufferTime = float.NaN)
    {
        if (context.action.WasPerformedThisFrame())
        {
            //there is no actual way to check if the input is a bool or not thanks to unity in all its glory
            _InputBuffer.AddInput(context.action.name, context.performed, bufferTime == float.NaN ? defaultBufferTime : bufferTime);
        }
    }

    void StoreInputFloat(InputAction.CallbackContext context, float bufferTime = float.NaN)
    {
        if (context.action.WasPerformedThisFrame())
        {
            _InputBuffer.AddInput(context.action.name, context.ReadValue<float>(), bufferTime == float.NaN ? defaultBufferTime : bufferTime);
        }
    }

    void StoreInputVector2(InputAction.CallbackContext context, float bufferTime = float.NaN)
    {
        _InputBuffer.AddInput(context.action.name, context.ReadValue<Vector2>(), bufferTime == float.NaN ? defaultBufferTime : bufferTime);
    }
}
