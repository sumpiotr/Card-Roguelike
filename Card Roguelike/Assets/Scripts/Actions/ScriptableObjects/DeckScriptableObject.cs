using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CardDataScriptableObject", menuName = "Card/Deck")]
    public class DeckScriptableObject : ScriptableObject
    {
        public List<DeckItemData> cards;
    }

    [System.Serializable]
    public class DeckItemData
    {
        public int count;
        public CardDataScriptableObject item;
    }
}
