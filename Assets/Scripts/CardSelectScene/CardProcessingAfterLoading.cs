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

        public Transform MGE_Heroes_Container;
        public Transform Furry_Heroes_Container;

        public void Start()
        {
            loader.loadingCardEnded += Loader_End_Loading_Card;

        }

        private void Loader_End_Loading_Card(object sender, EventArgs e)
        {

            foreach (UnitDeserialized unit in loader.loadedFurryUnits)
            {

                if (unit.Id != 0)
                {
                    SetCardToSelectedParent(unit.Card, unit.Id);
                }
                else
                {
                    SetCardToMeasureOfCard(unit.Card);
                }
            }
            foreach (UnitDeserialized unit in loader.loadedMGEUnits)
            {

                if (unit.Id != 0)
                {
                    SetCardToSelectedParent(unit.Card, unit.Id);
                }
                else
                {
                    SetCardToMeasureOfCard(unit.Card);
                }
            } 
            foreach (HeroDeserialized unit in loader.loadedMGEHeroes)
            {
                if (unit.isSelected)
                {
                    SetHeroToSelectedParent(unit.Hero);
                }
                else
                {
                    SetCardToHeroesPanelParent(unit.Hero);
                }
            }
            foreach (HeroDeserialized unit in loader.loadedFurryHeroes)
            {
                if (unit.isSelected)
                {
                    SetHeroToSelectedParent(unit.Hero);
                }
                else
                {
                    SetCardToHeroesPanelParent(unit.Hero);
                }
            }
        }
        private void SetHeroToSelectedParent(HeroesSO heroData)
        {
            
            if (heroData.Fraction == Fraction.MGE)
            {
                if (MGE_Heroes_Container.TryGetComponent<ContainerForSelectHero>(out ContainerForSelectHero cardContainer))
                {
                    cardContainer.SetCartAfterLoading(SpawnCardObject,heroData);
                }
            }
            else
            {
                if (Furry_Heroes_Container.TryGetComponent<ContainerForSelectHero>(out ContainerForSelectHero cardContainer))
                {
                    cardContainer.SetCartAfterLoading(SpawnCardObject,heroData);
                }
            }
        }

        private void SetCardToMeasureOfCard(UnitSO cardData)
        {
            Transform card = Instantiate(SpawnCardObject);
            if (cardData.Fraction == Fraction.MGE)
            {
                card.SetParent(SelectedCardLogic.Instance.mgeUnitsGroup,false);
            }
            else
                card.SetParent(SelectedCardLogic.Instance.furryUnitsGroup,false);

            if (card.TryGetComponent<CardPrefDataForSpawn>(out var cardPrefDataForSpawn))
            {
                cardPrefDataForSpawn.InitializeUnit(cardData);

            }

        } 
        private void SetCardToHeroesPanelParent(HeroesSO cardData)
        {

            Transform card = Instantiate(SpawnCardObject);
            if (cardData.Fraction == Fraction.MGE)
            {
                card.SetParent(SelectedCardLogic.Instance.mgeHeroesGroup,false);
            }
            else
            {
                card.SetParent(SelectedCardLogic.Instance.furryHeroesGroup,false);
            }
            if (card.TryGetComponent<CardPrefDataForSpawn>(out var cardPrefDataForSpawn))
            {
                cardPrefDataForSpawn.InitializeHero(cardData);
            }

        }
        private void SetCardToSelectedParent(UnitSO cardData, int i)
        {
            Transform card = Instantiate(SpawnCardObject);
            if (cardData.Fraction == Fraction.MGE)
            {
                card.SetParent(MGE_postition[i - 1],false);
                if (MGE_postition[i - 1].TryGetComponent<ContainerForCardPosition>(out ContainerForCardPosition cardContainer))
                {
                    cardContainer.SetSelectedCard(card);
                }
            }
            else
            {
                card.SetParent(FURRY_postition[i - 1], false);
                if (FURRY_postition[i - 1].TryGetComponent<ContainerForCardPosition>(out ContainerForCardPosition cardContainer))
                {
                    cardContainer.SetSelectedCard(card);
                }
            }
        }
    }
}
