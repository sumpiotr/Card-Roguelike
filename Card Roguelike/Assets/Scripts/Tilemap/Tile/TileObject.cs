using System;
using System.Collections.Generic;
using Character;
using Unity.VisualScripting;
using UnityEngine;

namespace Tilemap.Tile
{
    public class TileObject : MonoBehaviour
    {
        [SerializeField]
        private TileDataScriptableObject data;
    
        private BaseCharacter _occupyingCharacter;
        private SpriteRenderer _spriteRenderer;

        public Vector2Int axialPosition;


        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            LoadData();
        }

        public bool IsEmpty()
        {
            return _occupyingCharacter == null;
        }

        public void SetOccupiedCharacter(BaseCharacter character)
        {
            _occupyingCharacter = character;
        }

        public bool IsWalkable()
        {
            return data.walkable;
        }

        public void SetData(TileDataScriptableObject data)
        {
            this.data = data;
        }

        private void LoadData()
        {
            _spriteRenderer.sprite = data.sprite;
        }

        private void OnMouseDown()
        {
            //HexTilemap.Instance.Track(axialPosition);
            Debug.Log(axialPosition.x + "x" + axialPosition.y);
            List<TileObject> tiles = HexTilemap.Instance.GetTileObjectsInRange(axialPosition, 3, 4);
            foreach(TileObject tile in tiles)
            {
                Debug.Log(tile.axialPosition.x + "x" + tile.axialPosition.y);
                tile.gameObject.SetActive(false);
            }
        }

    }
}
