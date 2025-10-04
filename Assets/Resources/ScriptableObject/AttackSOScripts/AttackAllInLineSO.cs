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
            foreach (UnitOnScene child in lineTransform.GetComponentsInChildren<UnitOnScene>())
            {
                if (child.fraction != attackerUnitOnScene.fraction && int.Parse(child.Hp.text) > 0)
                {
                    enemyInRow = true;
                    await attackerUnitOnScene.PlayAttackAnimationWithMove(child.transform);

                }
            }
            if (!enemyInRow)
            {
                if (attacker.GetComponent<UnitOnScene>().fraction == GameManager.Instance.GetEnemyHeroes().GetComponent<HeroOnScene>().fraction)
                {
                    Transform owerHeroes = GameManager.Instance.GetOwnerHeroes();
                    HeroOnScene owerHeroesOnScene = owerHeroes.GetComponent<HeroOnScene>();
                    owerHeroesOnScene.GetDamage(int.Parse(attackerUnitOnScene.Damage.text));
                    await attackerUnitOnScene.PlayAttackAnimationWithMove(owerHeroes.transform);

                }
                else
                {
                    Transform enemyHeroes = GameManager.Instance.GetEnemyHeroes();
                    HeroOnScene enemyHeroesOnScene = enemyHeroes.GetComponent<HeroOnScene>();

                    enemyHeroesOnScene.GetDamage(int.Parse(attackerUnitOnScene.Damage.text));
                    await attackerUnitOnScene.PlayAttackAnimationWithMove(enemyHeroes.transform);


                }
            }

        }
    }
}
