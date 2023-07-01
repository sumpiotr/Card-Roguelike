using UnityEngine;



public abstract class Node : ScriptableObject
{
    [HideInInspector] public NodeState state = NodeState.Running;
    [HideInInspector] public bool started;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    public NodeState Update()
    {
        if (!started)
        {
            OnStart();
            started = true;
        }
        state = OnUpdate();
        if (state != NodeState.Running)
        {
            OnStop();
            started = false;
        }
        return state;
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract NodeState OnUpdate();
}

public enum NodeState
{
    Running,
    Success,
    Failure
}

