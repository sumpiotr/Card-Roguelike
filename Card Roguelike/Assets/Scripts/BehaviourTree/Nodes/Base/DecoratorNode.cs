using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class DecoratorNode : Node
{
    public Node child;

    public override Node Clone()
    {
        DecoratorNode clone = Instantiate(this);
        clone.child = child.Clone();
        return clone;
    }
}
