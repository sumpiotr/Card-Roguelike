using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;

public class Tile : MonoBehaviour
{
    public bool walkable = true;
    public CharacterBase occupyingCharacter;

    public bool IsOccupied()
    {
        return occupyingCharacter == null;
    }
   
}
