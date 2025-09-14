using System;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class PlayerMovementValues
{
    public Vector3 planeVector { get; private set; }
    public float baseFactor;
    public float overrideFactor;
    public float DecayFactor;
    public AnimationCurve DelayCurve;
    public float clampMaxVelocity = float.NaN;
    public ForceMode forceMode;

    public PlayerMovementValues(float baseFactor, float overrideFactor, AnimationCurve delayCurve, float DecayFactor = default)
    {
        this.planeVector = Vector3.zero;
        this.baseFactor = baseFactor;
        this.overrideFactor = overrideFactor;
        this.DecayFactor = DecayFactor;
        this.DelayCurve = delayCurve;
        this.clampMaxVelocity = float.NaN;
        this.forceMode = ForceMode.Acceleration;
    }
    public PlayerMovementValues(Vector3 planeVector, float baseFactor, float overrideFactor, AnimationCurve delayCurve, float DecayFactor = default, float clampMaxVelocity = float.NaN, ForceMode forceMode = ForceMode.Acceleration)
    {
        this.planeVector = planeVector;
        this.baseFactor = baseFactor;
        this.overrideFactor = overrideFactor;
        this.DecayFactor = DecayFactor;
        this.DelayCurve = delayCurve;
        this.clampMaxVelocity = clampMaxVelocity;
        this.forceMode = forceMode;
    }
    public static PlayerMovementValues zero => new PlayerMovementValues(Vector3.zero, 0f, 0f, AnimationCurve.Linear(0,0,0,0), 0f, float.NaN, ForceMode.Acceleration);

    public PlayerMovementValues UpdateDirection(Vector3 newDirection)
    {
        this.planeVector = newDirection;
        return this;
    }
}

[CreateAssetMenu(fileName = "Player Rigidbody", menuName = "Scriptable Object/Component/Player Rigidbody")]
public class EC_Rigidbody : AC_Component
{
    [Serializable]
    struct WallValues
    {
        public float rotationAngle;
        public float WallRayCastDistance;
        public LayerMask WallMask;
        public WallValues(float rotationAngle, float rayCastDistance, LayerMask layerMask)
        {
            this.rotationAngle = rotationAngle;
            this.WallRayCastDistance = rayCastDistance;
            this.WallMask = layerMask;
        }
    }

    [Serializable]
    struct GroundCheckValues
    {
        public LayerMask GroundLayer;
        public float GCRayLength;
        public float RideHeight;
        public float RideSpringStrength;
        public float RideSpringDamper;
        public GroundCheckValues(LayerMask groundLayer, float rayLength, float rideHeight, float rideSpringStrength, float rideSpringDamper)
        {
            this.GroundLayer = groundLayer;
            this.GCRayLength = rayLength;
            this.RideHeight = rideHeight;
            this.RideSpringStrength = rideSpringStrength;
            this.RideSpringDamper = rideSpringDamper;
        }
    }
    
    [Serializable]
    struct MovementValues
    {
        public float baseSpeed;
        public float maxSpeed;
        public float Speedfactor;
        public float Acceleration;
        public AnimationCurve AccelerationFactorFromDot;
        public float MaxAccel;
        public AnimationCurve MaxAccelerationFactorFromDot;
        public float responsivenessFactor;

        public MovementValues(float baseSpeed, float maxSpeed, float speedFactor, float acceleration, AnimationCurve accelerationFactorFromDot, float maxAccel, AnimationCurve maxAccelerationFactorFromDot, float responsivenessFactor)
        {
            this.baseSpeed = baseSpeed;
            this.maxSpeed = maxSpeed;
            this.Speedfactor = speedFactor;
            this.Acceleration = acceleration;
            this.AccelerationFactorFromDot = accelerationFactorFromDot;
            this.MaxAccel = maxAccel;
            this.MaxAccelerationFactorFromDot = maxAccelerationFactorFromDot;
            this.responsivenessFactor = responsivenessFactor;
        }
    }
    
    #region --- Variables ---
    [SerializeField] float _GRAVITY;
    [SerializeField] public bool _CHECK_GRAVITY = true;
    [SerializeField] Vector3Int playerPlane;
    [SerializeField] float movementDirectionCollisionCheckDistance,radius;

    public  float GRAVITY { get => _GRAVITY;}

