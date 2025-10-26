using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootNode : Node
{
    GameObject bullet;
    GameObject iceShard;
    Transform shootpoint;
    GameObject enemy;
    bool shot = false;
    //DamageConstructor damage;
    //colors color;

    public ShootNode(GameObject b,Transform s,GameObject e,GameObject i)
    {
        bullet = b; 
        shootpoint = s;
        enemy = e;
        //damage = e.GetComponent<DamageConstructor>();
        //color = damage._color;
        iceShard =i;
    }

    IEnumerator shooting()
    {
        //shot = true;
        //switch (color)
        //{
        //    case colors.green:
        //        GameObject bulletshot = GameObject.Instantiate(bullet, shootpoint.position, shootpoint.rotation);
        //        bulletshot.GetComponent<EnemyBulletScript>().SetValues(damage._DMG_earth);
        //        break;
        //    case colors.red:
        //        Collider[] colliders = Physics.OverlapCapsule(enemy.transform.position + (enemy.transform.forward * 2), enemy.transform.position + (enemy.transform.forward * 10), 7f);

        //        foreach (Collider c in colliders)
        //        {
        //            if (c.gameObject.CompareTag("Player"))
        //            {
                        
        //                c.gameObject.GetComponent<PlayerDamageReciever>().getDamageValue(damage._DMG_fire);

        //            }

        //            if (c.GetComponent<MeshDestroy>()) { c.GetComponent<MeshDestroy>().DestroyMesh(); }

        //        }
        //        break;
        //    case colors.blue:
        //        GameObject shard1 = Object.Instantiate(iceShard, shootpoint.position + shootpoint.right * 1.5f, enemy.transform.rotation);

        //        EnemyBulletScript[] vv = new EnemyBulletScript[9];
        //        vv = shard1.GetComponentsInChildren<EnemyBulletScript>();
        //        foreach (EnemyBulletScript script in vv)
        //        {
        //            script.SetValues( damage._DMG_ice);
        //        }
        //        break;
        //}
        yield return new WaitForSecondsRealtime(0.5f);
        shot = false;
        
    }
    public override NodeState Evaluate()
    {
        //if(!shot)
        //    enemy.GetComponent<DamageConstructor>().StartCoroutine(shooting());
        //state = NodeState.Success; 
        return state;
    }
}
