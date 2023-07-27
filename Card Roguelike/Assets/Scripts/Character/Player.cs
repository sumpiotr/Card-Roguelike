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
using Unity.VisualScripting;
using UnityEngine.WSA;

public class Player : BaseCharacter
{
    //TO DO ADD CARDS Validation
    const int HAND_SIZE = 5;

    [SerializeField]
    private DeckScriptableObject deckData;

    private List<CardDataScriptableObject> _deck = new List<CardDataScriptableObject>();
    private List<CardDataScriptableObject> _discarded = new List<CardDataScriptableObject>();

    private List<Vector2Int> _targetsPositions = new List<Vector2Int>();

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


   

    protected override void ResolveAction(ActionData actionData, Action onActionResolved)
    {
        if (actionData.type == ActionType.ChooseAttackTarget) {
            if (actionData.range.rangeType == RangeType.Line)
            {
                ChooseAttackTarget(actionData.range, (Vector2Int choosedPosition) =>
                {
                    _targetsPositions.Clear();
                    _targetsPositions.Add(choosedPosition);
                }, onActionResolved);
            }
            else if (actionData.range.rangeType == RangeType.Area)
            {

            }
           
        }
        else if(actionData.type == ActionType.ChooseMoveTarget)
        {
                ChooseMoveTarget(actionData.range, (Vector2Int choosedPosition) =>
                {
                    _targetsPositions.Clear();
                    _targetsPositions.Add(choosedPosition);
                    onActionResolved();
                });
        }
        else
        {
            base.ResolveAction(actionData, onActionResolved);
        }
    }

    protected override void PlayAttack(ActionData actionData, Action onResolved)
    {
        switch (actionData.range.rangeType)
        {
            case RangeType.Target:
                foreach(Vector2Int position in _targetsPositions)
                {
                    Attack(position, actionData.value);
                }
                onResolved();
                break;
            case RangeType.Line:
                ChooseAttackTarget(actionData.range, (Vector2Int enemyPosition) =>
                {
                    Attack(enemyPosition, actionData.value);
                    _targetsPositions.Clear();
                    _targetsPositions.Add(enemyPosition);
                }, onResolved);
                break;
            case RangeType.Area:
                _targetsPositions.Clear();
                ChooseAttackTarget(actionData.range, (Vector2Int enemyPosition) =>
                {
                    Attack(enemyPosition, actionData.value);
                    _targetsPositions.Add(enemyPosition);
                }, onResolved);
                break;
            default:
                onResolved();
                return;
        }

        
    }

    private void ChooseAttackTarget(RangeData rangeData, Action<Vector2Int> onChoosed, Action onResolved)
    {
        List<TileObject> tiles = new List<TileObject>();
        tiles = HexTilemap.Instance.GetOccupiedTileObjectsInRange(AxialPosition, rangeData.minRange, rangeData.maxRange);


        if (rangeData.rangeType == RangeType.Line)
        {
            foreach (TileObject tile in tiles)
            {
                if (!CanHit(tile.axialPosition)) continue;
                tile.SetHighlight(Color.red, (Vector2Int tilePosition) =>
                {
                    ResetHighlightedTiles();
                    onChoosed(tilePosition);
                    onResolved();
                });
            }
            _highlitedTiles = tiles;
        }
        else if(rangeData.rangeType == RangeType.Area)
        {
            foreach (TileObject tile in tiles)
            {
                if (!CanHit(tile.axialPosition, true)) continue;
                onChoosed(tile.axialPosition);
            }
            onResolved();
        }
    }

