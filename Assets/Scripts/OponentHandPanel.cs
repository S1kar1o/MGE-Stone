using UnityEngine;
using static TurnManager;

public class OponentHandPanel : MonoBehaviour
{
    [SerializeField] private float maxSpacing = 200f; // відстань при малій кількості карт
    [SerializeField] private float minSpacing = 30f;  // мінімальна відстань, коли карт багато
    [SerializeField] private Transform enemyCardPref;
    private RectTransform panel;

    private void Awake()
    {
        panel = GetComponent<RectTransform>();
        TurnManager.Instance.TurnChanged += On_TurnChanged;
    }
    private void Start()
    {
        AlignChildren();
    }
    private void On_TurnChanged(object sender, TurnManager.OnStateChangedEventArgs e)
    {
        if(e.state== TurnState.EnemySpawning)
        {
            TryAddCardToEnemyHand();
            AlignChildren();
        }
    }

    public void TryAddCardToEnemyHand()
    {
        Debug.Log("TryAddCard");
        Instantiate(enemyCardPref,transform);
    }
  
    private void AlignChildren()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float panelWidth = panel.rect.width;

        // беремо розмір картки (припускаємо, що всі однакові)
        RectTransform firstCard = transform.GetChild(0).GetComponent<RectTransform>();
        float cardWidth = firstCard.rect.width;
        float pivotOffset = (0.5f - firstCard.pivot.x) * cardWidth;

        // доступний простір = ширина панелі мінус ширина картки
        float availableWidth = panelWidth - cardWidth;

        // реальна відстань між картками
        float spacing = (count > 1) ? availableWidth / (count - 1) : 0f;

        // обмежуємо spacing
        spacing = Mathf.Clamp(spacing, minSpacing, maxSpacing);

        // зсув зліва, щоб карти починались всередині панелі
        float startX = -panelWidth / 2f + cardWidth * firstCard.pivot.x;

        for (int i = 0; i < count; i++)
        {
            RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
            Vector3 targetPos = new Vector3(startX + i * spacing + pivotOffset, 0, 0);
            child.localPosition = targetPos;
        }
    }
}
