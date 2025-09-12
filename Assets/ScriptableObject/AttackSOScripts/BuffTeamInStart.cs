using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackActions/BuffTeamDamageOnStartSO")]
public class BuffTeamInStart : AbilitySO
{
    public int boosterDamage;
    public override async Task Execute(GameObject attacker)
    {
        UnitOnScene attackerUnitOnSceneScript = attacker.GetComponent<UnitOnScene>();
        Transform lineTransform = attacker.transform.parent;
        Transform lineControllerTransform= lineTransform.parent;
        foreach(Transform linesTransform in lineControllerTransform)
        {
            foreach (Transform unitTransform in linesTransform)
            {
                UnitOnScene unitScript = unitTransform.GetComponent<UnitOnScene>();
                if (unitScript.fraction == attackerUnitOnSceneScript.fraction)
                {
                    if (attacker.transform != unitTransform)
                    {
                        unitScript.boostDamage(boosterDamage);
                    }
                }
            }

        }
    }
}
