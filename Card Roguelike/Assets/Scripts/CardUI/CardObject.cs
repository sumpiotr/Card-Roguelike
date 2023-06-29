using Actions.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardObject : MonoBehaviour, IDragHandler, IDropHandler
{

    private CardDataScriptableObject _cardData = null;

    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    private Vector2 _startPosition = new Vector2(0, 0);



    #region Intefaces

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(eventData.position.x, eventData.position.y, -1);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(transform.localPosition.y - _startPosition.y > 100)
        {
            gameObject.SetActive(false);
            GameManager.Instance.GetPlayer().ResolveActionSet(_cardData.actions);
            return;
        }


        SetDefaultPosition();
    }



    #endregion

    public void SetDefaultPosition()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void setupCard(CardDataScriptableObject cardData)
    {
        this._cardData = cardData;
        titleText.text = cardData.cardName;
        descriptionText.text = cardData.description;
        cardImage.sprite = cardData.image;
    }

    private void OnEnable()
    {
        _startPosition = transform.localPosition;
    }
}
