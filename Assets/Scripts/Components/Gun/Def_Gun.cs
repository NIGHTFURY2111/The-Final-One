using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class Def_Gun : MonoBehaviour
{
    public SO_InputAccess InputAccessSO;
    public Material debugSelectedMaterial;
    public Material debugSeenMaterial;
    public Material debugNotMaterial;
    public GameObject debugPoint;


    [SerializeField] ParticleSystem ShootingSystem;
    [SerializeField] Transform BulletSpawnPoint;
    [SerializeField] ParticleSystem ImpactParticleSystem;
    [SerializeField] TrailRenderer BulletTrail;
    [SerializeField] float ShootDelay = 0.1f;
    [SerializeField] float Speed = 100;
    [SerializeField] LayerMask HitMask;
    [SerializeField] LayerMask EnemyMask;
    [SerializeField] LayerMask BounceMask;
    [SerializeField] bool BouncingBullets;
    [SerializeField] float BounceDistance = 10f;
    [SerializeField] GameObject Enemies;
    [SerializeField] float maxBulletBounceDist = 1000f;

    GameObject[] enemies;
    List<GameObject> magnetableEnemies = new();
    Transform cameraTransform;
    private float LastShootTime;


    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

    }

    private void Start()
    {
    }


    private void Update()
    {
        Debug.Log(InputAccessSO.Shoot());
        if (InputAccessSO.Shoot())
        {
            Shoot();
        }



        string jjj = "";
        // sorting the list

        //if direct hit, then put that enemy at the top
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Enemy")))
        {
            // Remove and reinsert the hit transform if it exists in the list
            if (magnetableEnemies.Remove(hit.transform.gameObject))
            {
                magnetableEnemies.Insert(0, hit.transform.gameObject);
            }
        }

        #region coloring the enemies for debuggign
        foreach (GameObject enemy in magnetableEnemies)
        {
            if (enemy == null)
            {
                magnetableEnemies.RemoveAt(0);
                break;
            }
            if (enemy == magnetableEnemies[0])
            {
                magnetableEnemies[0].GetComponent<MeshRenderer>().material = debugSelectedMaterial;
            }
            else
            {
                enemy.GetComponent<MeshRenderer>().material = debugSeenMaterial;
            }
            jjj += enemy.name + "   ";

        }
        #endregion

        // get the edges, maths style (deprecated now ig)
        //magnetableEnemies[0].TryGetComponent<ReturnEdges>(out ReturnEdges edgeScript);

        //new way to get closest point on the edge of the selected enemy

        #region debugging sphere to the targeted position
        // Calculate the projected point
        #endregion


    }
    public void Shoot()
    {


        if (LastShootTime + ShootDelay < Time.time)
        {
            // Use an object pool instead for these! To keep this tutorial focused, we'll skip implementing one.
            // for more details you can see: https://youtu.be/fsDE_mO4RZM or if using Unity 2021+: https://youtu.be/zyzqA_CPz2E

            ShootingSystem.Play();

            TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

            Vector3 direction = cameraTransform.forward;

            bool impactMade = Physics.Raycast(cameraTransform.position, direction, out RaycastHit hit, float.MaxValue, HitMask);
            bool bounceImpact = false;

            //Debug.Log(hit.collider);
            if (!hit.collider.IsUnityNull() && hit.collider.gameObject.layer == (Mathf.Log(EnemyMask.value, 2)))
            {
                //Debug.Log("auihghaui");
                bounceImpact = Physics.Raycast(cameraTransform.position, direction, float.MaxValue, BounceMask);
            }
            else if (magnetableEnemies.Count > 0)
            {
                DetectNearestEnemy();

                if (magnetableEnemies[0] == null)
                {
                    magnetableEnemies.RemoveAt(0);
                }

                Vector3 vectorToEnemy = magnetableEnemies[0].transform.position - cameraTransform.position;
                float dotEnemyToRayCentre = Vector3.Dot(vectorToEnemy, cameraTransform.forward);
                Vector3 projectedPoint = (cameraTransform.position) + (dotEnemyToRayCentre * cameraTransform.forward);
                Vector3 PointToShoot = magnetableEnemies[0].GetComponent<Collider>().ClosestPoint(projectedPoint);


                direction = (PointToShoot - cameraTransform.position).normalized;

                impactMade = Physics.Raycast(cameraTransform.position, direction, out hit, float.MaxValue, HitMask);
                bounceImpact = Physics.Raycast(cameraTransform.position, direction, float.MaxValue, BounceMask);

            }
            if (hit.collider.IsUnityNull() || hit.point == Vector3.zero)
            {
                hit.point = BulletSpawnPoint.position + direction * 100;
                hit.normal = cameraTransform.forward;
                impactMade = true;
                bounceImpact = false;
            }

            debugPoint.transform.position = hit.point;
            //Debug.Log(hit.point.ToString());

            //(The discard _ = is used to call the async method without awaiting it.)
            _ = SpawnTrail(trail, hit.point, hit.normal, BounceDistance, impactMade, bounceImpact);


            LastShootTime = Time.time;
        }
    }

    private async Task SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, float BounceDistance, bool MadeImpact, bool BounceImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        Vector3 direction = (HitPoint - Trail.transform.position).normalized;

        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float startingDistance = distance;

        while (distance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * Speed;

            await Task.Yield();
        }

        Trail.transform.position = HitPoint;

        if (MadeImpact)
        {
            //Debug.Log(HitNormal);
            Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
            if (BouncingBullets && BounceDistance > 0 && BounceImpact)
            {
                Vector3 bounceDirection = (findEnemy(HitPoint) - HitPoint).normalized;

                if (Physics.Raycast(HitPoint, bounceDirection, out RaycastHit hit, BounceDistance, HitMask))
                {
                    if (Physics.Raycast(HitPoint, bounceDirection, out RaycastHit hit1, BounceDistance, BounceMask))
                    {


                        await SpawnTrail(
                        Trail,
                        hit1.point,
                        hit.normal,
                        BounceDistance - Vector3.Distance(hit.point, HitPoint),
                        true,
                        true
                        );
                    }
                    else
                    {
                        await SpawnTrail(
                        Trail,
                        hit.point,
                        hit.normal,
                        BounceDistance - Vector3.Distance(hit.point, HitPoint),
                        true,
                        false
                        );
                    }

                }
                else
                {
                    await SpawnTrail(
                        Trail,
                        HitPoint + bounceDirection * BounceDistance,
                        Vector3.zero,
                        0,
                        false,
                        false
                    );
                }
            }
        }

        Destroy(Trail.gameObject, Trail.time);
    }


    Vector3 findEnemy(Vector3 hitPoint)
    {

        GameObject trackedEnemy = null;
        float minDist = maxBulletBounceDist;

        foreach (GameObject enemy in enemies)
        {
            float Dist = Vector3.Distance(hitPoint, enemy.transform.position);
            if (Dist < minDist)
            {
                minDist = Dist;
                trackedEnemy = enemy;
            }
        }
        if (trackedEnemy != null)
        {
            return trackedEnemy.transform.position;
        }
        else
        {

            return Vector3.zero;
        }
    }



    public void TriggerEnter(Collider other)
    {   // uses the cone collider and checks only on the "enemy layer"
        //if (other.CompareTag("Enemy") || other.)
        //   {
        if (other.gameObject != null && other.CompareTag("Enemy"))
        {
            magnetableEnemies.Add(other.gameObject);
            other.GetComponent<MeshRenderer>().material = debugSeenMaterial;
        }
        //Debug.Log(magnetableEnemies.Count);

        //Debug.Log(other.name);
    }

    public void TriggerExit(Collider other)
    {
        //if (other.CompareTag("Enemy"))
        //   {
        if (other.gameObject != null && other.CompareTag("Enemy"))
        {
            magnetableEnemies.Remove(other.gameObject);
            other.GetComponent<MeshRenderer>().material = debugNotMaterial;
        }
        //Debug.Log(magnetableEnemies.Count);
        //}
    }


    void DetectNearestEnemy()
    {
        magnetableEnemies.Sort((enemy1, enemy2) =>
        {
            float angle1 = Vector3.Angle(
                cameraTransform.forward,
                (enemy1.transform.position - cameraTransform.position).normalized
            );
            float angle2 = Vector3.Angle(
                cameraTransform.forward,
                (enemy2.transform.position - cameraTransform.position).normalized
            );
            return angle1.CompareTo(angle2);
        });
    }
}



//working:
//    store the list of enemies that are colliding
//    calculate their distance for the center (using the .forward of camera as centre)
//    shoot the enemy with the lowests angle
//    if not possible, shoot the closest enemy
//    priority list: * Hat
//                   * enemy closest by angle
//                   * enemy closest by distance(?)
//                   * straight forward

// *  