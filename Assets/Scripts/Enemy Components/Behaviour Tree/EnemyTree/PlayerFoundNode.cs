using UnityEngine;

public class PlayerFoundNode : Node
{
    Enemy_Entity enemy;
    DetectionManager detector;

    public PlayerFoundNode(Enemy_Entity enemy, DetectionManager detector)
    {
        this.enemy = enemy;
        this.detector = detector;
    }

    public override NodeState Evaluate()
    {
        state = (detector.CurrentState == Enum_DetectionState.Chasing && detector.CurrentTarget != null) 
            ? NodeState.Success 
            : NodeState.Failure;
        
        return state;
    }
}
