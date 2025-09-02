using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public HandManager handManager;
    public GameObject cardPrefab;
    public Transform handPanel;
    public Button buttonTest;
    public UnitsSOList unitSOList;
    [SerializeField] private Transform Hero, OponentHero;
    public HeroesSO HeroSo, OponentHeroSo;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // якщо вже є інший екземпляр, видаляємо цей
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        buttonTest.onClick.AddListener(() =>
        {
            DrawCard();
        });
        Hero.GetComponent<HeroOnScene>().Initialize(HeroSo);
        OponentHero.GetComponent<HeroOnScene>().Initialize(OponentHeroSo);
    }
    public void DrawCard()
    {
        CardInHand card = Instantiate(cardPrefab, handPanel).GetComponent<CardInHand>();
        int index = Random.Range(0, unitSOList.UnitsSoList.Count);
        UnitSO randomUnit = unitSOList.UnitsSoList[index];
        card.cardData = randomUnit; // прив’язуємо дані
        card.Initialize();

        handManager.UpdateHand();
    }
    public Transform GetEnemyHeroes()
    {
        return OponentHero;
    } public Transform GetOwerHeroes()
    {
        return Hero;
    }
}
