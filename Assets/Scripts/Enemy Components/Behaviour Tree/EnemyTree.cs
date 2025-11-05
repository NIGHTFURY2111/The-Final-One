using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Tree", menuName = "Behaviour Tree/Enemy BTree")]
public class EnemyTree : BehaviourTree
{
    [Header("NODE TEMPLATES ")]
    [SerializeField] private SO_InRange inRangeTemplate;
    [SerializeField] private SO_Aim aimTemplate;
    [SerializeField] private SO_Shoot shootTemplate;
    [SerializeField] private SO_Chase chaseTemplate;
    [SerializeField] private SO_MoveToLastKnown moveToLastKnownTemplate;
    [SerializeField] private SO_LookAround lookAroundTemplate;
    [SerializeField] private SO_Patrol patrolTemplate;

    public override Enum_ComponentType componentType => Enum_ComponentType.Behaviour_Tree;
    
    // Runtime instances
    private SO_InRange inRange;
    private SO_Aim aim;
    private SO_Shoot shoot;
    private SO_Chase chase;
    private SO_MoveToLastKnown moveToLastKnown;
    private SO_LookAround lookAround;
    private SO_Patrol patrol;
    
    [HideInInspector] public DetectionManager detector;
    [HideInInspector] public Enemy_Entity enemyEntity;

    public override void ComponentAwake()
    {
        enemyEntity = entity as Enemy_Entity;
        detector = enemyEntity.DetectorManager;

        CreateRuntimeNodeInstances();

        InitializeNodeInstances();
    }


    protected override Node SetupTree()
    {
        // Behavior Tree Structure:

        return new Selector(new List<Node>
        {
            //// Combat: Shoot if target exists and is within weapon range
            //// This allows shooting even when not in vision (e.g., at last known position)
            //new Sequence(new List<Node>
            //{
            //    new SO_LeafNode(inRange),  // Check weapon range (could be > vision range)
            //    new SO_LeafNode(aim),      // Aim at target
            //    new SO_LeafNode(shoot)     // Shoot
            //}),

            //// Chase: Actively pursue the target when in Chasing state
            //new SO_LeafNode(chase),

            //// Investigation: When Alerted (lost sight), go to last known position and look around
            new Sequence(new List<Node>
            {
                new SO_LeafNode(moveToLastKnown),
                new SO_LeafNode(lookAround)
            }),

            //// Patrol: Default idle behavior
            //new SO_LeafNode(patrol)
        });
    }

    public override void ComponentDisable()
    {
        DestroyRuntimeNodeInstances();
    }

    private void CreateRuntimeNodeInstances()
    {
        // Instantiate per-enemy instances
        inRange = Instantiate(inRangeTemplate);
        aim = Instantiate(aimTemplate);
        shoot = Instantiate(shootTemplate);
        chase = Instantiate(chaseTemplate);
        moveToLastKnown = Instantiate(moveToLastKnownTemplate);
        lookAround = Instantiate(lookAroundTemplate);
        patrol = Instantiate(patrolTemplate);
    }
    
    private void InitializeNodeInstances()
    {
        // Initialize
        inRange.Initialize(this);
        aim.Initialize(this);
        shoot.Initialize(this);
        chase.Initialize(this);
        moveToLastKnown.Initialize(this);
        lookAround.Initialize(this);
        patrol.Initialize(this);
    }
    
    private void DestroyRuntimeNodeInstances()
    {
        if (inRange != null) { inRange.Reset(); Destroy(inRange); }
        if (aim != null) { aim.Reset(); Destroy(aim); }
        if (shoot != null) { shoot.Reset(); Destroy(shoot); }
        if (chase != null) { chase.Reset(); Destroy(chase); }
        if (moveToLastKnown != null) { moveToLastKnown.Reset(); Destroy(moveToLastKnown); }
        if (lookAround != null) { lookAround.Reset(); Destroy(lookAround); }
        if (patrol != null) { patrol.Reset(); Destroy(patrol); }
    }
}

public class SO_LeafNode : Node
{
    private SO_BehaviourNode soNode;
    
    public SO_LeafNode(SO_BehaviourNode soNode)
    {
        this.soNode = soNode;
    }
    
    public override NodeState Evaluate()
    {
        state = soNode != null ? soNode.Evaluate() : NodeState.Failure;
        return state;
    }
}
