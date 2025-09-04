using UnityEngine;
using DG.Tweening;
using static TurnManager;

public class HandManager : MonoBehaviour
{
    public float handWidth = 1000f;
    public float startX = 0f;
    public float minOffset = 50f;
    public float moveDuration = 0.2f;
    public static HandManager Instance;
    public UnitsSOList unitSOList;
    public GameObject cardPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ���� ��� � ����� ���������, ��������� ���
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        TurnManager.Instance.TurnChanged += On_TurnChanged;
    }
    private void On_TurnChanged(object sender, TurnManager.OnStateChangedEventArgs e)
    {
        if(e.state== TurnState.YourSpawning)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        CardInHand card = Instantiate(cardPrefab, transform).GetComponent<CardInHand>();
        int index = Random.Range(0, unitSOList.UnitsSoList.Count);
        UnitSO randomUnit = unitSOList.UnitsSoList[index];
        card.cardData = randomUnit; // ��������� ���
        card.Initialize();

        UpdateHand();
    }

    public void UpdateHand()
    {
        int cardCount = transform.childCount;
        if (cardCount == 0) return;

        float offset = Mathf.Max(handWidth / cardCount, minOffset);
        float totalWidth = offset * (cardCount - 1);
        float leftStart = -totalWidth / 2;

        // ��������� �� ������� ����� ����������
        foreach (Transform card in transform)
        {
            card.DOKill(); // ��������� �� ������� (�������, �������, �������)
            CardInHand cardScript = card.GetComponent<CardInHand>();
            if (cardScript != null)
            {
                cardScript.DOKillAll(); // ��������� �������� ����
                cardScript.RefreshOriginalPosition(); // ��������� originalPosition
            }
        }

        // ��������� �������
        for (int i = 0; i < cardCount; i++)
        {
            Transform card = transform.GetChild(i);
            Vector3 targetPosition = new Vector3(startX + leftStart + i * offset, card.localPosition.y, 0);

            // ������������� ������������ �������, ���� ����� �� �������
            CardInHand cardScript = card.GetComponent<CardInHand>();
            if (cardScript != null && !cardScript.IsSelected) // ������ �������� �� ����
            {
                card.localPosition = targetPosition; // ������������ ������� ��� �������
                cardScript.RefreshOriginalPosition();
            }
            else
            {
                card.DOLocalMove(targetPosition, moveDuration);
            }
        }
    }
}
