using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AimNode : Node
{
    Transform player;
    Transform enemyTransform;
    public AimNode(GameObject enemy)
    {
        enemyTransform = enemy.transform;
        player = GameObject.FindWithTag("Player").transform;
    }
    public override NodeState Evaluate()
    {
        enemyTransform.forward = player.position - enemyTransform.position; 
        state = NodeState.Success; 
        return state;
    }
}

