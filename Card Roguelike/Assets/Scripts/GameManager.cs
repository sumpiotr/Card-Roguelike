using Camera;
using Map;
using System.Collections;
using System.Collections.Generic;
using Tilemap;
using Tilemap.Tile;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private Player playerPrefab;

    [SerializeField]
    private CameraController cameraController;

 

    void Start()
    {
        MapManager.Instance.GenerateMap();
        TileObject startTile = null;

        bool foundPosition = false;
        do
        {

            Vector2Int playerStartPosition = new Vector2Int(Random.Range(0, MapManager.Instance.GetMapSize().x), Random.Range(0, MapManager.Instance.GetMapSize().y));
             startTile = HexTilemap.Instance.GetHexByIndexPosition(playerStartPosition);
            if (startTile != null)
            {
                foundPosition = startTile.IsEmpty() && startTile.IsWalkable();
            }

        } while (!foundPosition);

        Player player = Instantiate(playerPrefab, new Vector3(startTile.transform.position.x, startTile.transform.position.y, -1), Quaternion.identity);
        startTile.SetOccupiedCharacter(player);
        cameraController.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cameraController.transform.position.z);
    }
}