    [SerializeField] GroundCheckValues GroundValues;
    [SerializeField] WallValues wallValues;
    [SerializeField] MovementValues movementValues;

    public RaycastHit _groundRayHit, wallHit, movementDirectionCollisionCheck;
    public bool isGrounded { get; private set; }
    public float lastGrounded{ get; private set; }
    public bool isWall{ get; private set; }
    
    bool updateGoalVel = false;
    float appliedGravity;
    float currentGoalSpeedFactor = 0f;
    float StateSwitchTime;
    Vector3 m_GoalVel = Vector3.zero;
    float m_JumpVel = 0f;
    Rigidbody _RB;
    CapsuleCollider collider;
    public override Enum_ComponentType componentType => Enum_ComponentType.RigidBody;

    #endregion
    public override void ComponentAwake()
    {
        _RB = entity.GetComponent<Rigidbody>();
        collider = entity.GetComponent<CapsuleCollider>();
        setGravity(GRAVITY);
    }
    
    public override void ComponentStart(){}
    
    public override void ComponentUpdate()
    {
        ST_debug.Log(lastGrounded.ToString("F2"));
        ST_debug.Log(_RB.velocity.ToString("F2"));
        ST_debug.Log(PlayerPlaneVel.magnitude.ToString("F2"));
        CheckWallHit();
    }
    
    public override void ComponentFixedUpdate()
    {
        CheckGrounded();
        PlayerGravityhandler(_groundRayHit);
    }

    private void PlayerGravityhandler(RaycastHit _rayHit)
    {
        if (!isGrounded || !_CHECK_GRAVITY)
        {
            ApplyGravity(PlayerDown);
            return;
        }

        //ApplySpringPull(_rayHit);
        #region depreciate this into a generic function, mentioned below
        Rigidbody other = _rayHit.rigidbody;
        Vector3 otherVel = other != null ? other.velocity : Vector3.zero;

        float rayDirVel = Vector3.Dot(PlayerDown, PlayerVelocity);
        float otherDirVel = Vector3.Dot(PlayerDown, otherVel);

        float relativeVel = rayDirVel - otherDirVel;

        //float x = Vector3.Distance(_RB.transform.position,SpringPoint) - GroundValues.RideHeight;
        // Alternative to _rayHit.distance

        float x = _rayHit.distance - GroundValues.RideHeight;
        float springForce = (x * GroundValues.RideSpringStrength) - (relativeVel * GroundValues.RideSpringDamper);

        _RB.AddForce(PlayerDown * springForce);

        Debug.DrawLine(_RB.transform.position, _RB.transform.position + (PlayerDown * GroundValues.GCRayLength), Color.yellow);

        if (other != null)
        {
            other.AddForceAtPosition(PlayerDown * -springForce, _rayHit.point);
        }

        #endregion
    }

    public void ApplySpringPull(RaycastHit _rayHit) => ApplySpringPull(_rayHit.rigidbody, -_rayHit.normal, _rayHit.point);
    public void ApplySpringPull(Rigidbody otherRB, Vector3 springDirection, Vector3 SpringPoint)
    {
        springDirection = springDirection.normalized;
        Rigidbody other = otherRB;
        Vector3 otherVel = other != null ? other.velocity : Vector3.zero;

        float rayDirVel = Vector3.Dot(springDirection, PlayerVelocity);
        float otherDirVel = Vector3.Dot(springDirection, otherVel);

        float relativeVel = rayDirVel - otherDirVel;

        //float x = Vector3.Distance(_RB.transform.position,SpringPoint) - GroundValues.RideHeight;
        // Alternative to _rayHit.distance
        Vector3 toPoint = SpringPoint - _RB.transform.position;
        float perpendicularDistance = Vector3.Dot(toPoint, springDirection);
        float x = perpendicularDistance - GroundValues.RideHeight;
        float springForce = (x * GroundValues.RideSpringStrength) - (relativeVel * GroundValues.RideSpringDamper);

        _RB.AddForce(springDirection * springForce);

        Debug.DrawLine(_RB.transform.position, _RB.transform.position + (springDirection * GroundValues.GCRayLength), Color.yellow);

        if (other != null)
        {
            other.AddForceAtPosition(springDirection * -springForce, SpringPoint);
        }
    }

