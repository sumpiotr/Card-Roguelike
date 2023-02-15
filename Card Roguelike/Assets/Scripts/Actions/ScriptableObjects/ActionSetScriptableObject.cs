using System.Collections.Generic;
using Actions.ScriptableObjects;
using UnityEngine;

namespace Actions.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ActionSetScriptableObject", menuName = "Action/Set")]
    public class ActionSetScriptableObject : ScriptableObject
    {
        public List<BaseActionScriptableObject> actions;
    }
}
