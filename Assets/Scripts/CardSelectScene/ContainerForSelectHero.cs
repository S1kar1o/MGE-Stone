using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerForSelectHero : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private Transform defaultCardTrans;
    private Transform cardObject;
    private Transform currentCard;
    private const float cardWidth = 190;
    private const float cardHeight = 260;
    [SerializeField] private string aditional;
    private HeroesSO savedCard;
    private int indexHeroSOinList, defaultValue = -1;
    private void Start()
    {
        // PlayerPrefs.SetInt($"indexHeroSOinList{aditional}", -1); 

        int index = PlayerPrefs.GetInt($"indexHeroSOinList" + aditional, defaultValue);
        Debug.Log(index);
        if (index != defaultValue)
        {
            LoadCardFromUnitsList(index);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        cardObject = SelectedCardLogic.Instance.selectedHero;
        if (cardObject.TryGetComponent<CardPrefDataForSpawn>(out CardPrefDataForSpawn cardDataForSpawn))
        {
            savedCard = cardDataForSpawn.heroeSO;
            if (SelectedCardLogic.Instance.isMge)
                indexHeroSOinList = SelectedCardLogic.Instance.mgeHeroesListSO.list.IndexOf(savedCard);
            else
                indexHeroSOinList = SelectedCardLogic.Instance.furryHeroesListSO.list.IndexOf(savedCard);

            PlayerPrefs.SetInt($"indexHeroSOinList" + aditional, indexHeroSOinList);
            PlayerPrefs.Save();
            Debug.Log(PlayerPrefs.GetInt($"indexHeroSOinList" + aditional, indexHeroSOinList));
        }
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
                if (SelectedCardLogic.Instance.isMge)
                {
                    savedCard = cardPrefData.heroeSO = SelectedCardLogic.Instance.mgeHeroesListSO.list[index];
                }
                else
                {
                    savedCard = cardPrefData.heroeSO = SelectedCardLogic.Instance.furryHeroesListSO.list[index];
                }
                cardPrefData.InitializeHero();
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
    public HeroesSO GetHeroSo()
    {
        return savedCard;
    }
}
