using Actions.ScriptableObjects;

namespace Character
{
    public class ResolveActionData
    {
        private ActionSetScriptableObject _resolvingSet = null;
        private bool _resolving = false;
        private int _currentActionIndex = 0;

        public void StartResolve(ActionSetScriptableObject actionSet)
        {
            if (actionSet == null) return;
            _resolvingSet = actionSet;
            _currentActionIndex = 0;
            _resolving = true;
        }

        public void StartResolve()
        {
            if (_resolvingSet == null) return;
            _currentActionIndex = 0;
            _resolving = true;
        }

        public bool IsResolving()
        {
            return _resolving;
        }

        public BaseActionScriptableObject GetNextAction()
        {
            if (_resolvingSet == null) return null;
            BaseActionScriptableObject nextAction = _resolvingSet.actions[_currentActionIndex];
            _currentActionIndex++;

            if (_currentActionIndex >= _resolvingSet.actions.Count) _resolving = false;
            
            
            return nextAction;
        }
    }
}
