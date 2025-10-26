using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/CreateTurretAfterSO")]
public class CreateTurretAfterAttackSO : AbilitySO
{
    public UnitSO objectForSpawnFirstLevelData;
    public UnitSO objectForSpawnSecondLevelData;
    public UnitSO objectForSpawnThirdLevelData;
    public bool waitingForNextTurn = false;
    public override async Task Execute(GameObject attacker)
    {
        Transform lineTransform = attacker.transform.parent;
        LineScript lineScript = lineTransform.GetComponent<LineScript>();
        if (!waitingForNextTurn)
        {
            if (lineScript.CheckPosibilityToAddACard())
            {
                await lineScript.TryAddCardOnLine(objectForSpawnFirstLevelData, false);
                waitingForNextTurn = !waitingForNextTurn;
                return;
            }
            foreach (Transform child in lineTransform)
            {
                UnitOnScene unitOnLine = child.GetComponent<UnitOnScene>();
                if (unitOnLine.cardData == objectForSpawnFirstLevelData)
                {
                    await unitOnLine.ChangeUnitTo(objectForSpawnSecondLevelData);
                    waitingForNextTurn = !waitingForNextTurn;
                    break;
                }
                else if(unitOnLine.cardData == objectForSpawnSecondLevelData)
                {
                    await unitOnLine.ChangeUnitTo(objectForSpawnThirdLevelData);
                    waitingForNextTurn = !waitingForNextTurn;
                    break;
                }

            }
        }
        waitingForNextTurn = !waitingForNextTurn;
    }
}
