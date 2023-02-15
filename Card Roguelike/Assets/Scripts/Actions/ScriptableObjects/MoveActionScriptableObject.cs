using UnityEngine;

namespace Actions.ScriptableObjects
{
    [CreateAssetMenu(fileName = "MoveActionScriptableObject", menuName = "Action/Move")]
    public class MoveActionScriptableObject : BaseActionScriptableObject
    {
        private void Awake()
        {
            type = ActionType.Move;
        }
    }
}
