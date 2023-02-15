using Actions;
using Actions.ScriptableObjects;
using UnityEngine;

namespace Character
{
    public class CharacterBase : MonoBehaviour
    {

        //keep track of action order
        private readonly ResolveActionData _resolveActionData = new ResolveActionData();
        
        protected void ResolveActionSet(ActionSetScriptableObject actionSet)
        {
            if (_resolveActionData.IsResolving()) return;
            _resolveActionData.StartResolve(actionSet);
            ResolveAction();
        }

        protected void ResolveAction()
        {
            if (!_resolveActionData.IsResolving()) return;
            BaseActionScriptableObject action = _resolveActionData.GetNextAction();
            switch (action.GetActionType())
            {
                case ActionType.Attack:
                    ResolveAttack((AttackActionScriptableObject)action);
                    break;
                case ActionType.Move:
                    ResolveMove((MoveActionScriptableObject)action);
                    break;
                case ActionType.Buff:
                    ResolveBuff((BuffActionScriptableObject)action);
                    break;
            }
        }
        
        

        protected void ResolveMove(MoveActionScriptableObject moveAction)
        {
            ResolveAction();
        }

       protected void ResolveAttack(AttackActionScriptableObject attackAction)
        {
            ResolveAction();
        }

       protected void ResolveBuff(BuffActionScriptableObject buffAction)
        {
            ResolveAction();
        }
    }
}
