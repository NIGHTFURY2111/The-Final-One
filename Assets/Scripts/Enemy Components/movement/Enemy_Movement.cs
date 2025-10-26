using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Movement", menuName = "Scriptable Object/Component/Enemy/Enemy Movement")]

public class Enemy_Movement : AC_Component
{
    Enemy_Entity enemy_entity;
    public override Enum_ComponentType componentType => Enum_ComponentType.Movement;

    public override void ComponentAwake()
    {
    }

    public override void ComponentDisable()
    {
    }

    public override void ComponentStart()
    {
        enemy_entity = (Enemy_Entity)entity;
    }

    public override void ComponentUpdate()
    {
        //bool inrange = Vector3.Distance(enemy_entity.transform.position, enemy_entity.target.transform.position) <= enemy_entity.shootDistance;
        //if (inrange)
        //{
        //    enemy_entity.navMeshAgent.isStopped = true;
        //    Debug.Log(enemy_entity.target.transform);
        //    //enemy_entity.transform.LookAt(enemy_entity.target.transform);

        //}
        //else
        //{
        //    enemy_entity.navMeshAgent.isStopped = false;
        //    enemy_entity.navMeshAgent.SetDestination(enemy_entity.target.transform.position);
        //}


        //if (enemy_entity.movementSO != null && enemy_entity.movementSO.stateManager?.currentState != null)
        //{
        //    ST_debug.LogState(enemy_entity.movementSO.stateManager.currentState.name);
        //}

        //// Pass the current velocity from rigidbody to camera for FOV effects
        //if (enemy_entity.rigidbodySO != null && enemy_entity.cameraSO != null)
        //{
        //    enemy_entity.cameraSO.ProcessVelocityForFOV(Vector3.Dot(enemy_entity.rigidbodySO.PlayerPlaneVel, enemy_entity.rigidbodySO.PlayerForward));
        //}

    }
}
