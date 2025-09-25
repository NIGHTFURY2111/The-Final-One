using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Detector", menuName = "Scriptable Object/Component/Detector")]
public class SO_detector : AC_Component
{
    [Serializable]
    struct GroundCheckValues
    {
        public LayerMask GroundLayer;
        public float GCRayLength;
        public GroundCheckValues(LayerMask groundLayer, float rayLength)
        {
            this.GroundLayer = groundLayer;
            this.GCRayLength = rayLength;
        }
    }

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

    public override Enum_ComponentType componentType => Enum_ComponentType.Detector;
    Rigidbody _RB;
    CapsuleCollider collider;

    [SerializeField] GroundCheckValues GroundValues;
    [SerializeField] WallValues wallValues;


    public RaycastHit _groundRayHit, wallHit, movementDirectionCollisionCheck;
    public bool isGrounded { get; private set; }
    public float lastGrounded { get; private set; }
    public bool isWall { get; private set; }


    public override void ComponentAwake()
    {
        _RB = entity.GetComponent<Rigidbody>();
        collider = entity.GetComponent<CapsuleCollider>();
    }

    public override void ComponentDisable() { }

    public override void ComponentStart() {
    }

    public override void ComponentUpdate()
    {
        CheckGrounded();
        CheckWallHit();
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


    Vector3 PlayerDown => _RB.transform.TransformDirection(Vector3.down);
    Vector3 PlayerUp => _RB.transform.TransformDirection(Vector3.up);
    Vector3 PlayerRight => _RB.transform.TransformDirection(Vector3.right);

}
