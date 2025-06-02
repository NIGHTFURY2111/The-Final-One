using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Rigidbody", menuName = "Scriptable Object/Component/Player Rigidbody")]
public class EC_Rigidbody : AC_Component
{
    [SerializeField]LayerMask Ground;
    [SerializeField]float gravity;
    
    public bool isgrounded { get; private set; }
    Rigidbody rb;
    CapsuleCollider collider;
    public override ComponentType componentType => ComponentType.RigidBody;

    public override void ComponentAwake()
    {
        rb = entity.GetComponent<Rigidbody>();
        collider = entity.GetComponent<CapsuleCollider>();
    }

    public override void ComponentStart()
    {
    }

    public override void ComponentUpdate()
    {
        ApplyGravity();
        isGrounded();
    }

    public void Move(Vector3 velocity)
    {
        rb.velocity = velocity;
    }

    void isGrounded()
    {
        isgrounded = Physics.Raycast(rb.transform.position, Vector3.down, collider.height * 0.5f + 0.2f, Ground);
    }

    public void ApplyGravity()
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    public void RotateGameObject(Vector2 Rotation)
    {
        rb.transform.rotation *= Quaternion.Euler(0, Rotation.x, 0);
    }
    public void Jump( float force)
    {
        rb.velocity = (rb.transform.up * force);
    }

    public void Move(Vector2 moveVector)
    {
        
        rb.velocity = rb.transform.forward * moveVector.y + rb.transform.right * moveVector.x;
        //if (isgrounded)
        //{
        //    rb.velocity = moveDirection * rb.velocity.magnitude;
        //}
        //else
        //{
        //    rb.velocity = new Vector3(moveDirection.x * rb.velocity.magnitude, rb.velocity.y, moveDirection.z * rb.velocity.magnitude);
        //}
    }
}
