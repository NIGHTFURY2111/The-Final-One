using System;
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

[Serializable]
public struct SpringValues
{
    public LayerMask SpringHitLayers;

    public float RideHeight;
    public float RideSpringStrength;
    public float RideSpringDamper;
    public SpringValues(LayerMask groundLayer, float rideHeight, float rideSpringStrength, float rideSpringDamper)
    {
        SpringHitLayers = groundLayer;
        RideHeight = rideHeight;
        RideSpringStrength = rideSpringStrength;
        RideSpringDamper = rideSpringDamper;
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

    [Header("Rigidbody specific Values")]
    [SerializeField] SO_detector detector;
    [SerializeField] Vector3Int playerPlane;
    [SerializeField] float moveDirCollisionCheckDistance,radius;
    [SerializeField] public bool _CHECK_GRAVITY = true;
    [SerializeField] float _GRAVITY;


    [Header("Struct Values")]
    [SerializeField] SpringValues GroundValues;
    [SerializeField] WallValues wallValues;
    [SerializeField] MovementValues movementValues;


    public RaycastHit _groundRayHit, wallHit, movementDirectionCollisionCheck;
    public  float GRAVITY { get => _GRAVITY;}
    float appliedGravity;

    bool updateGoalVel = false;
    float currentGoalSpeedFactor = 0f;
    float StateSwitchTime;

    Vector3 m_GoalVel = Vector3.zero;
    float m_JumpVel = 0f;

    Rigidbody _RB;
    public override Enum_ComponentType componentType => Enum_ComponentType.RigidBody;

    #endregion
    public override void ComponentAwake()
    {
        _RB = entity.GetComponent<Rigidbody>();
        setGravity(GRAVITY);
    }
    public override void ComponentStart(){}
    
    public override void ComponentUpdate()
    {
        ST_debug.Log($"last Grounded: {detector.lastGrounded.ToString("F2")} " +
            $"\nTotal vel: {_RB.velocity.ToString("F2")} " +
            $"\nplane Vel: {PlayerPlaneVel.magnitude.ToString("F2")}");
    }
    
    public override void ComponentFixedUpdate()
    {
        PlayerGravityhandler(_groundRayHit);
    }

    private void PlayerGravityhandler(RaycastHit _rayHit)
    {
        if (!detector.isGrounded || !_CHECK_GRAVITY)
        {
            ApplyGravity(PlayerDown);
            return;
        }

        ApplySpringPull(detector._groundRayHit);
    }

    #region --- Spring and Gravity Physics ---
    public void ApplySpringPull(RaycastHit _rayHit) => ApplySpringPull(_rayHit.rigidbody, -_rayHit.normal, _rayHit.point,GroundValues);
    public void ApplySpringPull(Rigidbody otherRB, Vector3 springDirection, Vector3 SpringPoint, SpringValues GroundValues)
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

        //Debug.DrawLine(_RB.transform.position, _RB.transform.position + (springDirection * GroundValues.GCRayLength), Color.yellow);

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

    #endregion

    public void Jump(AnimationCurve jumpCurve, float jumpHeight, ForceMode forceMode)
    {
        // time since jump started (StateSwitchTime is already set in UpdateGoalVel)
        float t = Time.time - StateSwitchTime;
        
        // --- 1. Evaluate desired height from curve ---
        float targetHeight = jumpCurve.Evaluate(t) * jumpHeight;

        // --- 2. Get baseline from when jump started ---
        //float groundY = Vector3.Dot(_groundRayHit.point, PlayerUp);   // spring-defined contact
        float currentY = Vector3.Dot(PlayerVelocity, PlayerUp);

        m_JumpVel = Mathf.MoveTowards(m_JumpVel, (jumpHeight) * movementValues.Speedfactor, targetHeight);
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
                value.UpdateDirection(DirectionInLocalSpace(value.planeVector));
        MoveInPlayerPlane(value);
    }

    public Vector3 DirectionInLocalSpace(Vector2 moveVector, bool AccountForZeroMagnitude = false)
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
        Debug.DrawRay(_RB.transform.position, moveOnPlane * moveDirCollisionCheckDistance, Color.blue);
        Vector3 wallHittingVel = GetWallSlideVector(moveOnPlane);
        
        ST_debug.DrawSphere(
            wallHittingVel.Equals(Vector3.zero) ? 
            _RB.transform.position + moveOnPlane * moveDirCollisionCheckDistance: 
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
            moveDirCollisionCheckDistance))
        {
            return Vector3.ProjectOnPlane(checkingDir, movementDirectionCollisionCheck.normal);
        }
        else
        {
            return Vector3.zero;
        }
    }

    #region Velocity  modifiers
    public void UpdateGoalVel()
    {
        updateGoalVel = true;
        StateSwitchTime = Time.time;
    }
    float CalculateNewGoalVel(float basefactor, float newFactor)
    {
        updateGoalVel = false;

         return (PlayerPlaneVel.magnitude <= movementValues.baseSpeed * basefactor)?
                basefactor:
                (currentGoalSpeedFactor * newFactor);
    }
    float decayVelocity(float basefactor, float DecayFactor, AnimationCurve delayCurve)
    {
        ST_debug.Log("Last switched:"+(Time.time - StateSwitchTime).ToString());

        return  (currentGoalSpeedFactor <= basefactor * 1.1f)?
                basefactor:
                Mathf.Lerp(currentGoalSpeedFactor, basefactor, 1 - delayCurve.Evaluate(Time.time - StateSwitchTime));
    }

    #endregion

    public void RotatePlayer(Vector2 Rotation) 
    {
        _RB.MoveRotation(_RB.rotation * Quaternion.Euler(0, Rotation.x, 0));
    }

    public void ApplyForce(Vector3 force, ForceMode forceMode = ForceMode.Force) => _RB.AddForce(force, forceMode);
    public void setGravity(float gravity) => appliedGravity = gravity;
    public void OverrideVelocity(Vector3 moveVector, float moveSpeed) => PlayerVelocity = moveVector * moveSpeed;
    public override void ComponentDisable() { }

    public Transform PlayerTransform => _RB.transform;
    public Vector3 PlayerForward => _RB.transform.forward;
    public Vector3 PlayerRight => _RB.transform.right;
    public Vector3 PlayerUp => _RB.transform.up;
    public float PlayerDownVelocity => Vector3.Dot(PlayerVelocity, PlayerDown);
    public Vector3 PlayerVelocity { get => _RB.velocity; set => _RB.velocity = value; }
    public Vector3 PlayerPlaneVel { get => Vector3.Scale(PlayerVelocity, playerPlane);}
    Vector3 PlayerDown => _RB.transform.TransformDirection(Vector3.down);
}