using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerFoundNode : Node
{
    Transform enemyTransform;
    NavMeshAgent agent;
    public PlayerFoundNode(GameObject enemy)
    {
        enemyTransform = enemy.transform;
        agent = enemy.GetComponent<NavMeshAgent>();
    }
    public override NodeState Evaluate()
    {
        foreach (Collider c in Physics.OverlapSphere(enemyTransform.position, 10f))
        {
            if (c.CompareTag("Player"))
            {
                agent.destination = enemyTransform.position;
                state = NodeState.Success;
                return state;
            }
        }
        state = NodeState.Failure; 
        return state;
    }
    
}
