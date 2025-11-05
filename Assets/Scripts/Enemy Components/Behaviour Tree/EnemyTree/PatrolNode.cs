using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PatrolNode : Node
{
    Enemy_Entity enemy;
    NavMeshAgent agent => enemy.navMeshAgent;
    
    [System.Serializable]
    public struct PatrolSettings
    {
        public float patrolRadius;
        public float waypointReachedDistance;
    }

    [SerializeField] PatrolSettings patrolSettings = new PatrolSettings { 
        patrolRadius = 15f, 
        waypointReachedDistance = 2f 
    };

    List<Vector3> patrolPoints; // Pre-computed patrol waypoints
    int currentWaypointIndex = 0;
    Vector3 currentWaypoint;
    bool hasWaypoint = false;

    public PatrolNode(Enemy_Entity enemy)
    {
        this.enemy = enemy;
    }

    public override NodeState Evaluate()
    {
        if (!hasWaypoint || Vector3.Distance(enemy.transform.position, currentWaypoint) <= patrolSettings.waypointReachedDistance)
        {
            if (patrolPoints == null || patrolPoints.Count == 0)
            {
                currentWaypoint = GetRandomPatrolPoint();
            }
            else
            {
                currentWaypoint = patrolPoints[currentWaypointIndex];
                currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Count;
            }
            hasWaypoint = true;
        }

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(currentWaypoint);
        }

        state = NodeState.Running;
        return state;
    }

    Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolSettings.patrolRadius;
        randomDirection += enemy.transform.position;
        randomDirection.y = enemy.transform.position.y;

        return NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolSettings.patrolRadius, NavMesh.AllAreas) 
            ? hit.position 
            : enemy.transform.position;
    }
}