    protected override void PlayMove(ActionData actionData, Action onResolved)
    {
        if(actionData.range.rangeType == RangeType.Target)
        {

            if (_targetsPositions.Count > 0) Move(_targetsPositions[0], onResolved);
            else onResolved();
            return;
        }
        else
        {
            ChooseMoveTarget(actionData.range, (Vector2Int choosedPosition) =>
            {
                _targetsPositions.Clear();
                _targetsPositions.Add(choosedPosition);
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

    protected override void PlayPush(ActionData actionData, Action onResolved)
    {
        switch (actionData.range.rangeType)
        {
            case RangeType.Target:
                if(_targetsPositions.Count == 1) Push(_targetsPositions[0], actionData.value, onResolved);
                else
                {
                    foreach(Vector2Int tilePosition in _targetsPositions)
                    {
                        RandomPush(tilePosition, actionData.value);
                    }
                    onResolved();
                }
                break;
            case RangeType.Line:
                ChooseAttackTarget(actionData.range, (Vector2Int enemyPosition) =>
                {
                    _targetsPositions.Clear();
                    _targetsPositions.Add(enemyPosition);
                    Push(enemyPosition, actionData.value, onResolved);
                }, () => { });
                break;
           case RangeType.Area:
                _targetsPositions.Clear();
                for (int i = actionData.range.maxRange; i >= actionData.range.minRange; i--)
                {
                    List<TileObject> tiles = HexTilemap.Instance.GetOccupiedTileObjectsInRange(AxialPosition, i, i);
                    foreach (TileObject tile in tiles)
                    {
                        if (!CanHit(tile.axialPosition, true)) continue;
                        _targetsPositions.Add(tile.axialPosition);
                        RandomPush(tile.axialPosition, actionData.value);
                    }
                }
                onResolved();
                break;
            default:
                onResolved();
                return;
        }
    }

    protected override void PlayPull(ActionData actionData, Action onResolved)
    {
        switch (actionData.range.rangeType)
        {
            case RangeType.Target:
                if (_targetsPositions.Count == 1) Pull(_targetsPositions[0], actionData.value, onResolved);
                else
                {
                    foreach (Vector2Int tilePosition in _targetsPositions)
                    {
                        RandomPull(tilePosition, actionData.value);
                    }
                    onResolved();
                }
                break;
            case RangeType.Line:
                ChooseAttackTarget(actionData.range, (Vector2Int enemyPosition) =>
                {
                    _targetsPositions.Clear();
                    _targetsPositions.Add(enemyPosition);
                    Pull(enemyPosition, actionData.value, onResolved);
                }, () => { });
                break;
            case RangeType.Area:
                for (int i = actionData.range.minRange; i <= actionData.range.maxRange; i++)
                {
                    _targetsPositions.Clear();
                    List<TileObject> tiles = HexTilemap.Instance.GetOccupiedTileObjectsInRange(AxialPosition, i, i);
                    foreach (TileObject tile in tiles)
                    {
                        if (!CanHit(tile.axialPosition, true)) continue;
                        _targetsPositions.Add(tile.axialPosition);
                        RandomPull(tile.axialPosition, actionData.value);
                    }
                }
                onResolved();
                break;
            default:
                onResolved();
                return;
        }
    }



    protected override void PlayBuff(ActionData actionData, Action onResolved)
    {
        onResolved();
    }

    protected override void PlayAdvance(ActionData actionData, Action onResolved)
    {
        if(actionData.range.rangeType == RangeType.Target)
        {
            if(_targetsPositions.Count > 0)
            {
                Advance(actionData.value, _targetsPositions[0]);
                onResolved();
            }
        }
        else
        {
            ChooseAttackTarget(actionData.range, (Vector2Int target) =>
            {
                Advance(actionData.value, target);
            }, onResolved);
        }
    }

    protected override void PlayRetreat(ActionData actionData, Action onResolved)
    {
        if (actionData.range.rangeType == RangeType.Target)
        {
            if (_targetsPositions.Count > 0)
            {
                Retreat(actionData.value, _targetsPositions[0]);
                onResolved();
            }
        }
        else
        {
            ChooseAttackTarget(actionData.range, (Vector2Int target) =>
            {
                Retreat(actionData.value, target);
            }, onResolved);
        }
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

    private void Attack(Vector2Int enemyPosition, int amount)
    {
        TileObject tile = HexTilemap.Instance.GetTile(enemyPosition);
        if (tile == null) return;
        if (tile.IsEmpty()) return;
        tile.GetOccupyingCharacter().TakeDamage(amount);
    }

    private void Push(Vector2Int enemyPosition, int amount, Action onEnd)
    {
        TileObject tile = HexTilemap.Instance.GetTile(enemyPosition);
        if (tile == null) return;
        if (tile.IsEmpty()) return;
        BaseCharacter enemy = tile.GetOccupyingCharacter();
        List<TileObject> tiles = enemy.GetRetretTiles(AxialPosition, amount, amount);
        if(tiles.Count == 0)
        {
            amount--;
            while(amount > 0)
            {
                tiles = enemy.GetRetretTiles(AxialPosition, amount, amount);
                if (tiles.Count > 0) break;
                amount--;
            }
        }
        _highlitedTiles = tiles;

        if(tiles.Count == 1)
        {
            TileObject selectedTile = tiles[0];
            selectedTile.SetOccupiedCharacter(enemy);
            tile.SetOccupiedCharacter(null);
            enemy.transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, enemy.transform.position.z);
            onEnd();
            return;
        }

        foreach (TileObject retreatTile in tiles)
        {
            retreatTile.SetHighlight(Color.red, (Vector2Int tilePosition) =>
            {
                TileObject selectedTile = HexTilemap.Instance.GetTile(tilePosition);
                selectedTile.SetOccupiedCharacter(enemy);
                tile.SetOccupiedCharacter(null);
                enemy.transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, enemy.transform.position.z);
                ResetHighlightedTiles();
                onEnd();
            });
        }
    }


    private void RandomPush(Vector2Int enemyPosition, int amount)
    {
        TileObject tile = HexTilemap.Instance.GetTile(enemyPosition);
        if (tile == null) return;
        if (tile.IsEmpty()) return;
        BaseCharacter enemy = tile.GetOccupyingCharacter();
        enemy.Retreat(amount, AxialPosition);
    }

    private void Pull(Vector2Int enemyPosition, int amount, Action onEnd)
    {
        TileObject tile = HexTilemap.Instance.GetTile(enemyPosition);
        if (tile == null) return;
        if (tile.IsEmpty()) return;
        BaseCharacter enemy = tile.GetOccupyingCharacter();
        List<TileObject> tiles = enemy.GetAdvanceTiles(AxialPosition, amount, amount);
        _highlitedTiles = tiles;


        if (tiles.Count == 1)
        {
            TileObject selectedTile = tiles[0];
            selectedTile.SetOccupiedCharacter(enemy);
            tile.SetOccupiedCharacter(null);
            enemy.transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, enemy.transform.position.z);
            onEnd();
            return;
        }

        foreach (TileObject retreatTile in tiles)
        {
            retreatTile.SetHighlight(Color.red, (Vector2Int tilePosition) =>
            {
                TileObject selectedTile = HexTilemap.Instance.GetTile(tilePosition);
                selectedTile.SetOccupiedCharacter(enemy);
                tile.SetOccupiedCharacter(null);
                enemy.transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, enemy.transform.position.z);
                ResetHighlightedTiles();
                onEnd();
            });
        }
    }

    private void RandomPull(Vector2Int enemyPosition, int amount)
    {
        TileObject tile = HexTilemap.Instance.GetTile(enemyPosition);
        if (tile == null) return;
        if (tile.IsEmpty()) return;
        BaseCharacter enemy = tile.GetOccupyingCharacter();
        enemy.Advance(amount, AxialPosition);
    }


}
