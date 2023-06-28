using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CardDataScriptableObject", menuName = "Card/Card")]
    public class CardDataScriptableObject : ScriptableObject
    {
        public int cost = 0;
        public Sprite image;
        public string cardName;
        public string description;
        public List<ActionData> actions;
    }
}
