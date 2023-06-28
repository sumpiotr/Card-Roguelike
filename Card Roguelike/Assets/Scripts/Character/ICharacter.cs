using Actions.ScriptableObjects;
using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public Vector2 Position { get; set; }
    public Vector2Int AxialPosition { get; set; }
    

}
