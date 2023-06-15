using System.Collections.Generic;
using Tilemap.Tile;
using UnityEngine;

namespace Tilemap
{
    public class HexTilemap : MonoBehaviour
    {

        public static HexTilemap Instance = null;

        [SerializeField] private TileObject hexPrefab;

        //size in hex numer
        private int mapWidth = 10;
        private int mapHeight = 10;

        //hex size in unity unit
        [SerializeField] private float hexSize = 1f;

        private int[,] _neighbourDirections =  { { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 } };

        private Dictionary<Vector2Int, TileObject> _hexes = new Dictionary<Vector2Int, TileObject>();


        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public void GenerateTilemap(int width, int height)
        {
            mapWidth = width;
            mapHeight = height;
            for(int y = 0; y < mapHeight; y++) for (int x = 0; x < mapWidth; x++)
            {
                Vector2 size = hexPrefab.GetComponent<SpriteRenderer>().size;
                size *= hexSize;
                float offset = x % 2 == 1 ?  size.y/2  : 0;
                Vector2 position = new Vector2(0 + x *(size.x*3/4), mapHeight*size.y - y*size.y - offset);
                TileObject hex = Instantiate(hexPrefab, position, Quaternion.identity, transform);
                hex.transform.localScale = new Vector3(hexSize, hexSize, 1);
                //hex.name = x + "x" + y;

                int q = y;
                int r = x - (y - (y & 1)) / 2;
                _hexes.Add(new Vector2Int(q, r), hex);
                hex.name = q + "x" + r;
            }
        }

        public TileObject GetHex(Vector2Int axialPosition)
        {
            return _hexes.ContainsKey(axialPosition) ? _hexes[axialPosition] : null;
        }

        public TileObject GetHexByIndexPosition(Vector2Int offsetPosition)
        {
            int q = offsetPosition.y;
            int r = offsetPosition.x - (offsetPosition.y - (offsetPosition.y & 1)) / 2;
            Vector2Int axialPosition = new Vector2Int(q, r);
            return _hexes.ContainsKey(axialPosition) ? _hexes[axialPosition] : null;
        }

        public List<TileObject> GetNeighbours(Vector2Int axialPosition)
        {
            List<TileObject> neighbours = new List<TileObject>();
            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbourPosition = new Vector2Int(axialPosition.x + _neighbourDirections[i, 0], axialPosition.y + _neighbourDirections[i, 1]);
                if (_hexes.ContainsKey(neighbourPosition))
                {
                    neighbours.Add(_hexes[neighbourPosition]);
                }
            }
            return neighbours;
        }

        public List<TileObject> GetTileObjectsInRange(Vector2Int axialPosition, int range)
        {
            List<TileObject> hexesInRange = new List<TileObject>();
            for (int dx = -range; dx <= range; dx++)
            {
                int minDy = Mathf.Max(-range, -dx - range);
                int maxDy = Mathf.Min(range, -dx + range);
                for (int dy = minDy; dy <= maxDy; dy++)
                {
                    int dz = -dx - dy;
                    Vector2Int pos = new Vector2Int(axialPosition.x + dx, axialPosition.y + dz);
                    if (_hexes.ContainsKey(pos))hexesInRange.Add(_hexes[pos]);
                }
            }
            return hexesInRange;
        }
    }
}

