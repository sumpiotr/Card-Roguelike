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

    const int HAND_SIZE = 5;

    [SerializeField]
    private DeckScriptableObject deckData;

    private List<CardDataScriptableObject> _deck = new List<CardDataScriptableObject>();
    private List<CardDataScriptableObject> _discarded = new List<CardDataScriptableObject>();

    private Nullable<Vector2Int> _targetPosition = null;

    private List<TileObject> _highlitedTiles = new List<TileObject>();


    private void Start()
    {
        foreach (DeckItemData card in deckData.cards)
        {
            for (int i = 0; i < card.count; i++)
            {
                _deck.Add(card.item);
            }
        }
        DrawCards(HAND_SIZE);
    }

    private void DrawCard()
    {
        if (_deck.Count == 0 && _discarded.Count == 0) return;
        if (_deck.Count == 0)
        {
            _deck = _discarded;
            _discarded = new List<CardDataScriptableObject>();
        }
        int randomIndex = Random.Range(0, _deck.Count);
        CardDataScriptableObject cardData = _deck[randomIndex];
        _deck.Remove(cardData);
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
            if (_targetPosition == null) return;
            Move((Vector2Int)_targetPosition, onResolved);
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
        _highlitedTiles = tiles;
    }

    private void ResetHighlightedTiles()
    {
        foreach(TileObject tile in _highlitedTiles)
        {
            tile.RemoveHighlight();
        }
        _highlitedTiles.Clear();
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
        List<PathNode> path = AStar.findPath(HexTilemap.Instance, AxialPosition, target);
        Debug.Log(path == null);
        if (path != null)
        {
            if (path.Count > 0)
            {
                HexTilemap.Instance.GetTile(AxialPosition).SetOccupiedCharacter(null);
                foreach (PathNode node in path)
                {
                    TileObject tile = HexTilemap.Instance.GetTile(node.nodePosition);
                    transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, transform.position.z);
                    yield return new WaitForSeconds(Time.deltaTime * 500);
                }
                TileObject finalNode = HexTilemap.Instance.GetTile(path[path.Count - 1].nodePosition);
                finalNode.SetOccupiedCharacter(this);
            }
        }
        callback();

    }
}
