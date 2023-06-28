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

        private Action<Vector2Int> _onClick = null;


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
            if(character == null)return;
            character.AxialPosition = axialPosition;
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


        public void SetHighlight(Color color, Action<Vector2Int> onClick)
        {
            this._spriteRenderer.color = color;
            _onClick = onClick;
        }

        public void RemoveHighlight()
        {
            this._spriteRenderer.color = Color.white;
            _onClick = null;
        }

        private void OnMouseDown()
        {
           if(_onClick != null) _onClick(axialPosition);
        }
    }
}
