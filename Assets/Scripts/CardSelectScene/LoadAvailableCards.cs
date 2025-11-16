using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAvailbleCards : MonoBehaviour
{
    [SerializeField] private Transform SpawnCardObject;
    [SerializeField] private Transform SpawnHeroObject;
    [SerializeField] private HeroesSOList MGEHeroes;
    [SerializeField] private HeroesSOList FurryHeroes;

    [SerializeField] private UnitsSOList MGEUnits;
    [SerializeField] private UnitsSOList FurryUnits;

    [SerializeField] private Button test;
    private bool MGEFractionSelected=true; private const string MGEFractionSelectedString = "MGEFractionSelected";
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
        //send request
    }
    private void Awake()
    {
       // MGEFractionSelected = PlayerPrefs.GetInt(MGEFractionSelectedString, 0) == 1;

    }
    //parser
    private UnitSO GetUnitFromLis(string Name)
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
    private void SpawnCard(UnitSO cardData)
    {
        Transform card = Instantiate(SpawnCardObject);
        if (cardData.Fraction == Fraction.MGE)
            card.SetParent(SelectedCardLogic.Instance.mgeUnitsGroup);
        else
            card.SetParent(SelectedCardLogic.Instance.furryUnitsGroup);

        if (card.TryGetComponent<CardPrefDataForSpawn>(out var cardPrefDataForSpawn))
        {
            cardPrefDataForSpawn.InitializeUnit(cardData);
        }

    }
    private void SpawnHeroes(HeroesSO heroData)
    {
        Transform card = Instantiate(SpawnHeroObject);
        if (heroData.Fraction == Fraction.MGE)
            card.SetParent(SelectedCardLogic.Instance.mgeHeroesGroup,false);
        else
            card.SetParent(SelectedCardLogic.Instance.furryHeroesGroup,false);

        if (card.TryGetComponent<CardPrefDataForSpawn>(out var cardPrefDataForSpawn))
        {
            cardPrefDataForSpawn.InitializeHero(heroData);
        }

    }

}
