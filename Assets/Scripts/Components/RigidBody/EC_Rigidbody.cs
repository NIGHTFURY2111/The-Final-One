using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Player Rigidbody", menuName = "Scriptable Object/Component/Player Rigidbody")]
public class EC_Rigidbody : AC_Component
{/*TODO: make them into classes like:
  * 
  *     [System.Serializable]
    public class GeneralSettings
    {
        public float maxFallingVelocity;
        public Playerinput playerMovement;
        public Transform groundCheck;
        public LayerMask groundLayer;
    }

    [SerializeField] GeneralSettings generalSettings;

  */
    #region --- Variables ---
    [SerializeField] float _GRAVITY;
    [SerializeField] public bool _CHECK_GRAVITY = true;
    [SerializeField] LayerMask Ground;
    [SerializeField] float GCRayLength;
    [SerializeField] float RideHeight;
    [SerializeField] float RideSpringStrength;
    [SerializeField] float RideSpringDamper;
    [SerializeField] Vector3Int playerPlane;


    [SerializeField] float baseSpeed; 
    [SerializeField] float maxSpeed;
    [SerializeField] float Speedfactor;
    [SerializeField] float Acceleration;
    [SerializeField] AnimationCurve AccelerationFactorFromDot;
    [SerializeField] float MaxAccel;
    [SerializeField] AnimationCurve MaxAccelerationFactorFromDot;
    [SerializeField] float responsivenessFactor;

    [SerializeField] LayerMask WallMask;




    //[SerializeField] float RBSpeedFactor;

    
    float appliedGravity;
    Vector3 m_GoalVel = Vector3.zero;
    float currentGoalSpeed = 0f;
    bool updateGoalVel = false;

    public  float GRAVITY { get => _GRAVITY; }
    public bool isGrounded { get; private set; }
    public bool isWall{ get; private set; }
    Rigidbody _RB;
    CapsuleCollider collider;
    public RaycastHit hit;

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
        isGrounded =  Physics.SphereCast(
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
        if (isGrounded && _CHECK_GRAVITY)
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



    public void Move(Vector3 moveVector,float baseFactor, float overrideFactor) => Move(moveVector, baseFactor, overrideFactor, true);
    public void Move(Vector3 moveVector,float baseFactor, float overrideFactor, bool ChangeInputToLocalSpace)
    {
        // Get the movement vector in local space
        if (ChangeInputToLocalSpace)

        MoveInPlayerPlane
                (DirectionRespectiveToPlayer(moveVector),baseFactor, overrideFactor);

        else
        MoveInPlayerPlane(moveVector,baseFactor, overrideFactor);
    }


    public void AirMovement(Vector2 moveDirection,float basefactor, float overrideFactor)
    {
        Vector3 move = DirectionRespectiveToPlayer(moveDirection);
        //Debug.Log("AirMovement: " + move + " | " + moveDirection);  
        //clamping speed
        MoveInPlayerPlane(move, basefactor, overrideFactor, forceMode:ForceMode.Acceleration);

    }


    public void MoveInSpecifiedDirection(Vector3 moveVector, float moveSpeed)
    {
        PlayerVelocity = moveVector * moveSpeed;
    }

    public Vector3 DirectionRespectiveToPlayer(Vector2 moveVector, bool AccountForZeroMagnitude = false)
    {
        if (AccountForZeroMagnitude && moveVector.magnitude == 0)
        {
            return PlayerForward.normalized; // Default to forward direction if no input
        }
        return _RB.transform.TransformDirection(new Vector3(moveVector.x,0f,moveVector.y));
    }





    public void MoveInPlayerPlane(Vector3 planeVector,float baseFactor, float overrideFactor, float maxAccelFactor = 1f, float clampMaxVelocity = float.NaN, ForceMode forceMode = ForceMode.Acceleration)
    {

        //convert movement into a usable value
        Vector3 moveOnPlane = Vector3.Scale(planeVector, playerPlane);

        //getting new goalVel
        Vector3 unitVel =m_GoalVel.normalized;
        float velDot = Vector3.Dot(moveOnPlane.normalized, unitVel);
        float accel = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);

        //calcuate new actual goal
        if(updateGoalVel)       CalculateNewGoalVel(baseFactor, overrideFactor);
        Vector3 goalVel = moveOnPlane* currentGoalSpeed *Speedfactor;

        m_GoalVel = Vector3.MoveTowards(m_GoalVel, goalVel, accel);


        //neededVel
        Vector3 neededAccel = m_GoalVel - (PlayerPlaneVel * responsivenessFactor);


        //dividing by deltaTime is the issue, the Video is dividing by soemthing else enitrely, ignore this for now
        //Vector3 neededAccel = (moveOnPlane - Vector3.Scale(PlayerVelocity, playerPlane)) / Time.deltaTime;  
        float maxAccel = MaxAccel * MaxAccelerationFactorFromDot.Evaluate(velDot) * maxAccelFactor;

        //clamping the neededAccel
        neededAccel = Vector3.ClampMagnitude(neededAccel, (clampMaxVelocity == float.NaN) ? maxAccel: clampMaxVelocity);
        ST_debug.displayString = PlayerPlaneVel.magnitude.ToString("F2");
        
        //applying the force to the player
        _RB.AddForce(neededAccel, forceMode);


        //PlayerVelocity = moveOnPlane + downVelocity;
    }

    public void UpdateGoalVel()
    {
        updateGoalVel = true;
    }
    void CalculateNewGoalVel(float basefactor, float overrideFactor)
    {//update TS its not boosting the speed
        if (PlayerPlaneVel.magnitude < baseSpeed * basefactor)
        {
            currentGoalSpeed = baseSpeed * basefactor;
        }
        else if (PlayerPlaneVel.magnitude > baseSpeed * basefactor)
        {
            currentGoalSpeed = PlayerPlaneVel.magnitude * overrideFactor;
        }
        updateGoalVel = false;
    }

    //bool CheckWallHit (float rotationAngle, float rayCastDistance)
    //{
    //    Vector3[] dirList = new Vector3[]
    //    {
    //    Quaternion.AngleAxis(rotationAngle, PlayerUp) * PlayerRight,
    //    Quaternion.AngleAxis(-rotationAngle, PlayerUp) * PlayerRight
    //    };
    //    foreach (Vector3 direction in dirList)
    //    {
    //        isWall = Physics.Raycast(_RB.transform.position,direction,out hit,rayCastDistance,WallMask)
    //            || Physics.Raycast(_RB.transform.position,-direction,out hit,rayCastDistance,WallMask);
    //    }
    //}



    public Vector3 PlayerForward => _RB.transform.forward;
    public Vector3 PlayerRight => _RB.transform.right;
    public Vector3 PlayerUp => _RB.transform.up;
    public Vector3 PlayerVelocity { get => _RB.velocity; set => _RB.velocity = value; }
    public Vector3 PlayerPlaneVel { get => Vector3.Scale(PlayerVelocity, playerPlane);}
           Vector3 PlayerDown => _RB.transform.TransformDirection(Vector3.down);
}
