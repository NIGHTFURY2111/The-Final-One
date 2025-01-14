using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputActionParser : MonoBehaviour
{
    [SerializeField] TMP_Text  taxt;
    [SerializeField] float  defaultBufferTime;
    public InputBuffer<object> InputBuffer;
    private void Awake()
    {
        InputBuffer = new InputBuffer<object> ();
    }
    void StoreInputBool(InputAction.CallbackContext recievedValue)
    {
        if (recievedValue.action.WasPerformedThisFrame())
        { InputBuffer.AddInput(
            recievedValue.action.name, 
            recievedValue.ReadValue<bool>(), 
            defaultBufferTime); }
    }    
    void StoreInputFloat(InputAction.CallbackContext recievedValue)
    {
        if (recievedValue.action.WasPerformedThisFrame())
        { InputBuffer.AddInput(
            recievedValue.action.name,
            recievedValue.ReadValue<float>(),
            defaultBufferTime); }
    }    
    void StoreInputVector2(InputAction.CallbackContext recievedValue)
    {
        InputBuffer.AddInput(
            recievedValue.action.name,
            recievedValue.ReadValue<Vector2>(),
            defaultBufferTime);
    }

    private void LateUpdate()
    {
        taxt.text = InputBuffer.List();
    }


    void _()
    {

    }




}







