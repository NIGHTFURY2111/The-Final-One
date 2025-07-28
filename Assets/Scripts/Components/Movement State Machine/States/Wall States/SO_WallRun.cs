using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] float upwardForce = 2f;           // Slight upward force to counteract gravity

    private float wallRunTimer = 0f;
    private Vector3 wallNormal;
    private Vector3 wallRunDirection;
    private PlayerMovementValues wallRunValue;

    public SO_WallRun(EC_Movement ctx) : base(ctx)
    {
    }

    public override void EnterState()
    {
        // Store wall normal and calculate run direction
        wallNormal = p_Rigidbody.wallHit.normal;

        // Calculate wall run direction (along the wall)
        wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;

        // Ensure we're going in the right direction (forward not backward)
        if (Vector3.Dot(wallRunDirection, p_Rigidbody.PlayerVelocity) < 0)
            wallRunDirection = -wallRunDirection;

        // Disable gravity
        p_Rigidbody._CHECK_GRAVITY = false;

        // Reset wall run timer
        wallRunTimer = 0f;

        // Debug
        ST_debug.LogState("WALL RUN");
        ST_debug.DrawSphere(p_Rigidbody.wallHit.point, 0.2f, Color.blue);

        // Create wall run movement value
        wallRunValue = new PlayerMovementValues(
            wallRunDirection,
            1.0f,
            1.0f,
            AnimationCurve.Linear(0, 1, 1, 0),
            0f,
            wallRunSpeed,
            ForceMode.Force
        );
    }

    public override void ExitState()
    {
        // Re-enable gravity
        p_Rigidbody.setGravity(p_Rigidbody.GRAVITY);
        p_Rigidbody._CHECK_GRAVITY = true;
    }

    public override bool SwitchCondintion()
    {
        // Check if we're hitting a wall and have enough velocity
        if (!p_Rigidbody.isWall) return false;

        // Get velocity component parallel to the wall
        Vector3 wallParallelVelocity = Vector3.ProjectOnPlane(p_Rigidbody.PlayerVelocity, p_Rigidbody.wallHit.normal);

        // Check if we have enough velocity to wall run
        return wallParallelVelocity.magnitude >= minWallRunVelocity;
    }

    public override bool CanExit()
    {
        // Calculate current speed along the wall direction
        float speedAlongWall = Vector3.Dot(p_Rigidbody.PlayerVelocity, wallRunDirection);

        // Exit if wall ends, player is grounded, time expires, OR speed drops below entry threshold
        return !p_Rigidbody.isWall ||
               p_Rigidbody.isGrounded ||
               wallRunTimer >= wallRunDuration ||
               speedAlongWall < minWallRunVelocity;
    }

    public override void UpdateState()
    {
        // Update wall run direction based on input
        Vector2 input = p_Input.Movement();

        // Only use the forward/backward component of input
        float forwardInput = Vector3.Dot(new Vector3(input.x, 0, input.y), wallRunDirection);

        // Adjust wall run direction based on input
        Vector3 adjustedDirection = wallRunDirection * Mathf.Max(forwardInput, 0.4f); // Always keep some forward momentum

        wallRunValue = wallRunValue.UpdateDirection(adjustedDirection);

        // Debug visualization
        ST_debug.DrawSphere(p_Rigidbody.PlayerTransform.position, 0.2f, Color.yellow);
        ST_debug.Log($"Wall Run: {wallRunTimer:F1}s");
    }

    public override void FixedUpdate()
    {
        // Update timer
        wallRunTimer += Time.fixedDeltaTime;

        // Apply wall stick force
        p_Rigidbody.ApplyForce(-p_Rigidbody.wallHit.normal * wallStickForce, ForceMode.Force);

        // Apply slight upward force based on curve
        float upwardMultiplier = wallRunCurve.Evaluate(wallRunTimer / wallRunDuration);
        p_Rigidbody.ApplyForce(Vector3.up * upwardForce * upwardMultiplier, ForceMode.Force);

        // Move along the wall
        p_Rigidbody.MoveInSpecifiedDirection(wallRunDirection,
            wallRunSpeed * wallRunCurve.Evaluate(wallRunTimer / wallRunDuration));
    }
}
