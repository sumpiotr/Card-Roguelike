using UnityEngine;

namespace Actions
{
    [System.Serializable]
    public class BaseActionData
    {
        protected ActionType type;
        public int value;
        public bool targetOwner;

        public ActionType GetActionType()
        {
            return type;
        }

        public void SetActionType(ActionType type)
        {
            this.type = type;
        }
    }
}