    public void ApplyGravity(Vector3 dir)
    {
        if (PlayerDownVelocity < appliedGravity)
            _RB.AddForce(dir * appliedGravity, ForceMode.Acceleration);
        else
            PlayerVelocity = PlayerPlaneVel + (PlayerDown * appliedGravity);
    }

    public void Jump(AnimationCurve jumpCurve, float jumpHeight, ForceMode forceMode)
    {
        // time since jump started (StateSwitchTime is already set in UpdateGoalVel)
        float t = Time.time - StateSwitchTime;
        
        // --- 1. Evaluate desired height from curve ---
        float targetHeight = jumpCurve.Evaluate(t) * jumpHeight;

        // --- 2. Get baseline from when jump started ---
        float groundY = Vector3.Dot(_groundRayHit.point, PlayerUp);   // spring-defined contact
        float currentY = Vector3.Dot(PlayerVelocity, PlayerUp);

        m_JumpVel = Mathf.MoveTowards(m_JumpVel, (groundY + jumpHeight) * movementValues.Speedfactor, targetHeight);
        // --- 3. Desired vertical velocity to reach curve height this frame ---
        float desiredVelY = m_JumpVel - currentY;
        //float desiredVelY = (groundY+targetHeight - (currentY)) / Time.fixedDeltaTime;

        // --- 4. Apply delta as an impulse to steer velocity toward curve ---
        float deltaVelY = desiredVelY - Vector3.Dot(PlayerVelocity, PlayerUp) * movementValues.responsivenessFactor;
        Vector3 correction = PlayerUp * desiredVelY;

        _RB.AddForce(correction, forceMode);
    }

    public void Move(PlayerMovementValues value) => Move(value, true);
    public void Move(PlayerMovementValues value, bool ChangeInputToLocalSpace)
    {
        // Get the movement vector in local space
        if (ChangeInputToLocalSpace)
                value.UpdateDirection(DirectionRespectiveToPlayer(value.planeVector));
        MoveInPlayerPlane(value);
    }

    public Vector3 DirectionRespectiveToPlayer(Vector2 moveVector, bool AccountForZeroMagnitude = false)
    {
        if (AccountForZeroMagnitude && moveVector.magnitude == 0)
        {
            return PlayerForward.normalized; // Default to forward direction if no input
        }
        return _RB.transform.TransformDirection(new Vector3(moveVector.x,0f,moveVector.y));
    }

    public void MoveInPlayerPlane(PlayerMovementValues value)
    {
        //convert movement into a usable value
        Vector3 moveOnPlane = Vector3.Scale(value.planeVector, playerPlane);
        Debug.DrawRay(_RB.transform.position, moveOnPlane * movementDirectionCollisionCheckDistance, Color.blue);
        Vector3 wallHittingVel = GetWallSlideVector(moveOnPlane);
        
        ST_debug.DrawSphere(
            wallHittingVel.Equals(Vector3.zero) ? 
            _RB.transform.position + moveOnPlane * movementDirectionCollisionCheckDistance: 
            movementDirectionCollisionCheck.point,
            radius,
            Color.white);

        moveOnPlane = wallHittingVel.Equals(Vector3.zero)? moveOnPlane : Vector3.ProjectOnPlane(moveOnPlane, movementDirectionCollisionCheck.normal);
        
        //getting new goalVel
        Vector3 unitVel = m_GoalVel.normalized;
        float velDot = Vector3.Dot(moveOnPlane.normalized, unitVel);
        float accel = movementValues.Acceleration * movementValues.AccelerationFactorFromDot.Evaluate(velDot);

        //calcuate new actual goal
        currentGoalSpeedFactor = updateGoalVel ?  CalculateNewGoalVel(value.baseFactor, value.overrideFactor):
                                            decayVelocity(value.baseFactor, value.DecayFactor, value.DelayCurve);
        Vector3 goalVel = moveOnPlane * movementValues.baseSpeed * currentGoalSpeedFactor * movementValues.Speedfactor;

        m_GoalVel = Vector3.MoveTowards(m_GoalVel, goalVel, accel);

        //neededVel
        Vector3 neededAccel = m_GoalVel - (PlayerPlaneVel * movementValues.responsivenessFactor);

        float maxAccel = movementValues.MaxAccel * movementValues.MaxAccelerationFactorFromDot.Evaluate(velDot);

        //clamping the neededAccel
        neededAccel = Vector3.ClampMagnitude(neededAccel, (value.clampMaxVelocity == float.NaN) ? maxAccel : value.clampMaxVelocity);
        
        //applying the force to the player
        _RB.AddForce(neededAccel, value.forceMode);
    }

