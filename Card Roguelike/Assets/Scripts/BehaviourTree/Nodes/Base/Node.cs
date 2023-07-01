using UnityEngine;



public abstract class Node : ScriptableObject
{
    public NodeState state = NodeState.Running;
    public bool started;
    public string guid;
    public Vector2 position;
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

