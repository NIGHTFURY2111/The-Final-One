using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[CreateAssetMenu(fileName = "Player Rigidbody", menuName = "Scriptable Object/Component/Player Rigidbody")]
public class EC_Rigidbody : AC_Component
{
    #region --- Variables ---
    [SerializeField] float _GRAVITY;
    [SerializeField] public bool _CHECK_GRAVITY = true;
    [SerializeField] LayerMask Ground;
    [SerializeField] float GCRayLength;
    [SerializeField] float RideHeight;
    [SerializeField] float RideSpringStrength;
    [SerializeField] float RideSpringDamper;




    [SerializeField] float RBMaxSpeed;
    [SerializeField] float RBSpeedFactor;

    public  float GRAVITY { get => _GRAVITY; }
            float appliedGravity;
    public bool isgrounded { get; private set; }
    Rigidbody _RB;
    CapsuleCollider collider;

    #endregion
    public override Enum_ComponentType componentType => Enum_ComponentType.RigidBody;

    public override void ComponentAwake()
    {
        _RB = entity.GetComponent<Rigidbody>();
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
        GroundCheckAndGravity();
    }

    void GroundCheckAndGravity()
    {
        //isgrounded = Physics.Raycast(rb.transform.position, Vector3.down, collider.height * 0.5f + 0.2f, Ground);

        //isgrounded = Physics.Raycast(
        //    _RB.transform.position,
        //    PlayerDown,
        //    out RaycastHit _rayHit,
        //    collider.height * 0.5f + GCRayLength,
        //    Ground
        //);

        isgrounded =  Physics.SphereCast(
            _RB.transform.position,
            collider.radius * 0.5f,
            PlayerDown,
            out RaycastHit _rayHit,
            collider.height * 0.5f + GCRayLength,
            Ground
        );

        PlayerGravityhandler(_rayHit);

    }

    private void PlayerGravityhandler(RaycastHit _rayHit)
    {
        if (isgrounded && _CHECK_GRAVITY)
        {
            Vector3 vel = _RB.velocity;

            Rigidbody other = _rayHit.rigidbody;
            Vector3 otherVel = other != null ? other.velocity : Vector3.zero;

            float rayDirVel = Vector3.Dot(PlayerDown, vel);
            float otherDirVel = Vector3.Dot(PlayerDown, otherVel);

            float relativeVel = rayDirVel - otherDirVel;

            float x = _rayHit.distance - RideHeight;
            float springForce = (x * RideSpringStrength) - (relativeVel * RideSpringDamper);

            _RB.AddForce(PlayerDown * springForce);

            Debug.DrawLine(_RB.transform.position, _RB.transform.position + (PlayerDown * GCRayLength), Color.yellow);

            if (other != null)
            {
                other.AddForceAtPosition(PlayerDown * -springForce, _rayHit.point);
            }
        }
        else
        {
            ApplyGravity(PlayerDown);
        }
    }

    public void ApplyGravity(Vector3 dir)
    {
        _RB.AddForce(dir * appliedGravity);
    }

    public void RotatePlayer(Vector2 Rotation)
    {
        _RB.transform.rotation *= Quaternion.Euler(0, Rotation.x, 0);
    }
    public void Jump(float force)
    {
        _RB.AddForce (_RB.transform.up * force, ForceMode.Impulse);
    }

    public void Move(Vector2 moveVector)
    {
        // Get the movement vector in local space
        Vector3 move = DirectionRespectiveToPlayer(moveVector);

        MoveInPlayerPlane(move);
    }

    public void AirMovement(Vector2 moveDirection,float VelOnEnter, float maxVelAddition)
    {
        Vector3 move = DirectionRespectiveToPlayer(moveDirection);

        _RB.AddForce(move, ForceMode.Force);


        //clamping speed
        MoveInPlayerPlane(PlayerVelocity, VelOnEnter + maxVelAddition);

    }


    public void MoveInSpecifiedDirection(Vector3 moveVector)
    {
        PlayerVelocity = moveVector;
    }

    public Vector3 DirectionRespectiveToPlayer(Vector2 moveVector, bool AccountForZeroMagnitude = false)
    {
        if (AccountForZeroMagnitude && moveVector.magnitude == 0)
        {
            return PlayerForward.normalized; // Default to forward direction if no input
        }
        return (PlayerForward * moveVector.y + PlayerRight * moveVector.x);
    }


    public void MoveInPlayerPlane(Vector3 planeVector, float clampMaxVelocity = float.NaN)
    {
        //moves player without changing anything related to the gravity axis
        // Project movement onto the plane perpendicular to DownDir (removes any DownDir component)
        Vector3 moveOnPlane = Vector3.ProjectOnPlane(planeVector, PlayerDown);

        // Preserve the current velocity along DownDir
        float downSpeed = Vector3.Dot(_RB.velocity, PlayerDown);
        Vector3 downVelocity = PlayerDown * downSpeed;

        // Combine planar movement with preserved DownDir velocity

        if(clampMaxVelocity != float.NaN)
        {
            moveOnPlane = moveOnPlane.normalized * Mathf.Clamp(moveOnPlane.magnitude, 0, clampMaxVelocity);
        }

        PlayerVelocity = moveOnPlane + downVelocity;
    }

    public Vector3 PlayerForward => _RB.transform.forward;
    public Vector3 PlayerRight => _RB.transform.right;
    public Vector3 PlayerVelocity { get => _RB.velocity; set => _RB.velocity = value; }
           Vector3 PlayerDown => _RB.transform.TransformDirection(Vector3.down);
}
