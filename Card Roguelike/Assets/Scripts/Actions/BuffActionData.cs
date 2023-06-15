using Character;
using UnityEngine;

namespace Actions
{
   //[CreateAssetMenu(fileName = "BuffActionScriptableObject", menuName = "Action/Buff")]
   public class BuffActionData : BaseActionData
   {
        public Stats stat;

        public BuffActionData()
        {
            SetActionType(ActionType.Buff);
        }
   }
}
