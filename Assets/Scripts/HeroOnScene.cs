using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TurnManager;

public class HeroOnScene : MonoBehaviour
{
    public HeroesSO cardData; // посилання на ScriptableObject
    public Image cardImage;
    public TextMeshProUGUI Mana;
    public TextMeshProUGUI Hp;
    public Animator animator;
    public Fraction fraction;
    public void Initialize(HeroesSO heroData)
    {
        cardData = heroData;
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardSprite;
            Hp.text = cardData.Hp.ToString();
            Mana.text = cardData.Mana.ToString();
            fraction = cardData.Fraction;
            if (animator != null && cardData.animatorController != null)
            {
                animator.runtimeAnimatorController = cardData.animatorController;
            }
        }
    }
    private void Start()
    {
        TurnManager.Instance.TurnChanged += On_TurnChanged;
    }

    private void On_TurnChanged(object sender, TurnManager.OnStateChangedEventArgs e)
    {
        UpdateManaValue(e);
    }
    private void UpdateManaValue(TurnManager.OnStateChangedEventArgs e)
    {
        if (e.state == TurnState.YourSpawning)
        {
            Mana.text = (int.Parse(Mana.text) + 2).ToString();
        }
    }
    public void GetDamage(int damage)
    {
        if (damage >= int.Parse(Hp.text))
        {
            //Lose
        }
        else
        {
            Hp.text = (int.Parse(Hp.text) - damage).ToString();
        }

    }
}
