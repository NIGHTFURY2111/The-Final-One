using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathfindingNode : Node
{
    Transform player;
    NavMeshAgent agent;
    public PathfindingNode(GameObject enemy)
    {
        agent = enemy.GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
    }
    public override NodeState Evaluate()
    {
        agent.destination = player.position;
        state = NodeState.Running; 
        return state;
    }
}