    public Vector3 GetWallSlideVector(Vector3 checkingDir)
    {
        if (Physics.SphereCast
            (_RB.transform.position,
            radius,
            checkingDir,
            out movementDirectionCollisionCheck,
            movementDirectionCollisionCheckDistance))
        {
            return Vector3.ProjectOnPlane(checkingDir, movementDirectionCollisionCheck.normal);
        }
        else
        {
            return Vector3.zero;
        }
    }

    public void ApplyForce(Vector3 force, ForceMode forceMode = ForceMode.Force) => _RB.AddForce(force, forceMode);
    public void UpdateGoalVel()
    {
        updateGoalVel = true;
        StateSwitchTime = Time.time;
    }
    float CalculateNewGoalVel(float basefactor, float newFactor)
    {
        updateGoalVel = false;
        if (PlayerPlaneVel.magnitude <= movementValues.baseSpeed * basefactor)
        {
            return basefactor;
        }
        else
        {
            return currentGoalSpeedFactor * newFactor;
        }
    }

    float decayVelocity(float basefactor, float DecayFactor, AnimationCurve delayCurve)
    {
        float TimedFactor = Mathf.Lerp(currentGoalSpeedFactor, basefactor, 1- delayCurve.Evaluate(Time.time - StateSwitchTime));
        ST_debug.Log((Time.time - StateSwitchTime).ToString());
        
        if (currentGoalSpeedFactor <= basefactor * 1.1f)
        {
            return basefactor;
        }
        else
        {
            return TimedFactor;
        }
    }

    void CheckWallHit()
    {
        Vector3[] dirList = new Vector3[]
        {
        Quaternion.AngleAxis(wallValues.rotationAngle, PlayerUp) * PlayerRight,
        Quaternion.AngleAxis(-wallValues.rotationAngle, PlayerUp) * PlayerRight
        };
        foreach (Vector3 direction in dirList)
        {
            isWall = Physics.Raycast(_RB.transform.position, direction, out wallHit, wallValues.WallRayCastDistance, wallValues.WallMask)
                || Physics.Raycast(_RB.transform.position, -direction, out wallHit, wallValues.WallRayCastDistance, wallValues.WallMask);
            Debug.DrawRay(_RB.transform.position, direction * wallValues.WallRayCastDistance, isWall ? Color.red : Color.green);
            Debug.DrawRay(_RB.transform.position, -direction * wallValues.WallRayCastDistance, isWall ? Color.red : Color.green);
            if (isWall) break;
        }
    }
    
    void CheckGrounded()
    {
        bool tempCheck = Physics.SphereCast(
            _RB.transform.position,
            collider.radius * 0.5f,
            PlayerDown,
            out _groundRayHit,
            collider.height * 0.5f + GroundValues.GCRayLength,
            GroundValues.GroundLayer
        );
        if (isGrounded)
        {
            lastGrounded = Time.time;
        }
        isGrounded = tempCheck;
    }
    public void RotatePlayer(Vector2 Rotation) 
    {
        _RB.MoveRotation(_RB.rotation * Quaternion.Euler(0, Rotation.x, 0))/*_RB.transform.rotation *= Quaternion.Euler(0, Rotation.x, 0)*/;
    }

    public void setGravity(float gravity) => appliedGravity = gravity;
    public void MoveInSpecifiedDirection(Vector3 moveVector, float moveSpeed) => PlayerVelocity = moveVector * moveSpeed;
    public Transform PlayerTransform => _RB.transform;
    public Vector3 PlayerForward => _RB.transform.forward;
    public Vector3 PlayerRight => _RB.transform.right;
    public Vector3 PlayerUp => _RB.transform.up;
    public float PlayerDownVelocity => Vector3.Dot(PlayerVelocity, PlayerDown);
    public Vector3 PlayerVelocity { get => _RB.velocity; set => _RB.velocity = value; }
    public Vector3 PlayerPlaneVel { get => Vector3.Scale(PlayerVelocity, playerPlane);}
    Vector3 PlayerDown => _RB.transform.TransformDirection(Vector3.down);
}