using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTilemap : MonoBehaviour
{

    public static HexTilemap Instance = null;

    [SerializeField] private GameObject hexPrefab;

    //size in hex numer
    [SerializeField] private int mapWidth = 10;
    [SerializeField] private int mapHeight = 10;

    //hex size in unity unit
    [SerializeField] private float hexSize = 1f;

    private int[,] neighbourDirections = new int[,] { { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 } };

    private Dictionary<Vector2Int, GameObject> hexes = new Dictionary<Vector2Int, GameObject>();


    private void Start()
    {
        if (Instance != null) return;
        Instance = this;
        GenerateTilemap();
    }

    private void GenerateTilemap()
    {
        for(int y = 0; y < mapHeight; y++) for (int x = 0; x < mapWidth; x++)
        {
                Vector2 size = hexPrefab.GetComponent<SpriteRenderer>().size;
                size *= hexSize;
                float offset = x % 2 == 1 ?  size.y/2  : 0;
                Vector2 position = new Vector2(0 + x *(size.x*3/4), mapHeight*size.y - y*size.y - offset);
                GameObject hex = Instantiate(hexPrefab, position, Quaternion.identity, transform);
                hex.transform.localScale = new Vector3(hexSize, hexSize, 1);
                //hex.name = x + "x" + y;

                int q = y;
                int r = x - (y - (y & 1)) / 2;
                hexes.Add(new Vector2Int(q, r), hex);
                hex.name = q + "x" + r;
            }
    }

    public GameObject GetHex(Vector2Int position)
    {
        return hexes.ContainsKey(position) ? hexes[position] : null;
    }

    public List<GameObject> GetNeighbours(Vector2Int position)
    {
        List<GameObject> neighbours = new List<GameObject>();
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbourPosition = new Vector2Int(position.x + neighbourDirections[i, 0], position.y + neighbourDirections[i, 1]);
            if (hexes.ContainsKey(neighbourPosition))
            {
                neighbours.Add(hexes[neighbourPosition]);
            }
        }
        return neighbours;
    }

    public List<GameObject> GetTilesInRange(Vector2Int position, int range)
    {
        List<GameObject> hexesInRange = new List<GameObject>();
        for (int dx = -range; dx <= range; dx++)
        {
            int minDy = Mathf.Max(-range, -dx - range);
            int maxDy = Mathf.Min(range, -dx + range);
            for (int dy = minDy; dy <= maxDy; dy++)
            {
                int dz = -dx - dy;
                Vector2Int pos = new Vector2Int(position.x + dx, position.y + dz);
                if (hexes.ContainsKey(pos))hexesInRange.Add(hexes[pos]);
            }
        }
        return hexesInRange;
    }
}

