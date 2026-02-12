using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Input Accessor", menuName = "Scriptable Object/Input/Input Accessors")]

public class SO_InputAccess : AC_Component<SO_InputAccess>
{
    public SO_InputReader ReadInput;
    InputBuffer buffer => (ReadInput != null) ? ReadInput.InputBuffer : null;

    public override Enum_ComponentType componentType => Enum_ComponentType.Input;

    public bool ConsumeInput(string ActionName) => buffer != null && buffer.MarkInputAsUsed(ActionName);

    public string ListInputs() => buffer != null ? buffer.List() : "Buffer Null";


    #region Movement
    public bool Movement( out object value ) 
    {
        value = null;
        return buffer != null && buffer.GetInput("Movement", out value);
    }

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
        value = Vector2.zero;
        if (buffer == null) return false;

        bool output = buffer.GetInput("Camera", out object val);
        value = val.IsUnityNull() ? Vector2.zero : (Vector2)val;
        return output;
    }

    public bool Camera() => Camera(out Vector2 value);
    #endregion

    #region Jump
    public bool Jump( out object value ) 
    {
        value = null;
        return buffer != null && buffer.GetInput("Jump", out value);
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
        value = null;
        return buffer != null && buffer.GetInputValueThisFrame("Dash", out value);
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
        value = null;
        return buffer != null && buffer.GetInputValueThisFrame("Slide", out value);

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
        value = null;
        return buffer != null && buffer.GetInput("Grapple", out value);

    }

    public bool Grapple()
    {
        return Grapple(out object _);
    }
    #endregion

    #region Shoot
    public bool Shoot(out InputAction value)
    {
        value = null;
        return buffer != null && buffer.GetInputValueThisFrame("Shoot", out value);

    }

    public bool Shoot()
    {
        Shoot(out InputAction val);
        return val != null && val.WasPressedThisFrame();
    }

    #endregion

    #region escape
    public bool Pause(out InputAction value)
    {
        value = null;
        return buffer != null && buffer.GetInputValueThisFrame("OpenMenu", out value);

    }

    public bool Pause()
    {
        Pause(out InputAction val);
        return val != null && val.WasPressedThisFrame();
    }

    #endregion
    public override void ComponentAwake() { }

    public void Initialize(PlayerInput playerInput)
    {
        if (ReadInput != null)
        {
            // Clone the reader asset so this player has a private instance
            ReadInput = Instantiate(ReadInput);
            ReadInput.Initialize(playerInput);
        }
    }

    public override void ComponentStart() { }

    public override void ComponentUpdate() { }

    public override void ComponentDisable() { Destroy(this); }


}
