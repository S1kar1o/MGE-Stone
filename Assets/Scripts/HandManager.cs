using UnityEngine;
using DG.Tweening;
using static TurnManager;
using Photon.Pun;

public class HandManager : MonoBehaviourPunCallbacks
{
    public float handWidth = 1000f;
    public float startX = 0f;
    public float minOffset = 50f;
    public float moveDuration = 0.2f;
    public static HandManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // якщо вже є інший екземпляр, видаляємо цей
            return;
        }
        Instance = this;
    }
   
  

    public void UpdateHand()
    {
        int cardCount = transform.childCount;
        if (cardCount == 0) return;

        float offset = Mathf.Max(handWidth / cardCount, minOffset);
        float totalWidth = offset * (cardCount - 1);
        float leftStart = -totalWidth / 2;

        // Зупиняємо всі анімації перед оновленням
        foreach (Transform card in transform)
        {
            card.DOKill(); // Зупиняємо всі анімації (позиція, масштаб, поворот)
            CardInHand cardScript = card.GetComponent<CardInHand>();
            if (cardScript != null)
            {
                cardScript.DOKillAll(); // Зупиняємо позиційні твіни
                cardScript.RefreshOriginalPosition(); // Оновлюємо originalPosition
            }
        }

        // Оновлюємо позиції
        for (int i = 0; i < cardCount; i++)
        {
            Transform card = transform.GetChild(i);
            Vector3 targetPosition = new Vector3(startX + leftStart + i * offset, card.localPosition.y, 0);

            // Безпосередньо встановлюємо позицію, якщо карта не вибрана
            CardInHand cardScript = card.GetComponent<CardInHand>();
            if (cardScript != null && !cardScript.IsSelected) // Додаємо перевірку на вибір
            {
                card.localPosition = targetPosition; // Встановлюємо позицію без анімації
                cardScript.RefreshOriginalPosition();
            }
            else
            {
                card.DOLocalMove(targetPosition, moveDuration);
            }
        }
    }
}
