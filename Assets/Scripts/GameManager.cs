using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public HandManager handManager;
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
        Hero.GetComponent<HeroOnScene>().Initialize(HeroSo);
        OponentHero.GetComponent<HeroOnScene>().Initialize(OponentHeroSo);
    }
   
    public Transform GetEnemyHeroes()
    {
        return OponentHero;
    } public Transform GetOwerHeroes()
    {
        return Hero;
    }
}
