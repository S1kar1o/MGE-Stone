using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadAvailbleCards : MonoBehaviour
{
    public event EventHandler loadingCardEnded;
    [SerializeField] private Transform SpawnCardObject;
    [SerializeField] private Transform SpawnHeroObject;
    [SerializeField] private HeroesSOList MGEHeroes;
    [SerializeField] private HeroesSOList FurryHeroes;

    [SerializeField] private UnitsSOList MGEUnits;
    [SerializeField] private UnitsSOList FurryUnits;

    [SerializeField] private Button test;
    private bool MGEFractionSelected = true; private const string MGEFractionSelectedString = "MGEFractionSelected";

    public List<Item> loadedMGEUnits;
    public List<Item> loadedFurryUnits;

    public class Item
    {
        public int Id { get; set; }
        public UnitSO Card { get; set; }
        public Item(int id,UnitSO card)
        {
            this.Id = id;
            this.Card = card;
        } 
        public Item(UnitSO card):this(0,card) {}
    }

    [System.Serializable]
    public class CardListResponse
    {
        public string userId;
        public CardObject[] cards; // ⚠ НЕ List
    }

    [System.Serializable]
    public class CardObject
    {
        public string name;
        public int positionInDeck; // ⚠ НЕ short
    }

    void Start()
    {
        /* if (!PlayerPrefs.HasKey(MGEFractionSelectedString))
         {
             PlayerPrefs.SetInt(MGEFractionSelectedString, MGEFractionSelected ? 1 : 0);
             PlayerPrefs.Save();
         }*/
        test.onClick.AddListener(() =>
        {
            int i = 0;
            SpawnHeroes(GetHeroFromLis(MGEHeroes.list[i].name));
        });
        string userId = PlayerPrefs.GetString("UserId", "");// AuthUIMnager перемісти раді бога
        GetUserCards(userId);
        //send request
    }
    private void Awake()
    {
        // MGEFractionSelected = PlayerPrefs.GetInt(MGEFractionSelectedString, 0) == 1;

    }
    public string serverUrl = "https://mge-server.onrender.com/cards";

    public void GetUserCards(string userId)
    {
        StartCoroutine(GetCardsFlow(userId));
    }

    private IEnumerator GetCardsFlow(string userId)
    {
        yield return StartCoroutine(GetCardsCoroutine(userId));
       // yield return StartCoroutine(CreateLoadedCard());
    }

    private IEnumerator GetCardsCoroutine(string userId)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{serverUrl}/{userId}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            loadedMGEUnits = new List<Item>();

            loadedFurryUnits = new List<Item>();

            var response = JsonUtility.FromJson<CardListResponse>(request.downloadHandler.text);
            foreach (var loadedCard in response.cards)
            {
                bool find = false;
                foreach (UnitSO unit in MGEUnits.UnitsSoList)
                {
                    if (unit.name == loadedCard.name)
                    {
                        Debug.Log($"{unit.name}");

                        if (loadedCard.positionInDeck == 0)
                        {
                            Item item = new Item(unit);
                            loadedMGEUnits.Add(item);
                            find = true;
                            break;
                        }

                    }
                }
                if (find)
                    continue;
                foreach (UnitSO unit in FurryUnits.UnitsSoList)
                {
                    if (unit.name == loadedCard.name)
                    {
                        if (loadedCard.positionInDeck == 0)
                        {
                            Item item = new Item(unit);
                            loadedFurryUnits.Add(item);
                            break;
                        }
                    }
                }
            }
            Debug.Log("evvent");
            loadingCardEnded?.Invoke(this,EventArgs.Empty);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
/*    private IEnumerator CreateLoadedCard()//errrrrrrrrrooooorrrrr
    {
        foreach (UnitSO unit in loadedMGEUnits.UnitsSoList)
            SpawnCard(unit);
        foreach (UnitSO unit in loadedFurryUnits.UnitsSoList)
            SpawnCard(unit);
        yield return null;
    }*/
    //parser
    private UnitSO GetUnitFromList(string Name)
    {
        if (MGEFractionSelected)
        {
            for (int i = 0; i < MGEUnits.UnitsSoList.Count; i++)
            {
                if (MGEUnits.UnitsSoList[i].name == Name)
                    return MGEUnits.UnitsSoList[i];
            }
        }
        else
        {
            for (int i = 0; i < FurryUnits.UnitsSoList.Count; i++)
            {
                if (FurryUnits.UnitsSoList[i].name == Name)
                    return FurryUnits.UnitsSoList[i];
            }
        }
        return null;
    }
    private HeroesSO GetHeroFromLis(string Name)
    {
        if (MGEFractionSelected)
        {
            for (int i = 0; i < MGEHeroes.list.Count; i++)
            {

                if (MGEHeroes.list[i].name == Name)
                    return MGEHeroes.list[i];
            }
        }
        else
        {
            for (int i = 0; i < FurryHeroes.list.Count; i++)
            {
                if (FurryHeroes.list[i].name == Name)
                    return FurryHeroes.list[i];
            }
        }
        return null;
    }
   
    private void SpawnHeroes(HeroesSO heroData)
    {
        Transform card = Instantiate(SpawnHeroObject);
        if (heroData.Fraction == Fraction.MGE)
            card.SetParent(SelectedCardLogic.Instance.mgeHeroesGroup, false);
        else
            card.SetParent(SelectedCardLogic.Instance.furryHeroesGroup, false);

        if (card.TryGetComponent<CardPrefDataForSpawn>(out var cardPrefDataForSpawn))
        {
            cardPrefDataForSpawn.InitializeHero(heroData);
        }

    }

}
