using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static LoadAvailbleCards;

namespace Assets.Scripts.CardSelectScene
{
    public class CardProcessingAfterLoading : MonoBehaviour
    {
        [SerializeField] private LoadAvailbleCards loader;
        [SerializeField] private Transform SpawnCardObject;

        public List<Transform> MGE_postition = new List<Transform>();
        public List<Transform> FURRY_postition = new List<Transform>();
        public void Start()
        {
            loader.loadingCardEnded += Loader_End_Loading_Card;
            Debug.Log("evvent");

        }

        private void Loader_End_Loading_Card(object sender, EventArgs e)
        {
            Debug.Log("evvent");

            foreach (Item unit in loader.loadedFurryUnits)
            {
                Debug.Log(unit.ToString()+"furry");
                Debug.Log(unit.Id);

                if (unit.Id != 0)
                {
                    SetCardToSelectedParent(unit.Card, unit.Id);
                }
                else
                {
                    SetCardToMeasureOfCard(unit.Card);
                }
            }
            foreach (Item unit in loader.loadedMGEUnits)
            {
                Debug.Log(unit.ToString() + "MGE");
                Debug.Log(unit.Id);

                if (unit.Id != 0)
                {
                    SetCardToSelectedParent(unit.Card, unit.Id);
                }
                else
                {
                    Debug.Log(unit.Id + 12);

                    SetCardToMeasureOfCard(unit.Card);
                }
            }
        }

        private void SetCardToMeasureOfCard(UnitSO cardData)
        {
            Transform card = Instantiate(SpawnCardObject);
            Debug.Log(card.name+12);
            if (cardData.Fraction == Fraction.MGE)
            {
                card.SetParent(SelectedCardLogic.Instance.mgeUnitsGroup);
                Debug.Log(card.name + 12);
            }
            else
                card.SetParent(SelectedCardLogic.Instance.furryUnitsGroup);

            if (card.TryGetComponent<CardPrefDataForSpawn>(out var cardPrefDataForSpawn))
            {
                cardPrefDataForSpawn.InitializeUnit(cardData);
                Debug.Log(card.name + 12);

            }

        }
        private void SetCardToSelectedParent(UnitSO cardData, int i)
        {
            Transform card = Instantiate(SpawnCardObject);
            if (cardData.Fraction == Fraction.MGE)
            {
                card.SetParent(MGE_postition[i - 1]);
                if (MGE_postition[i - 1].TryGetComponent<ContainerForCardPosition>(out ContainerForCardPosition cardContainer))
                {
                    cardContainer.SetSelectedCard(card);
                }
            }
            else
            {
                card.SetParent(FURRY_postition[i - 1]);
                if (FURRY_postition[i - 1].TryGetComponent<ContainerForCardPosition>(out ContainerForCardPosition cardContainer))
                {
                    cardContainer.SetSelectedCard(card);
                }
            }
        }
    }
}
