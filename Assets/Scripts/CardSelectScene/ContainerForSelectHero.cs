using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerForSelectHero : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private Transform defaultCardTrans;
    public Transform cardObject;
    private Transform currentCard;
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
       
        SetCardPosAndParentThisObject(cardObject);

    }
    public void SetCartAfterLoading(Transform heroTransPref,HeroesSO heroData)
    {
        Transform card = Instantiate(heroTransPref,Vector2.zero,Quaternion.identity, transform);
        card.TryGetComponent<CardPrefDataForSpawn>(out CardPrefDataForSpawn cardData);
        cardData.InitializeHero(heroData);

        currentCard = cardObject = card;
        SetCardPosAndParentThisObject(card);
    }
    public void SetCardPosAndParentThisObject(Transform card)
    {
        Debug.Log(card.GetComponent<CardPrefDataForSpawn>().heroeSO);
        card.SetParent(transform,false);
        RectTransform recTransformOfCard = card.GetComponent<RectTransform>();


        recTransformOfCard.sizeDelta = new Vector2(cardWidth, cardHeight);
        recTransformOfCard.anchoredPosition = Vector2.zero;

        cardObject.GetComponent<CardPrefDataForSpawn>().enabled = false;

    }

}
