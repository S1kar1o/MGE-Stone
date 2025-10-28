using UnityEngine;
using static TurnManager;

public class OponentHandPanel : MonoBehaviour
{
    [SerializeField] private float maxSpacing = 200f; // відстань при малій кількості карт
    [SerializeField] private float minSpacing = 30f;  // мінімальна відстань, коли карт багато
    [SerializeField] private Transform enemyCardPref;
    private RectTransform panel;
    public static OponentHandPanel Instance;
    private void Awake()
    {
        panel = GetComponent<RectTransform>();
    }
    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
            Instance = this;

        AlignChildren();
    }

    public void AlignChildren()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float panelWidth = panel.rect.width;

        // беремо розмір картки (припускаємо, що всі однакові)
        RectTransform firstCard = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
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
            RectTransform child = transform.GetChild(i).GetChild(0).GetComponent<RectTransform>();
            Vector3 targetPos = new Vector3(startX + i * spacing + pivotOffset, 0, 0);
            child.localPosition = targetPos;
        }
    }
}
