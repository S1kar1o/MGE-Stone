using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(menuName = "AttackActions/AttackAllLineWithHeroSO")]

public class AttackAllLineWithHeroSO : AttackActionSO
{
    public override async Task Execute(GameObject attacker)
    {
        Transform lineTransform = attacker.transform.parent;
        UnitOnScene attackerUnitOnScene = attacker.GetComponent<UnitOnScene>();
        if (lineTransform != null)
        {
            foreach (Transform child in lineTransform)
            {
                UnitOnScene unitOnScene = child.GetComponent<UnitOnScene>();
                if (unitOnScene.fraction != attackerUnitOnScene.fraction && int.Parse(unitOnScene.Hp.text) > 0)
                {
                    await attackerUnitOnScene.PlayAttackAnimationWithMove(unitOnScene.transform);

                }
            }

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
