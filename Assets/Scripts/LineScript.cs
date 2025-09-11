using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
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
        }
    }
    public async Task TryAddCardOnLine(UnitSO unitSO)
    {
        if (heroOnLine >= 2)
        {
            return ;
        }


        UnityEngine.Transform unitOnSceneTransform = Instantiate(unitSO.UnitPref);
        UnitOnScene unitOnScene = unitOnSceneTransform.GetComponent<UnitOnScene>();
        unitOnScene.cardData = unitSO;
        unitOnSceneTransform.SetParent(transform, false);

        heroOnLine++;
        _=unitOnScene.Initialize();
        List<Transform> allies = new List<Transform>();
        foreach (Transform transform in transform)
        {
            if (transform.GetComponent<UnitOnScene>().cardData.Fraction == unitSO.Fraction)
            {
                allies.Add(transform);
            }
        }
        allies = allies.OrderBy(x => (int)x.GetComponent<UnitOnScene>().cardData.Type).ToList();
        for (int i = 0; i < allies.Count; i++)
        {
            allies[i].SetSiblingIndex(i); // ставимо на відповідний індекс
        }
        await UpdateHeroPositions();
    }
    public bool CheckPosibilityToAddACard()
    {
        return heroOnLine < 2;
    }

    public async Task UpdateHeroPositions()
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
            await enemies[0].DOLocalMove(enemyPositionOnLine[0], 0.3f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
        }
        else if (enemies.Count == 2)
        {
            var tween1 = enemies[0].DOLocalMove(enemyPositionOnLine[1], 0.3f).SetEase(Ease.OutQuad);
            var tween2 = enemies[1].DOLocalMove(enemyPositionOnLine[2], 0.3f).SetEase(Ease.OutQuad);
            await Task.WhenAll(tween1.AsyncWaitForCompletion(), tween2.AsyncWaitForCompletion());
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
            await allies[0].DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
        }
        else if (allies.Count == 2)
        {
            float offset = halfHeight / 2f;
            var tween1 = allies[0].DOLocalMove(new Vector3(0, offset, 0), 0.3f).SetEase(Ease.OutQuad);
            var tween2 = allies[1].DOLocalMove(new Vector3(0, -offset, 0), 0.3f).SetEase(Ease.OutQuad);
            await Task.WhenAll(tween1.AsyncWaitForCompletion(), tween2.AsyncWaitForCompletion());
        }

    }
    public async Task StartAttack()
    {
        var allUnits = new List<UnitOnScene>();
        foreach (Transform child in transform)
        {
            var unit = child.GetComponent<UnitOnScene>();
            if (unit != null) allUnits.Add(unit);
        }
        // Спочатку свої
        foreach (var unit in allUnits.Where(u => u.fraction == Fraction.MGE))
        {
            if (int.Parse(unit.Hp.text) > 0)
                await unit.Attack();
        }

        // Потім вороги
        foreach (var unit in allUnits.Where(u => u.fraction == Fraction.Furry))
        {
            if (int.Parse(unit.Hp.text) > 0)
                await unit.Attack();
        }
    } public async Task StartUseSkills()
    {
        var allUnits = new List<UnitOnScene>();
        foreach (Transform child in transform)
        {
            var unit = child.GetComponent<UnitOnScene>();
            if (unit != null) allUnits.Add(unit);
        }
        // Спочатку свої
        foreach (var unit in allUnits.Where(u => u.fraction == Fraction.MGE))
        {
            if (int.Parse(unit.Hp.text) > 0)
                await unit.StartSkill();
        }

        // Потім вороги
        foreach (var unit in allUnits.Where(u => u.fraction == Fraction.Furry))
        {
            if (int.Parse(unit.Hp.text) > 0)
                await unit.StartSkill();
        }
    }

    public void RemoveHero(UnityEngine.Transform hero)
    {
        if (hero.parent == transform)
        {
            heroOnLine--;
        }
    }
}
