
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Actions.ScriptableObjects 
{
    [CreateAssetMenu(fileName = "ActionSetScriptableObject", menuName = "Action/Deck")]
    public class ActionDeckScriptableObject : ScriptableObject
    {
        public List<ActionSetScriptableObject> actionSets;
    } 
}

