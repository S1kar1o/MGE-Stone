using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCardLogic : MonoBehaviour
{
    [SerializeField] private string sceneNameToChange;
    [SerializeField] private Button saveDeckButton;
    [SerializeField] private Button mgeFractionSelectedButton;
    [SerializeField] private Button furryFractionSelectedButton;
    [SerializeField] private Button unitsButton;
    [SerializeField] private Button heroesButton;

    [SerializeField] private Transform scrollViewElement;
    [SerializeField] public Transform mgeUnitsGroup; [SerializeField] public Transform mgeHeroesGroup;
    [SerializeField] public Transform furryUnitsGroup; [SerializeField] public Transform furryHeroesGroup;

    [SerializeField] public Transform currentGroup;

    [SerializeField] public Transform selectedCard;
    [SerializeField] public Transform selectedHero;
    [SerializeField] public UnitsSOList unitsSOList;


    [SerializeField] public HeroesSOList mgeHeroesListSO;
    [SerializeField] public HeroesSOList furryHeroesListSO;

    [SerializeField] private Transform groupOfMge;
    [SerializeField] private Transform groupOfFurry;
    public bool isMge = true;
    private bool isHeroes = false;
    private const string MGEFractionSelectedString = "MGEFractionSelected";
    public static SelectedCardLogic Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        //isMge = PlayerPrefs.GetInt(MGEFractionSelectedString, 0) == 1;
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
        saveDeckButton.onClick.AddListener(() =>
        {
            SaveSelectedCardToSaveManager();
            SceneLoader.Instance.LoadScene(sceneNameToChange);
        });
    }
    public void SetSelectedCard(Transform card)
    {
        selectedCard = card;
    }
    public void SetSelectedHero(Transform heroCard)
    {
        selectedHero = heroCard;
    }
    public void SwitchToGroup(Transform transform)
    {
        currentGroup.gameObject.SetActive(false);
        scrollViewElement.GetComponent<ScrollRect>().content = transform.GetComponent<RectTransform>();
        currentGroup = transform;
        transform.gameObject.SetActive(true);
        if (isMge)
        {
            groupOfMge.gameObject.SetActive(true);
            groupOfFurry.gameObject.SetActive(false);
        }
        else
        {
            groupOfMge.gameObject.SetActive(false);
            groupOfFurry.gameObject.SetActive(true);
        }
    }
    private void SaveSelectedCardToSaveManager()
    {
        List<UnitSO> list = new List<UnitSO>();
        HeroesSO hero = null;

        foreach (Transform card in isMge ? groupOfMge : groupOfFurry)
        {
            if (card.TryGetComponent<ContainerForCardPosition>(out ContainerForCardPosition cardContainer))
            {
                CardPrefDataForSpawn cardData = cardContainer.currentCard.GetComponent<CardPrefDataForSpawn>();

                list.Add(cardData.cardData);
            }
            else if (card.TryGetComponent<ContainerForSelectHero>(out ContainerForSelectHero container))
            {
                CardPrefDataForSpawn cardData = cardContainer.currentCard.GetComponent<CardPrefDataForSpawn>();

                hero = cardData.heroeSO;
            }
        }
        SaveSelectedCardsToGame.Instance.SaveSelectedCards(list, hero);
    }
}
