using UnityEngine;
using UnityEngine.AI;

public class InvestigateNode : Node
{
    Enemy_Entity enemy;
    DetectionManager detector;
    NavMeshAgent agent => enemy.navMeshAgent;
    
    Vector3 lastKnownPosition;
    float investigateReachedDistance = 2f;

    public InvestigateNode(Enemy_Entity enemy, DetectionManager detector)
    {
        this.enemy = enemy;
        this.detector = detector;
    }

    public override NodeState Evaluate()
    {
        if (detector.CurrentState != Enum_DetectionState.Alerted)
        {
            state = NodeState.Failure;
            return state;
        }

        if (detector.CurrentTarget != null)
            lastKnownPosition = detector.CurrentTarget.transform.position;

        if (Vector3.Distance(enemy.transform.position, lastKnownPosition) <= investigateReachedDistance)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            
            state = NodeState.Success;
            return state;
        }

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(lastKnownPosition);
        }

        state = NodeState.Running;
        return state;
    }
}
