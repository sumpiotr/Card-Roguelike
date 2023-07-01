using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{

    private int childIndex = 0;

    protected override void OnStart()
    {
       childIndex = 0;
    }

    protected override void OnStop()
    {
       
    }

    protected override NodeState OnUpdate()
    {
        Node child = children[childIndex];
        switch (child.Update())
        {
            case NodeState.Running:
                return NodeState.Running;
            case NodeState.Failure:
                return NodeState.Failure;
            case NodeState.Success:
                childIndex++;
                break;
        }
        return childIndex == children.Count ? NodeState.Success : NodeState.Running;
    }
}
