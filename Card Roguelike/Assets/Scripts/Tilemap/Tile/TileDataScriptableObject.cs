using UnityEngine;
namespace Tilemap.Tile
{
    [CreateAssetMenu(fileName = "TileData", menuName = "Tile")]
    public class TileDataScriptableObject : ScriptableObject
    {
        public bool walkable;
        public Sprite sprite;
    }
}
