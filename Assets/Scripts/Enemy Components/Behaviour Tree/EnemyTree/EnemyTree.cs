using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Tree", menuName = "Behaviour Tree/Enemy BTree")]
public class EnemyTree : BehaviourTree
{
    //[SerializeField] GameObject iceShard;
    //[SerializeField] GameObject bullet;
    //[SerializeField] Transform shootPoint;

    public override Enum_ComponentType componentType => Enum_ComponentType.Behaviour_Tree;

    protected override Node SetupTree()
    {
        Node root = new Sequence(new List<Node>
        {
            new Selector(new List<Node>{
                new PlayerFoundNode(entity.gameObject),
                new PathfindingNode(entity.gameObject)
            }),
            new Sequence(new List<Node>
            {
                new AimNode(entity.gameObject),
                //new ShootNode(bullet,shootPoint,entity.gameObject,iceShard)
            })
        });
        return root;
    }
    public override void ComponentAwake() { }
    public override void ComponentDisable() { }
}

