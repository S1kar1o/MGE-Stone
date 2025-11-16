using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSelectedCardsToGame : MonoBehaviour
{
    private List<UnitSO> selectedUnits;
    private HeroesSO selectedHeroForGame;
    public static SaveSelectedCardsToGame Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public List<UnitSO> GetSelectedUnits()
    {
        return selectedUnits;
    }
    public HeroesSO GetSelectedHeroForGame()
    {
        return selectedHeroForGame;
    }
    public void SaveSelectedCards(List<UnitSO> selectedUnits,HeroesSO selectedHero)
    {
        this.selectedUnits = selectedUnits;
        selectedHeroForGame = selectedHero;
    }
}
