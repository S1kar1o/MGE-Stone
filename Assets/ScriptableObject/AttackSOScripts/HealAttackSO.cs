using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackActions/AttackWithHeal")]
public class HealAttackSO : AttackActionSO
{
    public override async Task Execute(GameObject attacker)
    {
        Transform lineTransform = attacker.transform.parent;
        UnitOnScene attackerUnitOnScene = attacker.GetComponent<UnitOnScene>();
        if (lineTransform != null)
        {
            bool isSomeOneFriendOnine = false;
            foreach (Transform child in lineTransform)
            {
                UnitOnScene unit = child.GetComponent<UnitOnScene>();
                if (unit.fraction == attackerUnitOnScene.fraction&&child.gameObject!=attacker)
                {
                    await unit.GiveHealth(child);
                    isSomeOneFriendOnine = true;
                }
            } 
            if (isSomeOneFriendOnine)
            {
                await attacker.GetComponent<UnitOnScene>().GiveHealth(attacker.transform);
            }
            foreach (Transform child in lineTransform)
            {
                UnitOnScene unit = child.GetComponent<UnitOnScene>();
                if (unit.fraction != attackerUnitOnScene.fraction)
                {
                    await unit.PlayAttackAnimationWithMove(child);
                    break;
                }
            }

        }
    }
}
