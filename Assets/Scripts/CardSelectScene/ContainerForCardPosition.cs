using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerForCardPosition : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform defaultCardTrans;
    private Transform cardObject;
    private Transform currentCard;
    private const float cardWidth = 140;
    private const float cardHeight = 167;
    public int unitID;


    private UnitSO savedCard;
    private int indexUnitSOinList, defaultValue = -1;
    private void Start()
    {
        //PlayerPrefs.SetInt($"indexUnitSOinList_{unitID}", -1);

        int index = PlayerPrefs.GetInt($"indexUnitSOinList_{unitID}", defaultValue);
        Debug.Log(index);
        if (index != defaultValue)
        {
            LoadCardFromUnitsList(index);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        cardObject = SelectedCardLogic.Instance.selectedCard;
        if (cardObject.TryGetComponent<CardPrefDataForSpawn>(out CardPrefDataForSpawn cardDataForSpawn))
        {

            savedCard = cardDataForSpawn.cardData;
            indexUnitSOinList = SelectedCardLogic.Instance.unitsSOList.UnitsSoList.IndexOf(savedCard);
            PlayerPrefs.SetInt($"indexUnitSOinList_{unitID}", indexUnitSOinList);
            PlayerPrefs.Save();
            Debug.Log(PlayerPrefs.GetInt($"indexUnitSOinList_{unitID}", defaultValue));
        }
        if (currentCard != null)
        {
            currentCard.SetParent(SelectedCardLogic.Instance.cardSequel);
            currentCard.GetComponent<CardPrefDataForSpawn>().enabled = true;
        }
        currentCard = cardObject;
        SetCardPosAndParentThisObject(cardObject);
        RectTransform recTransformOfCard = cardObject.GetComponent<RectTransform>();
        recTransformOfCard.sizeDelta = new Vector2(cardWidth, cardHeight);
        cardObject.GetComponent<CardPrefDataForSpawn>().enabled = false;

    }
    private void LoadCardFromUnitsList(int index)
    {
        if (cardObject == null)
        {
            Transform newCard = Instantiate(defaultCardTrans);
            currentCard = newCard;
            RectTransform recTransformOfCard = newCard.GetComponent<RectTransform>();
            recTransformOfCard.sizeDelta = new Vector2(cardWidth, cardHeight);

            if (newCard.TryGetComponent<CardPrefDataForSpawn>(out CardPrefDataForSpawn cardPrefData))
            {
                cardPrefData.cardData = SelectedCardLogic.Instance.unitsSOList.UnitsSoList[index];
                cardPrefData.Initialize();
                cardPrefData.GetComponent<CardPrefDataForSpawn>().enabled = false;
                SetCardPosAndParentThisObject(newCard);
            }
        }
    }
    private void SetCardPosAndParentThisObject(Transform card)
    {
        card.SetParent(transform, false);
        card.transform.position = Vector2.zero;
    }
}
