using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HealAttackSO : AttackActionSO
{
    public override async Task Execute(GameObject attacker)
    {
        Transform lineTransform = attacker.transform.parent;
        UnitOnScene attackerUnitOnScene = attacker.GetComponent<UnitOnScene>();
        if (lineTransform != null)
        {
            foreach (Transform child in lineTransform)
            {
                UnitOnScene unit = child.GetComponent<UnitOnScene>();
                if (unit.fraction == attackerUnitOnScene.fraction)
                {
                    await unit.GetHealth(child);
                }
            }
        }
    }
}
