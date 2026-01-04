using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static LoadAvailbleCards;

public class RewardProcessing : MonoBehaviour
{
    private int indexOfHeroesInContainer = 1;
    private int indexOfUnitsInContainer = 0;
    [SerializeField] private UnitsSOList mgeUnits;
    [SerializeField] private UnitsSOList furryUnits;
    public void StartrocecingUnits(List<CardObject> cardsData)
    {
        List<UnitSO> unit = new List<UnitSO>();

        for (int i = cardsData.Count - 1; i >= 0; i--)
        {
            var card = cardsData[i];

            foreach (UnitSO mgeUnit in mgeUnits.UnitsSoList)
            {
                if (card.name == mgeUnit.name)
                {
                    unit.Add(mgeUnit);
                    cardsData.RemoveAt(i);
                    goto Next;
                }
            }

            foreach (UnitSO furryUnit in furryUnits.UnitsSoList)
            {
                if (card.name == furryUnit.name)
                {
                    unit.Add(furryUnit);
                    cardsData.RemoveAt(i);
                    goto Next;
                }
            }
        Next:;
        }


        for (int i = 0; i < unit.Count; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            CardPrefDataForSpawn initComp = child.transform.GetChild(indexOfUnitsInContainer).GetComponent<CardPrefDataForSpawn>();
            initComp.InitializeUnit(unit[i]);
        }
    }
    public void StartrocecingHeroes(HeroesSO[] cardsData)
    {
        for (int i = 0; i < cardsData.Length; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            CardPrefDataForSpawn initComp = child.transform.GetChild(indexOfHeroesInContainer).GetComponent<CardPrefDataForSpawn>();
            initComp.InitializeHero(cardsData[i]);
        }
    }
}
