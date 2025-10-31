using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base ScriptableObject node
/// Holds configuration AND implements tree logic
/// </summary>
public abstract class SO_BehaviourNode : ScriptableObject
{
    protected NodeState state;
    protected Enemy_Entity BT_Entity;
    protected DetectionManager detector;
    protected Enemy_Movement movement => BT_Entity?.EnemyMovement;
    
    /// <summary>
    /// Initialize node with enemy context (called once when tree is built)
    /// </summary>
    public virtual void Initialize( EnemyTree tree)
    {
        this.BT_Entity = tree.enemyEntity;
        this.detector = tree.detector;
    }
    
    /// <summary>
    /// Evaluate this node's logic
    /// </summary>
    public abstract NodeState Evaluate();
    
    /// <summary>
    /// Called when tree is reset or disabled
    /// </summary>
    public virtual void Reset()
    {
        state = NodeState.Failure;
    }
    
    /// <summary>
    /// Current state of this node
    /// </summary>
    public NodeState State => state;
}
