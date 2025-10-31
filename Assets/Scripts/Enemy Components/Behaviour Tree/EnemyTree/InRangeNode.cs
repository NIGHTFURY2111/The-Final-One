using UnityEngine;

public class InRangeNode : Node
{
    Enemy_Entity enemy;
    DetectionManager detector;
    float attackRange;

    public InRangeNode(Enemy_Entity enemy, DetectionManager detector, float attackRange)
    {
        this.enemy = enemy;
        this.detector = detector;
        this.attackRange = attackRange;
    }

    public override NodeState Evaluate()
    {
        if (detector.CurrentTarget == null)
        {
            state = NodeState.Failure;
            return state;
        }

        float distanceToTarget = Vector3.Distance(enemy.transform.position, detector.CurrentTarget.transform.position);

        state = distanceToTarget <= attackRange ? NodeState.Success : NodeState.Failure;
        return state;
    }
}
