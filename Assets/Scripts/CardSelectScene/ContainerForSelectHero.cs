using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerForSelectHero : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private Transform defaultCardTrans;
    private Transform cardObject;
    public Transform currentCard;
    private const float cardWidth = 190;
    private const float cardHeight = 260;
    [SerializeField] private string aditional;
   
    public void OnPointerClick(PointerEventData eventData)
    {
        cardObject = SelectedCardLogic.Instance.selectedHero;
       
        if (currentCard != null)
        {
            currentCard.SetParent(SelectedCardLogic.Instance.currentGroup);
            currentCard.GetComponent<CardPrefDataForSpawn>().enabled = true;
        }
        currentCard = cardObject;
        RectTransform recTransformOfCard = cardObject.GetComponent<RectTransform>();
        recTransformOfCard.sizeDelta = new Vector2(cardWidth, cardHeight);
        SetCardPosAndParentThisObject(cardObject);
        cardObject.GetComponent<CardPrefDataForSpawn>().enabled = false;

    }
    public void SetCardPosAndParentThisObject(Transform card)
    {
        card.SetParent(transform, false);
        card.transform.position = Vector2.zero;
    }
    
}
