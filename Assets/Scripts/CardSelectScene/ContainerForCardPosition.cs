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
        cardObject = SelectedCardLogic.Instance.SelectedCard;
        if (cardObject.TryGetComponent<CardPrefDataForSpawn>(out CardPrefDataForSpawn cardDataForSpawn))
        {
            savedCard = cardDataForSpawn.cardData;
            indexUnitSOinList = SelectedCardLogic.Instance.unitsSOList.UnitsSoList.IndexOf(savedCard);
            PlayerPrefs.SetInt($"indexUnitSOinList_{unitID}", indexUnitSOinList);
            PlayerPrefs.Save();
            Debug.Log(PlayerPrefs.GetInt($"indexUnitSOinList_{unitID}", defaultValue));
        }
        if(currentCard!= null)
        {
            currentCard.SetParent(SelectedCardLogic.Instance.CardSequel);
        }
        currentCard= cardObject;
        SetCardPosAndParentThisObject(cardObject);

    }
    private void LoadCardFromUnitsList(int index)
    {
        if (cardObject == null)
        {
            Transform newCard= Instantiate(defaultCardTrans);
            if (newCard.TryGetComponent<CardPrefDataForSpawn>(out CardPrefDataForSpawn cardPrefData)){
                cardPrefData.cardData= SelectedCardLogic.Instance.unitsSOList.UnitsSoList[index];
                cardPrefData.Initialize();
                SetCardPosAndParentThisObject(newCard);
            }
        }
    }
    private void SetCardPosAndParentThisObject(Transform card)
    {
        card.SetParent(transform);
        card.transform.position = Vector2.zero;
    }
}
