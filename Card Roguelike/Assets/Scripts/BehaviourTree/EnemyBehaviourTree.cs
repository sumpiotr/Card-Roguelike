using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourTreeScriptableObject", menuName = "BehaviourTree/EnemyBehaviourTree")]
public class EnemyBehaviourTree : BehaviourTree
{
    private Enemy owner;

    public void SetOwner(Enemy enemy)
    {
        owner = enemy;
    }

    protected override void OnNodeCreated(Node node)
    {
        if(node is EnemyActionNode)
        {
            ((EnemyActionNode)node).owner = owner;
        }
    }
}
