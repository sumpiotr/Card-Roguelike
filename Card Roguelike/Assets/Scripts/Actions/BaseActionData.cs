using Character;
using UnityEngine;

namespace Actions
{
    [System.Serializable]
    public class ActionData
    {
        public ActionType type;
        public int value;
        public Stats stat;
        public RangeData range;
    }
}
