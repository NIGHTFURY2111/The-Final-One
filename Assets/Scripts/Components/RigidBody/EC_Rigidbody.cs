using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[CreateAssetMenu(fileName = "Player Rigidbody", menuName = "Scriptable Object/Component/Player Rigidbody")]
public class EC_Rigidbody : AC_Component
{
    [SerializeField]LayerMask Ground;
    [SerializeField]float _GRAVITY;
    public float GRAVITY { get => _GRAVITY; }
    float appliedGravity;
    public bool isgrounded { get; private set; }
    Rigidbody rb;
    CapsuleCollider collider;
    public override Enum_ComponentType componentType => Enum_ComponentType.RigidBody;

    public override void ComponentAwake()
    {
        rb = entity.GetComponent<Rigidbody>();
        collider = entity.GetComponent<CapsuleCollider>();
        setGravity(GRAVITY);
    }

    public void setGravity(float gravity)
    {
        appliedGravity = gravity;
    }

    public override void ComponentStart()
    {
    }

    public override void ComponentUpdate()
    {
        ApplyGravity();
        isGrounded();
    }

    void isGrounded()
    {
        isgrounded = Physics.Raycast(rb.transform.position, Vector3.down, collider.height * 0.5f + 0.2f, Ground);
    }

    public void ApplyGravity()
    {
        rb.AddForce(Vector3.down * appliedGravity, ForceMode.Acceleration);
    }

    public void RotatePlayer(Vector2 Rotation)
    {
        rb.transform.rotation *= Quaternion.Euler(0, Rotation.x, 0);
    }
    public void Jump( float force)
    {
        rb.velocity += (rb.transform.up * force);
    }

    public void Move(Vector2 moveVector)
    {    
        rb.velocity = DirectionRespectiveToPlayer(moveVector);
    }

    public void MoveInSpecifiedDirection(Vector3 moveVector)
    {
        rb.velocity = moveVector;
    }

    public Vector3 DirectionRespectiveToPlayer(Vector2 moveVector, bool AccountForZeroMagnitude = false)
    {
        if (AccountForZeroMagnitude && moveVector.magnitude == 0)
        {
            return PlayerForward.normalized; // Default to forward direction if no input
        }
        return (PlayerForward * moveVector.y + PlayerRight * moveVector.x);
    }

    public Vector3 PlayerForward => rb.transform.forward;
    public Vector3 PlayerRight => rb.transform.right;
}
