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
    private Enemy enemyPrefab;

    [SerializeField]
    private GameObject exitPrefab;

    [SerializeField]
    private EncountersListScriptableObject encounters;


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
        Instantiate(exitPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y, -1), Quaternion.identity);
    }

    public void SpawnEnemies(Room room)
    {
        EncounterListItem encounter = encounters.GetRandomEncounter();
        foreach(EnemyDataScriptableObject enemyData in encounter.enemies)
        {
            int safe = 0;
            TileObject tile = null;
            while (tile == null)
            {
                Vector2Int position = new Vector2Int(Random.Range(room.center.x - ((room.size.size.x) / 2), room.center.x + ((room.size.size.x) / 2)), Random.Range(room.center.y - ((room.size.size.y) / 2), room.center.y + ((room.size.size.y ) / 2)));
                TileObject tmp = HexTilemap.Instance.GetTileByIndexPosition(position);
                if (tmp != null && tmp.IsEmpty() && tmp.IsWalkable()) {
                    tile = tmp;
                    break;
                }
                safe++;
                if (safe == 100) break;
            }
            if(tile != null)
            {
                Enemy enemy = Instantiate(enemyPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y, -1), Quaternion.identity);
                enemy.SetupData(enemyData);
                tile.SetOccupiedCharacter(enemy);
            }
        }
    }
}
