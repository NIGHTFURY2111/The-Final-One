using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "Wall Run", menuName = "Scriptable Object/State Machine/Wall Run")]

public class SO_WallRun : AC_BaseState
{
    [Header("Wall Run Settings")]
    [SerializeField] float minWallRunVelocity = 5f;    // Minimum velocity to enter wall run
    [SerializeField] float wallRunSpeed = 8f;          // Speed along the wall
    [SerializeField] float wallRunDuration = 2f;       // Maximum wall run time
    [SerializeField] float wallStickForce = 5f;        // Force sticking player to wall
    [SerializeField] AnimationCurve wallRunCurve;      // Speed curve over time
    /*    [SerializeField] float upwardForce = 2f;    */       // Slight upward force to counteract gravity
    [SerializeField] SpringValues WallSpringValues;
    [SerializeField] CameraTilt cameraTilt;


    private float wallRunTimer = 0f;
    private Vector3 wallNormal;
    private Vector3 wallPoint;
    private Vector3 wallRunDirection;
    private PlayerMovementValues wallRunValue;
    private Collider wallCollider;

    public SO_WallRun(EC_Movement ctx) : base(ctx)
    {
    }

    public override void EnterState()
    {
        p_Rigidbody._CHECK_GRAVITY = false;
        p_Rigidbody.setGravity(0f);
        //p_Rigidbody.OverrideVelocity(p_Rigidbody.PlayerPlaneVel, 1f);
        wallRunTimer = 0f;

        // Store wall normal and calculate run direction
        wallNormal = p_Detector.wallHit.normal;
        wallCollider = p_Detector.wallHit.collider;
        wallPoint = wallCollider.ClosestPoint(p_Rigidbody.PlayerTransform.position);

        //        // Calculate wall run direction (along the wall)
        //        wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;

        //        // Ensure we're going in the right direction (forward not backward)
        //        if (Vector3.Dot(wallRunDirection, p_Rigidbody.PlayerVelocity) < 0)
        //            wallRunDirection = -wallRunDirection;

        //        ST_debug.LogState("WALL RUN");

        //        wallRunValue = new PlayerMovementValues(
        //    wallRunDirection,
        //    1.0f,
        //    1.0f,
        //    AnimationCurve.Linear(0, 1, 1, 0),
        //    0f,
        //    wallRunSpeed,
        //    ForceMode.Force
        //);

    }

    public override void UpdateState()
    {
        Debug.Log("aikjnads");
        cameraTilt.calculateTilt(wallPoint- p_Rigidbody.PlayerTransform.position, p_Rigidbody.PlayerForward, p_Rigidbody.PlayerRight);
        //Vector2 input = p_Input.Movement();

        //float forwardInput = Vector3.Dot(new Vector3(input.x, 0, input.y), wallRunDirection);

        //Vector3 adjustedDirection = wallRunDirection * Mathf.Max(forwardInput, 0.4f); // Always keep some forward momentum


        //ST_debug.Log($"Wall Run: {wallRunTimer:F1}s");
    }

    private void ApplyWallSpring()
    {
        Vector3 playerPos = p_Rigidbody.PlayerTransform.position;
        wallPoint = wallCollider.ClosestPoint(playerPos);
        //Debug.Log(wallPoint);
        ST_debug.DrawSphere(wallPoint, .2f, Color.red, 10f);

        Vector3 springDir = wallPoint - playerPos;
        p_Rigidbody.ApplySpringPull(p_Detector.wallHit.rigidbody, springDir, wallPoint, WallSpringValues);
    }

    public override void FixedUpdate()
    {
        wallRunTimer += Time.fixedDeltaTime;

        ApplyWallSpring();
        p_Rigidbody.OverrideVelocity(Vector3.Lerp(p_Rigidbody.PlayerVelocity, p_Rigidbody.PlayerPlaneVel, Time.deltaTime*2f), 1f);
        //p_Rigidbody.ApplyForce(-p_Rigidbody.wallHit.normal * wallStickForce, ForceMode.Force);

        //float upwardMultiplier = wallRunCurve.Evaluate(wallRunTimer / wallRunDuration);
        //p_Rigidbody.ApplyForce(Vector3.up * upwardForce * upwardMultiplier, ForceMode.Force);

        //p_Rigidbody.MoveInSpecifiedDirection(wallRunDirection,
        //    wallRunSpeed * wallRunCurve.Evaluate(wallRunTimer / wallRunDuration));
    }

    public override void ExitState()
    {
        cameraTilt.resetTilt();
        p_Rigidbody.setGravity(p_Rigidbody.GRAVITY);
        p_Rigidbody._CHECK_GRAVITY = true;
    }

    public override bool SwitchCondintion()
    {
        if (!p_Detector.isWall) return false;

        // Get velocity component parallel to the wall
        Vector3 wallParallelVelocity = Vector3.ProjectOnPlane(p_Rigidbody.PlayerPlaneVel, p_Detector.wallHit.normal);

        return wallParallelVelocity.magnitude >= minWallRunVelocity;
    }

    public override bool CanExit()
    {
        // Calculate current speed along the wall direction
        float speedAlongWall = Vector3.Dot(p_Rigidbody.PlayerVelocity, wallRunDirection);

        return (!p_Detector.isWall) ||
               p_Detector.isGrounded ||
               p_Input.Jump() || 
               p_Input.Dash() ||
               wallRunTimer >= wallRunDuration ||
               speedAlongWall < minWallRunVelocity;
    }
    private void RecalculateRunDirection()
    {
        if (!p_Detector.isWall) return;

        Vector3 wallNormal = p_Detector.wallHit.normal;
        Vector3 newDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;

        // Only switch direction if it's a large change, or first calculation
        if (wallRunDirection == Vector3.zero ||
            Vector3.Angle(newDirection, wallRunDirection) > 30f)
        {
            // Ensure we're going in the right direction
            if (Vector3.Dot(newDirection, p_Rigidbody.PlayerVelocity) < 0)
                newDirection = -newDirection;

            wallRunDirection = newDirection;
        }

        // Debug wall normal
        ST_debug.DrawSphere(
            p_Detector.wallHit.point,
            0.1f,
            Color.blue,
            0.1f
        );
    }

}
