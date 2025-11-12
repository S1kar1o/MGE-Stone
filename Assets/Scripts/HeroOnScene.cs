using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HeroOnScene : MonoBehaviourPunCallbacks
{
    public HeroesSO cardData;
    public Image cardImage;
    public TextMeshProUGUI Mana;
    public TextMeshProUGUI Hp;
    public Animator animator;
    public Fraction fraction;


    public void Initialize(HeroesSO heroData)
    {
        cardData = heroData;
        Debug.Log($"Initialize викликано для {gameObject.name} з heroData: {heroData?.name ?? "null"}");
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
            Debug.Log($"Успішно ініціалізовано: Hp={Hp.text}, Mana={Mana.text}");
        }
        else
        {
            Debug.LogError($"heroData null для {gameObject.name}");
        }
    }

    private void Start()
    {
        TurnManager.Instance.TurnChanged += On_TurnChanged;
    }

    private void On_TurnChanged(object sender, TurnManager.OnStateChangedEventArgs e)
    {
        if ((e.state == TurnManager.TurnState.YourSpawning && photonView.IsMine) ||
            (e.state == TurnManager.TurnState.EnemySpawning && !photonView.IsMine))
        {
            photonView.RPC("UpdateManaValue", RpcTarget.All, e.state);
        }
    }

    [PunRPC]
    private void UpdateManaValue(TurnManager.TurnState state)
    {
        if (state == TurnManager.TurnState.YourSpawning)
        {
            Mana.text = (int.Parse(Mana.text) + 2).ToString();
        }
    }

    public void GetDamage(int damage)
    {
        if (damage >= int.Parse(Hp.text))
        {
            CongratulationMenu.Instance.EndGamePanel(this);
            if (PhotonNetwork.IsMasterClient)
            {
                TurnManager.Instance.photonView.RPC("EndGame", RpcTarget.All, photonView.IsMine);
            }
        }
        else
        {
            animator.SetTrigger("GetDamage");
            Hp.text = (int.Parse(Hp.text) - damage).ToString();
        }
    }

    
}