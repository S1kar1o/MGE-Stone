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

    public List<UnitDeserialized> loadedMGEUnits;
    public List<UnitDeserialized> loadedFurryUnits;

    public List<HeroDeserialized> loadedMGEHeroes;
    public List<HeroDeserialized> loadedFurryHeroes;

    public class UnitDeserialized
    {
        public int Id { get; set; }
        public UnitSO Card { get; set; }
        public UnitDeserialized(int id, UnitSO card)
        {
            this.Id = id;
            this.Card = card;
        }
        public UnitDeserialized(UnitSO card) : this(0, card) { }
    }
    public class HeroDeserialized
    {
        public bool isSelected{ get; set; }
        public HeroesSO Hero { get; set; }
        public HeroDeserialized(bool isSelected, HeroesSO hero)
        {
            this.isSelected=isSelected;
            this.Hero = hero;
        }
        public HeroDeserialized(HeroesSO card) : this(false, card) { }
    }

    [System.Serializable]
    public class CardListResponse
    {
        public Guid UserId { get; set; }
        public CardObject[] Cards { get; set; }
        public HeroObject[] Heroes { get; set; }
    }

    [System.Serializable]
    public class CardObject
    {
        public string name;
        public int positionInDeck;
    }
    [System.Serializable]
    public class HeroObject
    {
        public string Name { get; set; }
        public bool isSelected { get; set; }
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
            loadedMGEUnits = new List<UnitDeserialized>();

            loadedFurryUnits = new List<UnitDeserialized>();

            loadedMGEHeroes = new List<HeroDeserialized>();

            loadedFurryHeroes = new List<HeroDeserialized>();

            var response = JsonUtility.FromJson<CardListResponse>(request.downloadHandler.text);
            Debug.Log(response);
            foreach (var loadedCard in response.Cards)
            {
                bool find = false;
                foreach (UnitSO unit in MGEUnits.UnitsSoList)
                {
                    Debug.Log(121);
                    if (unit.name == loadedCard.name)
                    {
                        Debug.Log($"{unit.name}");

                        if (loadedCard.positionInDeck == 0)
                        {
                            UnitDeserialized item = new UnitDeserialized(unit);
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
                            UnitDeserialized item = new UnitDeserialized(unit);
                            loadedFurryUnits.Add(item);
                            break;
                        }
                    }
                }
            }
            foreach(var loadedHeroes in response.Heroes)
            {
                bool find = false;
                foreach (HeroesSO unit in MGEHeroes.list)
                {
                    if (unit.name == loadedHeroes.Name)
                    {
                        Debug.Log($"{unit.name}");

                        if (loadedHeroes.isSelected)
                        {
                            HeroDeserialized item = new HeroDeserialized(unit);
                            loadedMGEHeroes.Add(item);
                            find = true;
                            break;
                        }

                    }
                }
                if (find)
                    continue;
                foreach (HeroesSO unit in FurryHeroes.list)
                {
                    if (unit.name == loadedHeroes.Name)
                    {
                        if (loadedHeroes.isSelected)
                        {
                            HeroDeserialized item = new HeroDeserialized(unit);
                            loadedFurryHeroes.Add(item);
                            break;
                        }
                    }
                }
            }
            Debug.Log("evvent");
            loadingCardEnded?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
    
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
