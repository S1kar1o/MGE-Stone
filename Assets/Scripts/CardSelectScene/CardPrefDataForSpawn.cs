using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPrefDataForSpawn : MonoBehaviour
{
    private UnitSO cardData;
    public Image cardImage;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI Hp;
    public TextMeshProUGUI costText;


    public void Initialize(UnitSO unitSO)
    {
        cardData=unitSO;
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardSprite;
            Damage.text = cardData.Damage.ToString();
            Hp.text = cardData.Hp.ToString();
            costText.text = cardData.cost.ToString();
           
        }
    }
}
