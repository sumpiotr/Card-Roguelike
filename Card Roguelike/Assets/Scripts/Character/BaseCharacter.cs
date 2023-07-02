using System.Collections.Generic;
using Tilemap;
using Tilemap.Tile;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseCharacter : MonoBehaviour, ICharacter
{

    private Vector2Int _axialPosition;

    protected int health;

    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public Vector2Int AxialPosition { get => _axialPosition; set => _axialPosition = value; }
    public int Health { get => health; set => health = value; }


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
    }

    private float DistanceLineSegmentPoint(Vector3 start, Vector3 end, Vector3 point)
    {
        var wander = point - start;
        var span = end - start;

        // Compute how far along the line is the closest approach to our point.
        float t = Vector3.Dot(wander, span) / span.sqrMagnitude;

        // Restrict this point to within the line segment from start to end.
        t = Mathf.Clamp01(t);

        Vector3 nearest = start + t * span;
        return (nearest - point).magnitude;
    }
}
