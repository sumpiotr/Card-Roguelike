using UnityEngine;

namespace Actions.ScriptableObjects
{
    
    public class BaseActionScriptableObject : ScriptableObject
    {
        protected ActionType type;
        public int value;
        public bool targetOwner;

        public ActionType GetActionType()
        {
            return type;
        }
    }
}
