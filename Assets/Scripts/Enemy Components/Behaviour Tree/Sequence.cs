using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If any single child fails, the entire node fails
// If any child is still running, the node returns running
// Only returns success when all nodes return success
public class Sequence : Node
{
    
    public Sequence():base() { }
    public Sequence(List<Node> children) : base(children) { }
    public override NodeState Evaluate()
    {
        foreach (Node child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
                case NodeState.Success:
                    continue;
                case NodeState.Running:
                    state = NodeState.Running;
                    return state;
                    
                default:
                    state = NodeState.Success;
                    break;
            }
        }

        state = NodeState.Success;
        return state;
    }
}