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

    private bool MGEFractionSelected = true;

    public List<UnitDeserialized> loadedMGEUnits;
    public List<UnitDeserialized> loadedFurryUnits;

    public List<HeroDeserialized> loadedMGEHeroes;
    public List<HeroDeserialized> loadedFurryHeroes;

    public class UnitDeserialized
    {
        public int Id;
        public UnitSO Card;
        public UnitDeserialized(int id, UnitSO card)
        {
            this.Id = id;
            this.Card = card;
        }
        public UnitDeserialized(UnitSO card) : this(0, card) { }
    }
    public class HeroDeserialized
    {
        public bool isSelected;
        public HeroesSO Hero;
        public HeroDeserialized(bool isSelected, HeroesSO hero)
        {
            this.isSelected = isSelected;
            this.Hero = hero;
        }
        public HeroDeserialized(HeroesSO card) : this(false, card) { }
    }

    [System.Serializable]
    public class CardListResponse
    {
        public Guid userId;
        public CardObject[] cards;
        public HeroObject[] heroes;
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
        public string name;
        public bool isSelected;
    }

    void Start()
    {
        string userId = LoginLogic.instance.GetUserId();// AuthUIMnager перемісти раді бога
        GetUserCards(userId);
        //send request
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
            foreach (var loadedCard in response.cards)
            {
                bool find = false;
                foreach (UnitSO unit in MGEUnits.UnitsSoList)
                {
                    if (unit.name == loadedCard.name)
                    {

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
            foreach (var loadedHeroes in response.heroes)
            {
                bool find = false;
                foreach (HeroesSO unit in MGEHeroes.list)
                {
                    if (unit.name == loadedHeroes.name)
                    {
                        HeroDeserialized item = new HeroDeserialized(loadedHeroes.isSelected, unit);
                        loadedMGEHeroes.Add(item);
                        find = true;
                        break;
                    }
                }
                if (find)
                    continue;
                foreach (HeroesSO unit in FurryHeroes.list)
                {
                    if (unit.name == loadedHeroes.name)
                    {
                        HeroDeserialized item = new HeroDeserialized(loadedHeroes.isSelected, unit);
                        loadedFurryHeroes.Add(item);
                        break;
                    }
                }
            }
            loadingCardEnded?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
}
