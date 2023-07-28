using Tilemap;
using Tilemap.Tile;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {

        public static MapManager Instance = null;

        [SerializeField]
        private int width = 20;
    
        [SerializeField]
        private int height =  20;

        [SerializeField]
        private int minRoomWidth = 10;

        [SerializeField]
        private int minRoomHeight = 10;

        [SerializeField]
        private int dungeonWidth = 70;

        [SerializeField]
        private int dungeonHeight = 70;

        [SerializeField]
        private int roomOffset = 1;


        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public void GenerateMap()
        {
            Debug.Log(HexTilemap.Instance);
            HexTilemap.Instance.GenerateTilemap(dungeonWidth, dungeonHeight);
            DungeonGenerator.GenerateDungeon(dungeonWidth, dungeonHeight, minRoomWidth, minRoomHeight, roomOffset);
           
        }

        public Vector2Int GetMapSize()
        {
            return new Vector2Int(dungeonWidth, dungeonHeight);
        }
    }
}
