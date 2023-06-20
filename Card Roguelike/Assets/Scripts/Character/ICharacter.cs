using Actions.ScriptableObjects;
using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public Vector2 Position { get; set; }
    //public Stats Stats { get; set; }
    public void ResolveActionSet(ActionSetScriptableObject actionSet);

}
