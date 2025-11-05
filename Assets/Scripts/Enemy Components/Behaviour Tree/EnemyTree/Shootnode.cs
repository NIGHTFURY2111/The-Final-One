using UnityEngine;

public class ShootNode : Node
{
    Enemy_Entity enemy;
    DetectionManager detector;
    GameObject bullet;
    Transform shootPoint;
    
    [SerializeField] float bulletSpeed = 20f;
    float shootCooldown = 1f;
    float lastShootTime = 0f;

    public ShootNode(Enemy_Entity enemy, DetectionManager detector, 
                     GameObject bullet, Transform shootPoint, float bulletSpeed)
    {
        this.enemy = enemy;
        this.detector = detector;
        this.bullet = bullet;
        this.shootPoint = shootPoint;
        this.bulletSpeed = bulletSpeed;
    }

    public override NodeState Evaluate()
    {
        if (detector.CurrentTarget == null)
        {
            state = NodeState.Failure;
            return state;
        }

        if (Time.time - lastShootTime < shootCooldown)
        {
            state = NodeState.Running;
            return state;
        }

        Debug.Log("Shooting at target!");

        //Shoot();
        lastShootTime = Time.time;

        state = NodeState.Success;
        return state;
    }

    void Shoot()
    {
        if (bullet == null || shootPoint == null || detector.CurrentTarget == null) return;

        Vector3 directionToTarget = detector.CurrentTarget.transform.position - enemy.transform.position;
        directionToTarget.y = 0;
        
        if (directionToTarget != Vector3.zero)
            enemy.transform.forward = directionToTarget;

        GameObject bulletInstance = Object.Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        
        Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            Vector3 shootDirection = (detector.CurrentTarget.transform.position - shootPoint.position).normalized;
            bulletRb.velocity = shootDirection * bulletSpeed;
        }
    }
}
