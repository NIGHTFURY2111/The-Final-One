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
        p_Rigidbody._CHECK_GRAVITY = false;

        wallRunTimer = 0f;

        // Store wall normal and calculate run direction
        wallNormal = p_Rigidbody.wallHit.normal;

        // Calculate wall run direction (along the wall)
        wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;

        // Ensure we're going in the right direction (forward not backward)
        if (Vector3.Dot(wallRunDirection, p_Rigidbody.PlayerVelocity) < 0)
            wallRunDirection = -wallRunDirection;

        ST_debug.LogState("WALL RUN");

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

    private void RecalculateRunDirection()
    {
        if (!p_Rigidbody.isWall) return;

        Vector3 wallNormal = p_Rigidbody.wallHit.normal;
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
            p_Rigidbody.wallHit.point,
            0.1f,
            Color.blue,
            0.1f
        );
    }

    public override void ExitState()
    {
        p_Rigidbody.setGravity(p_Rigidbody.GRAVITY);
        p_Rigidbody._CHECK_GRAVITY = true;
    }

    public override bool SwitchCondintion()
    {
        if (!p_Rigidbody.isWall) return false;

        // Get velocity component parallel to the wall
        Vector3 wallParallelVelocity = Vector3.ProjectOnPlane(p_Rigidbody.PlayerVelocity, p_Rigidbody.wallHit.normal);

        return wallParallelVelocity.magnitude >= minWallRunVelocity;
    }

    public override bool CanExit()
    {
        // Calculate current speed along the wall direction
        float speedAlongWall = Vector3.Dot(p_Rigidbody.PlayerVelocity, wallRunDirection);

        return !p_Rigidbody.isWall ||
               p_Rigidbody.isGrounded ||
               p_Input.Jump() || 
               wallRunTimer >= wallRunDuration ||
               speedAlongWall < minWallRunVelocity;
    }

    public override void UpdateState()
    {
        Vector2 input = p_Input.Movement();

        float forwardInput = Vector3.Dot(new Vector3(input.x, 0, input.y), wallRunDirection);

        Vector3 adjustedDirection = wallRunDirection * Mathf.Max(forwardInput, 0.4f); // Always keep some forward momentum

        wallRunValue = wallRunValue.UpdateDirection(adjustedDirection);

        ST_debug.Log($"Wall Run: {wallRunTimer:F1}s");
    }

    public override void FixedUpdate()
    {
        wallRunTimer += Time.fixedDeltaTime;

        p_Rigidbody.ApplyForce(-p_Rigidbody.wallHit.normal * wallStickForce, ForceMode.Force);

        float upwardMultiplier = wallRunCurve.Evaluate(wallRunTimer / wallRunDuration);
        p_Rigidbody.ApplyForce(Vector3.up * upwardForce * upwardMultiplier, ForceMode.Force);

        p_Rigidbody.MoveInSpecifiedDirection(wallRunDirection,
            wallRunSpeed * wallRunCurve.Evaluate(wallRunTimer / wallRunDuration));
    }
}
