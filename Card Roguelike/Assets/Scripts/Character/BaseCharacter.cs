using Actions;
using Actions.ScriptableObjects;
using Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour, ICharacter
{

    [SerializeField]
    private ActionDeckScriptableObject deck;
    protected List<Tuple<Vector2, ActionData>> preparedActions;

    public Vector2 Position { get => transform.position; set => transform.position = value; }

    public void ResolveActionSet(ActionSetScriptableObject actionSet)
    {
        ResolveAction(0, actionSet);
    }   

    protected void ResolveAction(int actionIndex, ActionSetScriptableObject actionSet)
    {
        if (actionIndex == actionSet.actions.Count) return;
        Action onActionResolved = ()=> { ResolveAction(actionIndex + 1, actionSet); };
        ActionData actionData = actionSet.actions[actionIndex];
        switch (actionData.type)
        {
            case ActionType.Attack:
                PrepareAttack(actionData, onActionResolved);
                break;
            case ActionType.Buff:
                PrepareBuff(actionData, onActionResolved);
                break;
            case ActionType.Move:
                PrepareMove(actionData, onActionResolved);
                break;
            case ActionType.Push:
                PreparePush(actionData, onActionResolved);
                break;
            default:
                break;
        }
    }

    protected virtual void PrepareAttack(ActionData actionData, Action onResolved) 
    {
        onResolved();
    }

    protected virtual void PrepareMove(ActionData actionData, Action onResolved)
    {
        onResolved();
    }
    protected virtual void PreparePush(ActionData actionData, Action onResolved)
    {
        onResolved();
    }

    protected virtual void PrepareBuff(ActionData actionData, Action onResolved)
    {
        onResolved();
    }



}
