using UnityEngine;

namespace Actions.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AttackActionScriptableObject", menuName = "Action/Attack")]
    public class AttackActionScriptableObject : BaseActionScriptableObject
    {
        private void Awake()
        {
            type = ActionType.Attack;
        }
    }
}
