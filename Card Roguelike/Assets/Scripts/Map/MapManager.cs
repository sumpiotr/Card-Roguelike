using Tilemap;
using Tilemap.Tile;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {

        [SerializeField]
        private int width = 20;
    
        [SerializeField]
        private int height =  20;

        [SerializeField] private TileDataScriptableObject wallData;
    
        void Start()
        {
            Debug.Log(HexTilemap.Instance);
            HexTilemap.Instance.GenerateTilemap(width, height);
            int[,] map = PrimMapGenerator.GenerateMap(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[i, j] == 1)
                    {
                        HexTilemap.Instance.GetHexByIndexPosition(new Vector2Int(i, j)).SetData(wallData);
                    }
                }
            }
        }
    }
}
