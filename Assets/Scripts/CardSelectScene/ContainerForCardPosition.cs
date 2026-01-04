using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerForCardPosition : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform defaultCardTrans;
    private Transform cardObject;
    public Transform currentCard;
    private const float cardWidth = 140;
    private const float cardHeight = 167;
    public int unitID;


    public void OnPointerClick(PointerEventData eventData)
    {
        TrySetCartToContainer();
    }
    public void TrySetCartToContainer()
    {
        cardObject = SelectedCardLogic.Instance.selectedCard;

        if (currentCard != null)
        {
            currentCard.SetParent(SelectedCardLogic.Instance.currentGroup);
            currentCard.GetComponent<CardPrefDataForSpawn>().enabled = true;
        }
        currentCard = cardObject;
        SetCardPosAndParentThisObject(cardObject);
        RectTransform recTransformOfCard = cardObject.GetComponent<RectTransform>();
        recTransformOfCard.sizeDelta = new Vector2(cardWidth, cardHeight);
        cardObject.GetComponent<CardPrefDataForSpawn>().enabled = false;
    }
    private void SetCardPosAndParentThisObject(Transform card)
    {
        card.SetParent(transform,false);
        RectTransform recTransformOfCard = card.GetComponent<RectTransform>();


        recTransformOfCard.sizeDelta = new Vector2(cardWidth, cardHeight);
        recTransformOfCard.anchoredPosition = Vector2.zero;

    }
    public void SetSelectedCard(Transform card)
    {
        currentCard = card;
    }
}
