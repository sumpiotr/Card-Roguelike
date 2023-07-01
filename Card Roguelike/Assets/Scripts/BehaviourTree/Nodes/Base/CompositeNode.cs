using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CompositeNode : Node
{
   public List<Node> children = new List<Node>();

    public override Node Clone()
    {
        CompositeNode clone = Instantiate(this);
        clone.children = children.ConvertAll(c => c.Clone());
        return clone;
    }
}
