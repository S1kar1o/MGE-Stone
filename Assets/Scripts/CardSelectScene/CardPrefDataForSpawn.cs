using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPrefDataForSpawn : MonoBehaviour, IPointerClickHandler
{
    public UnitSO cardData;
    public HeroesSO heroeSO;
    public Image cardImage;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI Hp;
    public TextMeshProUGUI costText;
    private bool IsSelected = false;

    public void InitializeUnit(UnitSO unitSO)
    {
        cardData = unitSO;
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardSprite;
            Damage.text = cardData.Damage.ToString();
            Hp.text = cardData.Hp.ToString();

        }
    }
    public void InitializeHero(HeroesSO heroeSO)
    {
        this.heroeSO = heroeSO;
        if (heroeSO != null)
        {
            cardImage.sprite = heroeSO.cardSprite;
            //Damage.text = heroeSO.Damage.ToString();
            Hp.text = heroeSO.Hp.ToString();
        }
    }
    public void InitializeHero()
    {
        InitializeHero(heroeSO);
    }
    public void InitializeUnit()
    {
        InitializeUnit(cardData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        IsSelected = !IsSelected;
        if (IsSelected)
        {
            if (heroeSO != null)
                SelectedCardLogic.Instance.selectedHero = transform;
            else
                SelectedCardLogic.Instance.selectedCard = transform;
        }
        else
        {
            if (heroeSO != null)
                SelectedCardLogic.Instance.selectedHero = null;
            else
                SelectedCardLogic.Instance.selectedCard = null;

        }
    }
    public void SetSelectedToFalse()
    {
        IsSelected=false;
    }
}
