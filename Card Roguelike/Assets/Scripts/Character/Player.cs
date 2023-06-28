using Actions.ScriptableObjects;
using Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Tilemap.Tile;
using Tilemap;
using UnityEngine.Tilemaps;

public class Player : BaseCharacter
{

    const  int HAND_SIZE = 5;

    [SerializeField]
    private DeckScriptableObject deckData;

    private List<CardDataScriptableObject> deck = new List<CardDataScriptableObject>();
    private List<CardDataScriptableObject> discarded = new List<CardDataScriptableObject>();

    public Nullable<Vector2Int> targetPosition = null;

    public List<TileObject> highlitedTiles = new List<TileObject>();


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


    public void ResolveActionSet(List<ActionData> actionSet)
    {
        ResolveAction(0, actionSet);
    }

    protected void ResolveAction(int actionIndex, List<ActionData> actionSet)
    {
        if (actionIndex == actionSet.Count) return;
        Action onActionResolved = () => { ResolveAction(actionIndex + 1, actionSet); };
        ActionData actionData = actionSet[actionIndex];
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
            Move((Vector2Int)targetPosition, onResolved);
            return;
        }
        else
        {
            ChooseMoveTarget(actionData.range, (Vector2Int choosedPosition) =>
            {
                IEnumerator moveCoroutine =  Move(choosedPosition, onResolved);
                StartCoroutine(moveCoroutine);
            });
        }
        
    }


    private void ChooseMoveTarget(RangeData rangeData, Action<Vector2Int> onChoosed)
    {
        List<TileObject> tiles = new List<TileObject>();
        switch (rangeData.rangeType)
        {
            case RangeType.Area:
                tiles = HexTilemap.Instance.GetWalkableAndEmptyTileObjectsInRange(AxialPosition, rangeData.minRange, rangeData.maxRange);
                break;             
            case RangeType.Line:
                tiles = HexTilemap.Instance.GetWalkableAndEmptyTileObjectsInLines(AxialPosition, rangeData.minRange, rangeData.maxRange);
                break;                            
        }

        foreach (TileObject tile in tiles)
        {
            if (AStar.findPath(HexTilemap.Instance, AxialPosition, tile.axialPosition, rangeData.maxRange) == null) continue;
            tile.SetHighlight(Color.green, (Vector2Int tilePosition) =>
            {
                ResetHighlightedTiles();
                onChoosed(tilePosition);
            });
        }
        highlitedTiles = tiles;
    }

    private void ResetHighlightedTiles()
    {
        foreach(TileObject tile in highlitedTiles)
        {
            tile.RemoveHighlight();
        }
        highlitedTiles.Clear();
    }

    protected virtual void PlayPush(ActionData actionData, Action onResolved)
    {
        onResolved();
    }

    protected virtual void PlayBuff(ActionData actionData, Action onResolved)
    {
        onResolved();
    } 

    private IEnumerator Move(Vector2Int target, Action callback)
    {
        List<Node> path = AStar.findPath(HexTilemap.Instance, AxialPosition, target);
        Debug.Log(path == null);
        if (path != null)
        {
            if (path.Count > 0)
            {
                HexTilemap.Instance.GetHex(AxialPosition).SetOccupiedCharacter(null);
                foreach (Node node in path)
                {
                    TileObject tile = HexTilemap.Instance.GetHex(node.nodePosition);
                    transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, transform.position.z);
                    yield return new WaitForSeconds(Time.deltaTime * 500);
                }
                TileObject finalNode = HexTilemap.Instance.GetHex(path[path.Count - 1].nodePosition);
                finalNode.SetOccupiedCharacter(this);
            }
        }
        callback();

    }
}
