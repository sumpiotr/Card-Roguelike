using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourTreeScriptableObject", menuName = "BehaviourTree/EnemyBehaviourTree")]
public class EnemyBehaviourTree : BehaviourTree
{
    private Enemy owner;

    public void SetOwner(Enemy enemy)
    {
        owner = enemy;
        List<EnemyActionNode> enemyNodes = nodes.OfType<EnemyActionNode>().ToList();
        Debug.Log(enemyNodes.Count);

        foreach(EnemyActionNode enemyNode in enemyNodes)
        {
            enemyNode.owner = enemy;
        }
    }

    protected override void OnNodeCreated(Node node)
    {
        if(node is EnemyActionNode)
        {
            ((EnemyActionNode)node).owner = owner;
        }
    }
}
