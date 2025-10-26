using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If any single child reports a success, the node returns as a success
// The node returns failure only if all children fail
public class Selector : Node
{
    public Selector() : base() { }
    public Selector(List<Node> children) : base(children) { }
    public override NodeState Evaluate()
    {
        foreach (Node child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Failure:
                    continue;
                case NodeState.Success:
                    state = NodeState.Success;
                    return state;
                case NodeState.Running:
                    state = NodeState.Running;
                    return state;
                default:
                    continue;
            }
        }

        state = NodeState.Failure;
        return state;
    }
}