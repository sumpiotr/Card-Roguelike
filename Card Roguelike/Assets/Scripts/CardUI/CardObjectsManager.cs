using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObjectsManager : MonoBehaviour
{
    public static CardObjectsManager Instance = null;

    [SerializeField]
    private GameObject cardContainer;

    private List<CardObject> cardObjects = new List<CardObject> ();

    private void Awake()
    {
        if(Instance != null) return;
        Instance = this;
    }

    private void Start()
    {
        foreach (CardObject card in this.cardContainer.gameObject.GetComponentsInChildren<CardObject>(true))
        {
            card.gameObject.SetActive(false);
            cardObjects.Add(card);
        }
    }

    public CardObject GetCardObject()
    {
        foreach(CardObject cardObject in cardObjects)
        {
            if (!cardObject.gameObject.activeSelf) { 
                cardObject.gameObject.SetActive(true);
                return cardObject; 
            }
        }
        return null;
    }

    public void HideCards()
    {
        cardContainer.SetActive(false);
    }

    public void ShowCards()
    {
        cardContainer.SetActive(true);
    }


}
