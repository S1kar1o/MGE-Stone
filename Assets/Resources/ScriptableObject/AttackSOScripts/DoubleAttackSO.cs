using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(menuName = "AttackActions/DoubleAttackSO")]
public class DoubleAttackSO : AttackActionSO
{
    public int attackTimes = 2;
    public async override Task Execute(GameObject attacker)
    {
        UnitOnScene attackerScript = attacker.GetComponent<UnitOnScene>();
        Transform lineTransform = attacker.transform.parent;
        LineScript lineScript = lineTransform.GetComponent<LineScript>();
        for (int i = 0; i < attackTimes; i++)
        {
            bool attacked = false;

            foreach (UnitOnScene unit in lineTransform.GetComponentsInChildren<UnitOnScene>())
            {
                if (unit.fraction != attackerScript.fraction && int.Parse(unit.Hp.text) > 0)
                {
                    await attackerScript.PlayAttackAnimationWithMove(unit.transform);
                    attacked = true;
                    break; 
                }
            }
            if(attacked)
            {
                continue;
            }
            if (attacker.GetComponent<UnitOnScene>().fraction == GameManager.Instance.GetEnemyHeroes().GetComponent<HeroOnScene>().fraction)
            {
                Transform owerHeroes = GameManager.Instance.GetOwnerHeroes();
                HeroOnScene owerHeroesOnScene = owerHeroes.GetComponent<HeroOnScene>();
                owerHeroesOnScene.GetDamage(int.Parse(attackerScript.Damage.text));
                await attackerScript.PlayAttackAnimationWithMove(owerHeroes.transform);

            }
            else
            {
                Transform enemyHeroes = GameManager.Instance.GetEnemyHeroes();
                HeroOnScene enemyHeroesOnScene = enemyHeroes.GetComponent<HeroOnScene>();
                enemyHeroesOnScene.GetDamage(int.Parse(attackerScript.Damage.text));
                await attackerScript.PlayAttackAnimationWithMove(enemyHeroes.transform);


            }
        }
    }
}
