using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LineScript : MonoBehaviour
{
    private int heroOnLine = 0;
    private BoxCollider2D lineCollider;
    [SerializeField] private List<Vector2> enemyPositionOnLine = new List<Vector2>();

    private void Awake()
    {
        lineCollider = GetComponent<BoxCollider2D>();
        if (lineCollider == null)
        {
            Debug.LogError("LineScript requires a BoxCollider2D component!");
        }
    }

    public bool TryAddCardOnLine(UnityEngine.Transform card)
    {
        if (heroOnLine >= 2)
        {
            Debug.Log("Line is full! Cannot add more heroes.");
            return false;
        }

        CardInHand cardInHand = card.GetComponent<CardInHand>();
        UnityEngine.Transform unitOnSceneTransform = Instantiate(cardInHand.cardData.UnitPref);
        UnitOnScene unitOnScene = unitOnSceneTransform.GetComponent<UnitOnScene>();
        unitOnScene.cardData = cardInHand.cardData;
        unitOnSceneTransform.SetParent(transform, false);

        heroOnLine++;
        unitOnScene.Initialize();

        UpdateHeroPositions();

        return true;
    }

    private void Start()
    {
        UpdateHeroPositions();
    }
    public bool CheckPosibilityToAddACard()
    {
        return heroOnLine < 2;
    }

    public void UpdateHeroPositions()
    {
        if (heroOnLine == 0) return;

        Vector2 colliderSize = lineCollider.size;
        float halfHeight = colliderSize.y / 2f;

        // === Вороги ===
        List<UnityEngine.Transform> enemies = new List<UnityEngine.Transform>();
        foreach (UnityEngine.Transform child in transform)
        {
            var unit = child.GetComponent<UnitOnScene>();
            if (unit != null &&
                unit.fraction != GameManager.Instance.GetOwerHeroes().GetComponent<HeroOnScene>().fraction &&
                int.Parse(unit.Hp.text) > 0)
            {
                enemies.Add(child);
            }
        }

        if (enemies.Count == 1)
        {
            enemies[0].DOLocalMove(enemyPositionOnLine[0], 0.3f).SetEase(Ease.OutQuad);
        }
        else if (enemies.Count == 2)
        {
            enemies[0].DOLocalMove(enemyPositionOnLine[1], 0.3f).SetEase(Ease.OutQuad);
            enemies[1].DOLocalMove(enemyPositionOnLine[2], 0.3f).SetEase(Ease.OutQuad);
        }

        // === Союзні герої ===
        List<UnityEngine.Transform> allies = new List<UnityEngine.Transform>();
        foreach (UnityEngine.Transform child in transform)
        {
            var unit = child.GetComponent<UnitOnScene>();
            var hero = GameManager.Instance.GetOwerHeroes()?.GetComponent<HeroOnScene>();

            if (unit != null && hero != null && unit.fraction == hero.fraction)
            {
                allies.Add(child);
            }
        }

        if (allies.Count == 1)
        {
            allies[0].DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.OutQuad);
        }
        else if (allies.Count == 2)
        {
            float offset = halfHeight / 2f;
            allies[0].DOLocalMove(new Vector3(0, offset, 0), 0.3f).SetEase(Ease.OutQuad);
            allies[1].DOLocalMove(new Vector3(0, -offset, 0), 0.3f).SetEase(Ease.OutQuad);
        }
    }

    public async Task StartAttack()
    {
        foreach (UnityEngine.Transform child in transform)
        {
            if (child.GetComponent<UnitOnScene>() != null && child.GetComponent<UnitOnScene>().fraction == Fraction.MGE)
            {
                UnitOnScene unit = child.GetComponent<UnitOnScene>();
                await unit.Attack();
            }
        }
        foreach (UnityEngine.Transform child in transform)
        {
            if (child.GetComponent<UnitOnScene>() != null && child.GetComponent<UnitOnScene>().fraction == Fraction.Furry)
            {
                UnitOnScene unit = child.GetComponent<UnitOnScene>();
                await unit.Attack();
            }
        }
    }

    public void RemoveHero(UnityEngine.Transform hero)
    {
        if (hero.parent == transform)
        {
            heroOnLine--;
            UpdateHeroPositions();
        }
    }
}
