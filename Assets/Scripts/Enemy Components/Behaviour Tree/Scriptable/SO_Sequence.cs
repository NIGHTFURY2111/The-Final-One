using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sequence composite for SO nodes - succeeds only if ALL children succeed
/// </summary>
[CreateAssetMenu(fileName = "Sequence", menuName = "Behaviour Tree/Scriptable/Composite/Sequence")]
public class SO_Sequence : SO_BehaviourNode
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
        bool anyChildRunning = false;
        
        foreach (var child in children)
        {
            if (child == null) continue;
            
            switch (child.Evaluate())
            {
                case NodeState.Running:
                    anyChildRunning = true;
                    continue;
                case NodeState.Success:
                    continue;
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
            }
        }
        
        state = anyChildRunning ? NodeState.Running : NodeState.Success;
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
