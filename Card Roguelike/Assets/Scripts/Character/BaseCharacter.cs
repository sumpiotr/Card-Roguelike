using Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using Tilemap;
using Tilemap.Tile;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public abstract class BaseCharacter : MonoBehaviour, ICharacter
{

    private Vector2Int _axialPosition;

    protected int health;

    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public Vector2Int AxialPosition { get => _axialPosition; set => _axialPosition = value; }
    public int Health { get => health; set => health = value; }



    #region ResolveAction

    public void ResolveActionSet(List<ActionData> actionSet, int actionIndex = 0)
    {
        if (actionIndex == actionSet.Count) return;
        Action onActionResolved = () => { ResolveActionSet(actionSet, actionIndex + 1); };
        ResolveAction(actionSet[actionIndex], onActionResolved);
    }


    protected virtual void ResolveAction(ActionData actionData, Action onActionResolved)
    {
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
            case ActionType.Pull:
                PlayPull(actionData, onActionResolved);
                break;
            case ActionType.Advance:
                PlayAdvance(actionData, onActionResolved);
                break;
            case ActionType.Retreat:
                PlayRetreat(actionData, onActionResolved);
                break;
            default:
                onActionResolved();
                break;
        }
    }

    public void Advance(int value, Vector2Int targetAxialPosition)
    {
        TileObject tile = HexTilemap.Instance.GetTile(AxialPosition);
        int amount = value;
        List<TileObject> tiles = GetAdvanceTiles(targetAxialPosition, amount, amount);
        if (tiles.Count == 0)
        {
            amount--;
            while (amount > 0)
            {
                tiles = GetAdvanceTiles(targetAxialPosition, amount, amount);
                if (tiles.Count > 0) break;
                amount--;
            }
        }

        if (tiles.Count == 0) return;

        if (tiles.Count == 1)
        {
            TileObject selectedTile = tiles[0];
            selectedTile.SetOccupiedCharacter(this);
            tile.SetOccupiedCharacter(null);
            transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, transform.position.z);
            return;
        }
        else
        {
            TileObject selectedTile = tiles[Random.Range(0, tiles.Count)];
            selectedTile.SetOccupiedCharacter(this);
            tile.SetOccupiedCharacter(null);
            transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, transform.position.z);
            return;
        }
    }

    public void Retreat(int value, Vector2Int targetAxialPosition)
    {
        TileObject tile = HexTilemap.Instance.GetTile(AxialPosition);
        int amount = value;
        List<TileObject> tiles = GetRetretTiles(targetAxialPosition, amount, amount);
        if (tiles.Count == 0)
        {
            amount--;
            while (amount > 0)
            {
                tiles = GetRetretTiles(targetAxialPosition, amount, amount);
                if (tiles.Count > 0) break;
                amount--;
            }
        }

        if (tiles.Count == 0) return;

        TileObject selectedTile = null;
        if (tiles.Count == 1)selectedTile = tiles[0];
        else selectedTile = tiles[Random.Range(0, tiles.Count)];
        selectedTile.SetOccupiedCharacter(this);
        tile.SetOccupiedCharacter(null);
        transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, transform.position.z);
    }

    protected virtual void PlayAdvance(ActionData actionData, Action onResolved)
    {

    }

    protected virtual void PlayRetreat(ActionData actionData, Action onResolved)
    {

    }

    protected virtual void PlayAttack(ActionData actionData, Action onResolved)
    {
        onResolved();
    }

    protected virtual void PlayMove(ActionData actionData, Action onResolved)
    {
        onResolved();
    }

    protected virtual void PlayPush(ActionData actionData, Action onResolved)
    {
        onResolved();
    }

    protected virtual void PlayPull(ActionData actionData, Action onResolved)
    {
        onResolved();
    }



    protected virtual void PlayBuff(ActionData actionData, Action onResolved)
    {
        onResolved();
    }


    #endregion


    public bool CanHit(Vector2Int position, bool hitThroughCharacters = false)
    {
        TileObject tile1 = HexTilemap.Instance.GetTile(AxialPosition);
        TileObject tile2 = HexTilemap.Instance.GetTile(position);

        Vector3 direction = (tile2.transform.position - tile1.transform.position).normalized;
        RaycastHit2D[] hit = Physics2D.RaycastAll(tile1.transform.position, direction, Vector2.Distance(tile2.transform.position, tile1.transform.position));

        foreach (RaycastHit2D hit2 in hit)
        {
            if (hit2.collider == null) continue;

          

            TileObject tile = hit2.transform.gameObject.GetComponent<TileObject>();
            if (tile == null) continue;
            if (tile == tile1 || tile == tile2) continue;
            if (!tile.IsWalkable()) return false;
            if (!tile.IsEmpty() && !hitThroughCharacters) return false;
        }

        return true;
    }

    public List<TileObject> GetRetretTiles(Vector2Int enemyPosition, int minRange, int maxRange)
    {
        List<TileObject> retretTiles = new List<TileObject>();


        Vector3 enemyTilePosition = HexTilemap.Instance.GetTile(enemyPosition).transform.position;

        Vector3 direction = (transform.position - enemyTilePosition).normalized;

        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction, Vector2.Distance(transform.position, enemyPosition));



        foreach (RaycastHit2D hit2 in hit)
        {
            if (hit2.collider == null) continue;
            TileObject tile = hit2.transform.gameObject.GetComponent<TileObject>();
            if (tile == null) continue;
            if (tile.axialPosition == AxialPosition) continue;
            if (!tile.IsWalkable() || !tile.IsEmpty()) break;
            int tileDistance = HexTilemap.AxialDistance(AxialPosition, tile.axialPosition);
            if(tileDistance >= minRange && tileDistance <= maxRange) retretTiles.Add(tile);
        }




        return retretTiles;

    }

    public  List<TileObject> GetAdvanceTiles(Vector2Int enemyPosition, int minRange, int maxRange)
    {
        List<TileObject> advanceTiles = new List<TileObject>();


        Vector3 enemyTilePosition = HexTilemap.Instance.GetTile(enemyPosition).transform.position;

        Vector3 direction = (enemyTilePosition - transform.position).normalized;

        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction, Vector2.Distance(transform.position, enemyPosition));



        foreach (RaycastHit2D hit2 in hit)
        {
            if (hit2.collider == null) continue;
            TileObject tile = hit2.transform.gameObject.GetComponent<TileObject>();
            if (tile == null) continue;
            if (tile.axialPosition == AxialPosition) continue;
            if (!tile.IsWalkable() || !tile.IsEmpty()) break;
            int tileDistance = HexTilemap.AxialDistance(AxialPosition, tile.axialPosition);
            if (tileDistance >= minRange && tileDistance <= maxRange) advanceTiles.Add(tile);
        }




        return advanceTiles;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Auch");
    }
}
