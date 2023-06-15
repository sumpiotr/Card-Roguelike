using UnityEngine;

namespace Actions
{
    public class AttackActionData : BaseActionData
    {

        public int minRange = 1;
        public int maxRange = 1;

        public bool areaWide;
       
        public AttackActionData()
        {
            SetActionType(ActionType.Attack);
        }
    }
}
