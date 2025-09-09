using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackActions/CreateTurretAfterAttackSO")]
public class CreateTurretAfterAttackSO : AttackActionSO
{
    public Transform cardPref;
    public UnitSO objectForSpawnData;
    bool IsSpawned;
    public override async Task Execute(GameObject attacker)
    {
        Transform lineTransform = attacker.transform.parent;
        UnitOnScene attackerUnitOnScene = attacker.GetComponent<UnitOnScene>();
        LineScript lineScript = lineTransform.GetComponent<LineScript>();
        if (lineScript.CheckPosibilityToAddACard())
        {
            Transform card = Instantiate(cardPref);
            CardInHand cardInHand = card.GetComponent<CardInHand>();
            cardInHand.cardData = objectForSpawnData;
            lineScript.TryAddCardOnLine(card);
        }
    }
}
