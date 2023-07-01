using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{

    int childIndex = 0;

    protected override void OnStart()
    {
        childIndex = 0;
    }

    protected override void OnStop()
    {
        ;
    }

    protected override NodeState OnUpdate()
    {
        Node child = children[childIndex];
        switch (child.Update())
        {
            case NodeState.Running:
                return NodeState.Running;
            case NodeState.Failure:
                childIndex++;
                break;
            case NodeState.Success:
                return NodeState.Success;
        }
        return childIndex == children.Count ? NodeState.Failure : NodeState.Running;
    }
}
