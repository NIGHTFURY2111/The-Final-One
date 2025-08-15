using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Input Accessor", menuName = "Scriptable Object/Input/Input Accessors")]

public class SO_InputAccess : ScriptableObject
{
    public SO_InputReader ReadInput;
    InputBuffer buffer => ReadInput.InputBuffer;
    public bool ConsumeInput(string ActionName) => buffer.MarkInputAsUsed(ActionName);

    public string ListInputs() => buffer.List();


    #region Movement
    public bool Movement( out object value ) => buffer.GetInput("Movement", out value);

    //public bool Movement()
    //{
    //    Movement(out object val);
    //    return val != null && !val.Equals(Vector2.zero);
    //}
    public Vector2 Movement()
    {
        Movement(out object val);
        if (val == null)
            return Vector2.zero;
        
        if (((Vector2)val).magnitude > 1f)
            return ((Vector2)val).normalized;

        return (Vector2)val;
    }
    #endregion

    #region Camera
    public bool Camera( out Vector2 value ) 
    {
        bool output = buffer.GetInput("Camera", out object val);
        value = val.IsUnityNull() ? Vector2.zero : (Vector2)val;
        return output;
    }

    public bool Camera() => Camera(out Vector2 value);
    #endregion

    #region Jump
    public bool Jump( out object value ) 
    {
        bool output = buffer.GetInput("Jump", out value);
        return output;

    }

    public bool Jump()
    {
        Jump(out object val);
        return val!=null && (bool)val;
    }
    #endregion

    #region Dash
    public bool Dash( out InputAction value ) 
    {
        bool output = buffer.GetInputValueThisFrame("Dash", out value);
        return output;

    }

    public bool Dash()
    {
        Dash(out InputAction val);

        return val!=null && val.WasPressedThisFrame();
    }
    #endregion

    #region Slide
    public bool Slide( out InputAction value ) 
    {
        bool output = buffer.GetInputValueThisFrame("Slide", out value);
        return output;

    }

    public bool Slide()
    {
        Slide(out InputAction val);
        return val != null && val.IsInProgress();
    }
    #endregion

    #region Grapple
    public bool Grapple( out object value ) 
    {
        bool output = buffer.GetInput("Grapple", out value);
        return output;

    }

    public bool Grapple()
    {
        return Grapple(out object _);
    }
    #endregion

    #region Shoot
    public bool Shoot(out InputAction value)
    {
        bool output = buffer.GetInputValueThisFrame("Shoot", out value);
        return output;

    }

    public bool Shoot()
    {
        Shoot(out InputAction val);
        return val != null && val.WasPressedThisFrame();
    }

    #endregion

}
