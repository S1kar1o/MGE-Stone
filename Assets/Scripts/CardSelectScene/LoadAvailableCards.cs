using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadAvailbleCards : MonoBehaviour
{
    [SerializeField] private Transform SpawnCardObject;
    [SerializeField] private Transform ObjectForCards;
    [SerializeField] private UnitsSOList MGEUnits;
    [SerializeField] private UnitsSOList FYRRIUnits;

    [SerializeField] private Button test;
    private bool MGEFractionSelected; private const string MGEFractionSelectedString = "MGEFractionSelected";
    void Start()
    {
        if (!PlayerPrefs.HasKey(MGEFractionSelectedString))
        {
            PlayerPrefs.SetInt(MGEFractionSelectedString, MGEFractionSelected ? 1 : 0);
            PlayerPrefs.Save();
        }
        test.onClick.AddListener(() =>
        {
            int i =Random.Range(0, MGEUnits.UnitsSoList.Count);
            SpawnCard(GetUnitFromLis(MGEUnits.UnitsSoList[i].name));
        });
        //send request
    }
    private void Awake()
    {
        MGEFractionSelected = PlayerPrefs.GetInt(MGEFractionSelectedString, 0) == 1;

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
            for (int i = 0; i < FYRRIUnits.UnitsSoList.Count; i++)
            {
                if (FYRRIUnits.UnitsSoList[i].name == Name)
                    return FYRRIUnits.UnitsSoList[i];
            }
        }
        return null;
    }
    private void SpawnCard(UnitSO cardData)
    {
        Transform card = Instantiate(SpawnCardObject, ObjectForCards);
        if (card.TryGetComponent<CardPrefDataForSpawn>(out var cardPrefDataForSpawn))
        {
            cardPrefDataForSpawn.Initialize(cardData);
        }

    }

}
