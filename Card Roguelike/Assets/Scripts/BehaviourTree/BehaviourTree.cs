using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourTreeScriptableObject", menuName = "BehaviourTree")]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;

    public NodeState treeState = NodeState.Running;

    public List<Node> nodes = new List<Node>();

    public NodeState Update()
    {
        if (rootNode.state == NodeState.Running)
        {
            treeState = rootNode.Update();
        }
        return treeState;
    }


    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    protected virtual void OnNodeCreated(Node node)
    {

    }

    public void RemoveNode(Node node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        if(parent is DecoratorNode)
        {
            ((DecoratorNode)parent).child = child;
        }
        else if (parent is CompositeNode)
        {
           ((CompositeNode)parent).children.Add(child);
        }
        else if (parent is RootNode)
        {
            ((RootNode)parent).child = child;
        }
    }

    public void RemoveChild(Node parent, Node child)
    {
        if (parent is DecoratorNode)
        {
            ((DecoratorNode)parent).child = null;
        }
        else if (parent is CompositeNode)
        {
            ((CompositeNode)parent).children.Remove(child);
        }
        else if (parent is RootNode)
        {
            ((RootNode)parent).child = null;
        }
    }

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        if (parent is DecoratorNode)
        {
            DecoratorNode decoratorNode = (DecoratorNode)parent;
            if(decoratorNode.child != null) children.Add(decoratorNode.child);
        }
        else if (parent is CompositeNode)
        {
            children =  ((CompositeNode)parent).children;
        }
        else if (parent is RootNode)
        {
            RootNode rootNode = (RootNode)parent;
            if(rootNode.child != null)children.Add(rootNode.child);
        }
        return children;
    }


    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        return tree;
    }
}

