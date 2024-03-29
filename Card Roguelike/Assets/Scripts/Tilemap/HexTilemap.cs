using System.Collections.Generic;
using Tilemap.Tile;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Tilemap
{
    public class HexTilemap : MonoBehaviour
    {

        public static HexTilemap Instance = null;

        [SerializeField] private TileObject tilePrefab;

        //size in hex numer
        private int mapWidth = 10;
        private int mapHeight = 10;

        //hex size in unity unit
        [SerializeField] private float hexSize = 1f;

        [SerializeField]
        private TileDataScriptableObject wallData;

        private int[,] _neighbourDirections =  { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { -1, 1 } };

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
                Vector2 size = tilePrefab.GetComponent<SpriteRenderer>().size;
                size *= hexSize;
                float offset = x % 2 == 1 ?  size.y/2  : 0;
                Vector2 position = new Vector2(0 + x *(size.x*3/4), mapHeight*size.y - y*size.y - offset);
                TileObject hex = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                hex.transform.localScale = new Vector3(hexSize, hexSize, 1);
           

                int q = x;
                int r = y - (x - (x & 1)) / 2;
                hex.axialPosition = new Vector2Int(q, r);
                _hexes.Add(new Vector2Int(q, r), hex);
                hex.name = q + "x" + r + "(" +  x + "x" + y + ")";
            }
        }

        public TileObject GetTile(Vector2Int axialPosition)
        {
            return _hexes.ContainsKey(axialPosition) ? _hexes[axialPosition] : null;
        }

        public TileObject GetTileByIndexPosition(Vector2Int offsetPosition)
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

        public List<TileObject> GetWalkableNeighbours(Vector2Int axialPosition)
        {
            List<TileObject> neighbours = new List<TileObject>();
            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbourPosition = new Vector2Int(axialPosition.x + _neighbourDirections[i, 0], axialPosition.y + _neighbourDirections[i, 1]);
                if (_hexes.ContainsKey(neighbourPosition))
                {
                    if(_hexes[neighbourPosition].IsWalkable()) neighbours.Add(_hexes[neighbourPosition]);
                }
            }
            return neighbours;
        }

        public List<Vector2Int> GetWalkableNeighboursPositions(Vector2Int axialPosition)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();
            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbourPosition = new Vector2Int(axialPosition.x + _neighbourDirections[i, 0], axialPosition.y + _neighbourDirections[i, 1]);
                if (_hexes.ContainsKey(neighbourPosition))
                {
                    if (_hexes[neighbourPosition].IsWalkable()) neighbours.Add(neighbourPosition);
                }
            }
            return neighbours;
        }

        public List<TileObject> GetWalkableAndEmptyNeighbours(Vector2Int axialPosition)
        {
            List<TileObject> neighbours = new List<TileObject>();
            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbourPosition = new Vector2Int(axialPosition.x + _neighbourDirections[i, 0], axialPosition.y + _neighbourDirections[i, 1]);
                if (_hexes.ContainsKey(neighbourPosition))
                {
                    TileObject tile = _hexes[neighbourPosition];
                    if (tile.IsWalkable() && tile.IsEmpty()) neighbours.Add(_hexes[neighbourPosition]);
                }
            }
            return neighbours;
        }

        public List<TileObject> GetTileObjectsInRange(Vector2Int startPosition, int range)
        {
            List<TileObject> hexesInRange = new List<TileObject>();
            for (int dx = -range; dx <= range; dx++)
            {
                int minDy = Mathf.Max(-range, -dx - range);
                int maxDy = Mathf.Min(range, -dx + range);
                for (int dy = minDy; dy <= maxDy; dy++)
                {
                    int dz = -dx - dy;
                    Vector2Int pos = new Vector2Int(startPosition.x + dx, startPosition.y + dz);
                    if (_hexes.ContainsKey(pos))hexesInRange.Add(_hexes[pos]);
                }
            }
            return hexesInRange;
        }

        public List<TileObject> GetTileObjectsInRange(Vector2Int startPosition, int minRange, int maxRange)
        {
            List<TileObject> tilesInRange = new List<TileObject>();
            for (int dx = -maxRange; dx <= maxRange; dx++)
            {
                for (int dy = Mathf.Max(-maxRange, -dx - maxRange); dy <= Mathf.Min(maxRange, -dx + maxRange); dy++)
                {
                    Vector2Int currentPos = new Vector2Int(startPosition.x + dx, startPosition.y + dy);
                    if (!_hexes.ContainsKey(currentPos)) continue;
                    float distance = AxialDistance(startPosition, currentPos);
                    if (distance >= minRange && distance <= maxRange)
                    {
                        TileObject tile = _hexes[currentPos];
                        tilesInRange.Add(tile);
                    }
                }
            }

            return tilesInRange;
        }


        public List<TileObject> GetWalkableAndEmptyTileObjectsInRange(Vector2Int startPosition, int range)
        {
            List<TileObject> hexesInRange = new List<TileObject>();
            for (int dx = -range; dx <= range; dx++)
            {
                int minDy = Mathf.Max(-range, -dx - range);
                int maxDy = Mathf.Min(range, -dx + range);
                for (int dy = minDy; dy <= maxDy; dy++)
                {
                    int dz = -dx - dy;
                    Vector2Int pos = new Vector2Int(startPosition.x + dx, startPosition.y + dz);
                 
                    if (_hexes.ContainsKey(pos)) {
                        TileObject tile = _hexes[pos];
                        if (tile.IsEmpty() && tile.IsWalkable())hexesInRange.Add(tile);
                    }
                }
            }
            return hexesInRange;
        }

        public List<TileObject> GetWalkableAndEmptyTileObjectsInRange(Vector2Int startPosition, int minRange, int maxRange)
        {
            List<TileObject> tilesInRange = new List<TileObject>();
            for (int dx = -maxRange; dx <= maxRange; dx++)
            {
                for (int dy = Mathf.Max(-maxRange, -dx - maxRange); dy <= Mathf.Min(maxRange, -dx + maxRange); dy++)
                {
                    Vector2Int currentPos = new Vector2Int(startPosition.x + dx, startPosition.y + dy);
                    if (!_hexes.ContainsKey(currentPos)) continue;
                    float distance = AxialDistance(startPosition, currentPos);
                    if (distance >= minRange && distance <= maxRange)
                    {
                        if (_hexes.ContainsKey(currentPos))
                        {
                            TileObject tile = _hexes[currentPos];
                            if (tile.IsEmpty() && tile.IsWalkable()) tilesInRange.Add(tile);
                        }
                    }
                }
            }

            return tilesInRange;
        }


        public List<TileObject> GetOccupiedTileObjectsInRange(Vector2Int startPosition, int minRange, int maxRange)
        {
            List<TileObject> tilesInRange = new List<TileObject>();
            for (int dx = -maxRange; dx <= maxRange; dx++)
            {
                for (int dy = Mathf.Max(-maxRange, -dx - maxRange); dy <= Mathf.Min(maxRange, -dx + maxRange); dy++)
                {
                    Vector2Int currentPos = new Vector2Int(startPosition.x + dx, startPosition.y + dy);
                    if (!_hexes.ContainsKey(currentPos)) continue;
                    float distance = AxialDistance(startPosition, currentPos);
                    if (distance >= minRange && distance <= maxRange)
                    {
                        if (_hexes.ContainsKey(currentPos))
                        {
                            TileObject tile = _hexes[currentPos];
                            if (!tile.IsEmpty()) tilesInRange.Add(tile);
                        }
                    }
                }
            }

            return tilesInRange;
        }

        public List<TileObject> GetWalkableAndEmptyTileObjectsInLines(Vector2Int startPosition, int range)
        {
            List<TileObject> hexesInRange = new List<TileObject>();
            for(int directionIndex = 0; directionIndex <= 6; directionIndex++)
            {
                for(int i = 1; i <= range; i++)
                {
                    TileObject tile = GetTile(new Vector2Int(startPosition.x + (_neighbourDirections[directionIndex, 0] * i), startPosition.y + (_neighbourDirections[directionIndex, 1] * i)));
                    if (tile == null) break;
                    if (!tile.IsWalkable() || !tile.IsEmpty()) break;
                    hexesInRange.Add(tile);
                }
            }
            return hexesInRange;
        }

        public List<TileObject> GetWalkableAndEmptyTileObjectsInLines(Vector2Int startPosition, int minRange, int maxRange)
        {
            List<TileObject> hexesInRange = new List<TileObject>();
            for (int directionIndex = 0; directionIndex < 6; directionIndex++)
            {
                for (int i = minRange; i <= maxRange; i++)
                {
                    TileObject tile = GetTile(new Vector2Int(startPosition.x + (_neighbourDirections[directionIndex, 0] * i), startPosition.y + (_neighbourDirections[directionIndex, 1] * i)));
                    if (tile == null) break;
                    if (!tile.IsWalkable() || !tile.IsEmpty()) break;
                    hexesInRange.Add(tile);
                }
            }
            return hexesInRange;
        }


        public void LoadMap(int[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 1)
                    {
                        HexTilemap.Instance.GetTileByIndexPosition(new Vector2Int(i, j)).SetData(wallData);
                    }
                }
            }

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 1)
                    {
                        TileObject tile = HexTilemap.Instance.GetTileByIndexPosition(new Vector2Int(i, j));
                        if (HexTilemap.Instance.GetWalkableNeighbours(tile.axialPosition).Count == 0) tile.gameObject.SetActive(false);
                    }
                }
            }
        }



        public static int AxialDistance(Vector2Int a, Vector2Int b) {
            Vector2Int vec = a - b;
            return (Mathf.Abs(vec.x)
              + Mathf.Abs(vec.x + vec.y)
              + Mathf.Abs(vec.y)) / 2;
        }
    }

}

