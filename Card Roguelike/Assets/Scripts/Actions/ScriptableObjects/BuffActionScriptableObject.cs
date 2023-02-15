using Character;
using UnityEngine;

namespace Actions.ScriptableObjects
{
   [CreateAssetMenu(fileName = "BuffActionScriptableObject", menuName = "Action/Buff")]
   public class BuffActionScriptableObject : BaseActionScriptableObject
   {
      public Stats stat;
      private void Awake()
      {
         type = ActionType.Buff;
      }
   }
}
