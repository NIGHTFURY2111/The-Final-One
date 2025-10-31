using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    Enemy_Entity enemy;
    DetectionManager detector;
    NavMeshAgent agent => enemy.navMeshAgent;

    public ChaseNode(Enemy_Entity enemy, DetectionManager detector)
    {
        this.enemy = enemy;
        this.detector = detector;
    }

    public override NodeState Evaluate()
    {
        if (detector.CurrentState != Enum_DetectionState.Chasing || detector.CurrentTarget == null)
        {
            state = NodeState.Failure;
            return state;
        }

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(detector.CurrentTarget.transform.position);
        }

        state = NodeState.Success;
        return state;
    }
}
