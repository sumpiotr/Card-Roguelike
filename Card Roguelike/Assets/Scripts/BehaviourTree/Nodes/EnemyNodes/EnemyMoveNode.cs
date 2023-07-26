using Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveNode : EnemyActionNode
{

    [SerializeField]
    private ActionData actionData;
    

    protected override void OnStart()
    {
       
    }

    protected override void OnStop()
    {
        
    }

    protected override NodeState OnUpdate()
    {
        owner.PrepareAction(actionData);
        return NodeState.Success;
    }
}
