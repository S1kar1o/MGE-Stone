using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackActions/AttackAllInLineSO")]
public class AttackAllInLineSO : AttackActionSO
{
    public override async Task Execute(GameObject attacker)
    {
        Transform lineTransform = attacker.transform.parent;
        UnitOnScene attackerUnitOnScene = attacker.GetComponent<UnitOnScene>();
        if (lineTransform != null)
        {
            bool enemyInRow = false;
            foreach (Transform child in lineTransform)
            {
                UnitOnScene unitOnScene = child.GetComponent<UnitOnScene>();
                if (unitOnScene.fraction != attackerUnitOnScene.fraction && int.Parse(unitOnScene.Hp.text) > 0)
                {
                    await unitOnScene.GetDamage(int.Parse(attackerUnitOnScene.Damage.text));
                    enemyInRow = true;
                    await attackerUnitOnScene.PlayAttackAnimationWithMove(unitOnScene.transform);

                }
            }
            if (!enemyInRow)
            {
                if (attacker.GetComponent<UnitOnScene>().fraction == GameManager.Instance.GetEnemyHeroes().GetComponent<HeroOnScene>().fraction)
                {
                    Transform owerHeroes = GameManager.Instance.GetOwerHeroes();
                    HeroOnScene owerHeroesOnScene = owerHeroes.GetComponent<HeroOnScene>();
                    owerHeroesOnScene.GetDamage(int.Parse(attackerUnitOnScene.Damage.text));
                    await attackerUnitOnScene.PlayAttackAnimationWithMove(owerHeroes.transform);

                    Debug.Log(owerHeroesOnScene.name + int.Parse(attackerUnitOnScene.Damage.text));
                }
                else
                {
                    Transform enemyHeroes = GameManager.Instance.GetEnemyHeroes();
                    HeroOnScene enemyHeroesOnScene = enemyHeroes.GetComponent<HeroOnScene>();

                    enemyHeroesOnScene.GetDamage(int.Parse(attackerUnitOnScene.Damage.text));
                    await attackerUnitOnScene.PlayAttackAnimationWithMove(enemyHeroes.transform);

                    Debug.Log(enemyHeroesOnScene.name + int.Parse(attackerUnitOnScene.Damage.text));

                }
            }
            enemyInRow = true;

        }
    }
}
