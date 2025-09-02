using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManagerUI : MonoBehaviour
{
    [SerializeField] private Transform OwnerHero, EnemyHero;
    [SerializeField] private Transform OwnerHand, EnemyHand;
    void Start()
    {
        TurnManager.Instance.TurnChanged += Animating_TurnChangedUI;
    }

    private void Animating_TurnChangedUI(object sender, TurnManager.OnStateChangedEventArgs e)
    {

        if (e.state == TurnManager.TurnState.YourSpawning)
        {
            foreach (Transform child in OwnerHand)
            {
                CardInHand card = child.GetComponent<CardInHand>();
                card.enabled = true;
            }
        }
        else if (e.state == TurnManager.TurnState.EnemySpawning)
        {
            foreach (Transform child in OwnerHand)
            {
                CardInHand card = child.GetComponent<CardInHand>();
                card.enabled = false;
            }
        }

    }

}
