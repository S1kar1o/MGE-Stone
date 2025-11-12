using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPrefDataForSpawn : MonoBehaviour, IPointerClickHandler
{
    public UnitSO cardData;
    public Image cardImage;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI Hp;
    public TextMeshProUGUI costText;
    private bool IsSelected = false;

    public void Initialize(UnitSO unitSO)
    {
        cardData = unitSO;
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardSprite;
            Damage.text = cardData.Damage.ToString();
            Hp.text = cardData.Hp.ToString();
            costText.text = cardData.cost.ToString();

        }
    }
    public void Initialize()
    {
        Initialize(cardData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        IsSelected = !IsSelected;
        if (IsSelected)
        {
            SelectedCardLogic.Instance.SelectedCard = gameObject.transform;
        }
    }
}
