using Tilemap;
using Tilemap.Tile;
using UnityEngine;

public class BaseCharacter : MonoBehaviour, ICharacter
{

    private Vector2Int _axialPosition;

    protected int health;

    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public Vector2Int AxialPosition { get => _axialPosition; set => _axialPosition = value; }
    public int Health { get => health; set => health = value; }


    public bool CanHit(Vector2Int position)
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
            if (!tile.IsWalkable() || !tile.IsEmpty()) return false;
        }

        return true;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
    }
}
