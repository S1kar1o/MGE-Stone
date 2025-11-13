using UnityEngine;
using UnityEngine.UI;

public class SelectedCardLogic : MonoBehaviour
{
    [SerializeField] private Button mgeFractionSelectedButton;
    [SerializeField] private Button furryFractionSelectedButton;
    [SerializeField] private Button unitsButton;
    [SerializeField] private Button heroesButton;

    [SerializeField] private Transform scrollViewElement;
    [SerializeField] private Transform mgeUnitsGroup; [SerializeField] private Transform mgeHeroesGroup;
    [SerializeField] private Transform furryUnitsGroup; [SerializeField] private Transform furryHeroesGroup;

    [SerializeField] private Transform currentGroup;

    [SerializeField] public Transform selectedCard;
    [SerializeField] public Transform cardSequel;
    [SerializeField] public UnitsSOList unitsSOList;

    private bool isMge = true;
    private bool isHeroes = false;
    private const string MGEFractionSelectedString = "MGEFractionSelected";
    public static SelectedCardLogic Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        isMge = PlayerPrefs.GetInt(MGEFractionSelectedString, 0) == 1;
        heroesButton.onClick.AddListener(() => { SwitchToGroup(isMge ? mgeHeroesGroup : furryHeroesGroup); isHeroes = true; });
        unitsButton.onClick.AddListener(() => { SwitchToGroup(isMge ? mgeUnitsGroup : furryUnitsGroup); isHeroes = false; });
        mgeFractionSelectedButton.onClick.AddListener(() =>
        {
            isMge = true;
            SwitchToGroup(isHeroes ? mgeHeroesGroup : mgeUnitsGroup);
        });
        furryFractionSelectedButton.onClick.AddListener(() =>
        {
            isMge = false;
            SwitchToGroup(isHeroes ? furryHeroesGroup : furryUnitsGroup);
        });
        SwitchToGroup(isMge ? isHeroes ? mgeHeroesGroup : mgeUnitsGroup : isHeroes ? furryHeroesGroup : furryUnitsGroup);
    }
    public void SetelectedCard(Transform card)
    {
        selectedCard = card;
    }
    public void SwitchToGroup(Transform transform)
    {
        currentGroup.gameObject.SetActive(false);
        scrollViewElement.GetComponent<ScrollRect>().content = transform.GetComponent<RectTransform>();
        currentGroup = transform;
    }
}
