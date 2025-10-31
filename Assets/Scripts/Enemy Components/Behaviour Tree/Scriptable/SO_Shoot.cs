using UnityEngine;

/// <summary>
/// Action: Shoots at current target with cooldown
/// </summary>
[CreateAssetMenu(fileName = "Shoot", menuName = "Behaviour Tree/Scriptable/Action/Shoot")]
public class SO_Shoot : SO_BehaviourNode
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private string shootPointName = "ShootPoint";
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float shootCooldown = 1f;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    // Runtime state
    private float lastShootTime = float.NegativeInfinity;
    private Transform shootPoint;
    
    public override void Initialize(EnemyTree tree)
    {
        base.Initialize(tree);
        
        // Find shoot point
        shootPoint = BT_Entity.transform.Find(shootPointName);
        if (shootPoint == null)
        {
            Debug.LogWarning($"[SO_Shoot] ShootPoint '{shootPointName}' not found on {BT_Entity.name}");
            shootPoint = BT_Entity.transform;
        }
        lastShootTime = float.NegativeInfinity;
    }
    
    public override void Reset()
    {
        base.Reset();
        lastShootTime = float.NegativeInfinity;
    }
    
    public override NodeState Evaluate()
    {
        if (detector.CurrentTarget == null || detector.CurrentState == Enum_DetectionState.Idle)
        {
            state = NodeState.Failure;
            return state;
        }
        Debug.Log("shoot");
        
        // Check cooldown
        if (Time.time - lastShootTime < shootCooldown)
        {
            state = NodeState.Running;
            return state;
        }
        
        Shoot();
        lastShootTime = Time.time;
        
        state = NodeState.Success;
        return state;
    }
    
    private void Shoot()
    {
        if (bulletPrefab == null)
        {
            if (debugMode)
                Debug.LogWarning("[SO_Shoot] No bullet prefab assigned!");
            return;
        }
        
        Vector3 shootDirection = (detector.CurrentTarget.transform.position - shootPoint.position).normalized;
        
        GameObject bulletInstance = Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(shootDirection));
        
        Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = shootDirection * bulletSpeed;
        }
        
        if (debugMode)
        {
            Debug.Log($"[SO_Shoot] Fired at {detector.CurrentTarget.name}");
            Debug.DrawRay(shootPoint.position, shootDirection * 10f, Color.yellow, 0.5f);
        }
    }
}
