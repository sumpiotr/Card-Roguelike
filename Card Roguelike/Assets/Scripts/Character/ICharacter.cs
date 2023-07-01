using UnityEngine;

public interface ICharacter
{
    public Vector2 Position { get; set; }
    public Vector2Int AxialPosition { get; set; }
    
    public int Health { get; set; }
}
