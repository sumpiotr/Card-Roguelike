using System;
using Character;
using UnityEngine;

namespace Tilemap.Tile
{
    public class TileObject : MonoBehaviour
    {
        [SerializeField]
        private TileDataScriptableObject data;
    
        //public CharacterBase occupyingCharacter;
        private SpriteRenderer _spriteRenderer;


        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            LoadData();
        }

        /*public bool IsOccupied()
        {
            return occupyingCharacter == null;
        }*/

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
   
    }
}
