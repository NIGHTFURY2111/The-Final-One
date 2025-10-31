using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Selector composite for SO nodes - succeeds if ANY child succeeds
/// </summary>
[CreateAssetMenu(fileName = "Selector", menuName = "Behaviour Tree/Scriptable/Composite/Selector")]
public class SO_Selector : SO_BehaviourNode
{
    [SerializeField] private List<SO_BehaviourNode> children = new List<SO_BehaviourNode>();
    
    public override void Initialize(EnemyTree tree)
    {
        base.Initialize(tree);
        
        // Initialize all children
        foreach (var child in children)
        {
            if (child != null)
                child.Initialize(tree);
        }
    }
    
    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            if (child == null) continue;
            
            switch (child.Evaluate())
            {
                case NodeState.Running:
                    state = NodeState.Running;
                    return state;
                case NodeState.Success:
                    state = NodeState.Success;
                    return state;
                case NodeState.Failure:
                    continue;
            }
        }
        
        state = NodeState.Failure;
        return state;
    }
    
    public override void Reset()
    {
        base.Reset();
        foreach (var child in children)
        {
            if (child != null)
                child.Reset();
        }
    }
}
