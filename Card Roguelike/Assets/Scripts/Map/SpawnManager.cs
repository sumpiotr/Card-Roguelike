using System.Collections;
using System.Collections.Generic;
using Tilemap;
using Tilemap.Tile;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance = null;

    [SerializeField]
    private Player playerPrefab;

    [SerializeField]
    private GameObject exitPrefab;

    public void Awake()
    {
        if (Instance != null) return;
        Instance = this;
    }

    public void SpawnPlayer(Vector2Int startPosition)
    {
        
        TileObject tile = HexTilemap.Instance.GetTileByIndexPosition(startPosition);
        Player player = Instantiate(playerPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y, -1), Quaternion.identity);
        tile.SetOccupiedCharacter(player);
        GameManager.Instance.SetPlayer(player);
    }

    public void SpawnExit(Vector2Int position)
    {
        TileObject tile = HexTilemap.Instance.GetTileByIndexPosition(position);
        GameObject exit = Instantiate(exitPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y, -1), Quaternion.identity);
    }
}
