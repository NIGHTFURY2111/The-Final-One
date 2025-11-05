using UnityEngine;

public class AimNode : Node
{
    Enemy_Entity enemy;
    DetectionManager detector;

    public AimNode(Enemy_Entity enemy, DetectionManager detector)
    {
        this.enemy = enemy;
        this.detector = detector;
    }

    public override NodeState Evaluate()
    {
        if (detector.CurrentTarget == null)
        {
            state = NodeState.Failure;
            return state;
        }

        enemy.transform.forward = detector.CurrentTarget.transform.position - enemy.transform.position;
        
        state = NodeState.Success;
        return state;
    }
}

