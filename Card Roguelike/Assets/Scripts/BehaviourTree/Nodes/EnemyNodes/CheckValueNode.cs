using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckValueNode : EnemyActionNode
{
    [SerializeField]
    private EnemyValues enemyValues;

    [SerializeField]
    private ComperationOptions option;

    [SerializeField]
    private int value;

    protected override void OnStart()
    {
       
    }

    protected override void OnStop()
    {
        
    }

    protected override NodeState OnUpdate()
    {
        bool output = false;
        switch (enemyValues)
        {
            case EnemyValues.Range:
                output = Compare(owner.GetRangeFromPlayer());
                break;
            case EnemyValues.Health:
                output = Compare(owner.Health);
                break;
        }
        return output ? NodeState.Success : NodeState.Failure;
    }

    private bool Compare(int a)
    {
        switch (option)
        {
            case ComperationOptions.Equal:
                return a == value;
            case ComperationOptions.Lesser:
                return a < value;
            case ComperationOptions.Greater:
                return a > value;
            case ComperationOptions.GreaterOrEqual:
                return a >= value;
            case ComperationOptions.LesserOrEqual:
                return a <= value;
            default:
                return false;
        }
    }
}
