using Actions.ScriptableObjects;
using Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : BaseCharacter
{

    const  int HAND_SIZE = 5;

    [SerializeField]
    private DeckScriptableObject deckData;

    private List<CardDataScriptableObject> deck = new List<CardDataScriptableObject>();
    private List<CardDataScriptableObject> discarded = new List<CardDataScriptableObject>();

    public Nullable<Vector2Int> targetPosition = null;


    private void Start()
    {
        foreach (DeckItemData card in deckData.cards)
        {
            for (int i = 0; i < card.count; i++)
            {
                deck.Add(card.item);
            }
        }
        DrawCards(3);
    }

    private void DrawCard()
    {
        if (deck.Count == 0 && discarded.Count == 0) return;
        if (deck.Count == 0)
        {
            deck = discarded;
            discarded = new List<CardDataScriptableObject>();
        }
        int randomIndex = Random.Range(0, deck.Count);
        CardDataScriptableObject cardData = deck[randomIndex];
        deck.Remove(cardData);
        CardObject cardObject = CardObjectsManager.Instance.GetCardObject();
        cardObject.setupCard(cardData);
    }
    
    private void DrawCards(int count)
    {
        for(int i =0; i < count; i++)
        {
            DrawCard();
        }
    }


    public void ResolveActionSet(ActionSetScriptableObject actionSet)
    {
        ResolveAction(0, actionSet);
    }

    protected void ResolveAction(int actionIndex, ActionSetScriptableObject actionSet)
    {
        if (actionIndex == actionSet.actions.Count) return;
        Action onActionResolved = () => { ResolveAction(actionIndex + 1, actionSet); };
        ActionData actionData = actionSet.actions[actionIndex];
        switch (actionData.type)
        {
            case ActionType.Attack:
                PlayAttack(actionData, onActionResolved);
                break;
            case ActionType.Buff:
                PlayBuff(actionData, onActionResolved);
                break;
            case ActionType.Move:
                PlayMove(actionData, onActionResolved);
                break;
            case ActionType.Push:
                PlayPush(actionData, onActionResolved);
                break;
            default:
                break;
        }
    }

    protected virtual void PlayAttack(ActionData actionData, Action onResolved)
    {
        onResolved();
    }

    protected virtual void PlayMove(ActionData actionData, Action onResolved)
    {
        if(actionData.range.rangeType == RangeType.Target)
        {
            if (targetPosition == null) return;

        }
    }
    protected virtual void PlayPush(ActionData actionData, Action onResolved)
    {
        onResolved();
    }

    protected virtual void PlayBuff(ActionData actionData, Action onResolved)
    {
        onResolved();
    } 

    protected virtual void Move(Vector2Int target)
    {
        
    }
}
