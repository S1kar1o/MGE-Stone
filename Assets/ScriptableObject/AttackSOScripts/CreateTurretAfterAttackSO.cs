using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackActions/CreateTurretAfterAttackSO")]
public class CreateTurretAfterAttackSO : AttackActionSO
{
    public UnitSO objectForSpawnData;
    bool IsSpawned=false;
    public override async Task Execute(GameObject attacker)
    {
        Transform lineTransform = attacker.transform.parent;
        UnitOnScene attackerUnitOnScene = attacker.GetComponent<UnitOnScene>();
        LineScript lineScript = lineTransform.GetComponent<LineScript>();
        if (lineScript.CheckPosibilityToAddACard())
        {
           
            lineScript.TryAddCardOnLine(objectForSpawnData);
        }
    }
}
