using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/HealTeametsWhenTheirHpLowerThanProcent")]
public class HealTeametsWhenTheirHpLowerThanProcent : AbilitySO
{
    public float minimumProcentForHealing = 40;
    public int healAmount = 2;
    public override async Task Execute(GameObject attacker)
    {
        Transform lineTransform = attacker.transform.parent;
        UnitOnScene attackerUnitOnScene = attacker.GetComponent<UnitOnScene>();
        foreach (Transform everyLine in lineTransform.parent)
        {
            foreach (UnitOnScene unit in everyLine.GetComponentsInChildren<UnitOnScene>())
            {
                if (unit != attackerUnitOnScene && unit.fraction == attackerUnitOnScene.fraction)
                {
                    if ((float)int.Parse(unit.Hp.text) / unit.maxHp < (float)minimumProcentForHealing / 100f)
                    {
                        unit.GetHealth(healAmount);
                    }
                }
            }
        }
    }
}
