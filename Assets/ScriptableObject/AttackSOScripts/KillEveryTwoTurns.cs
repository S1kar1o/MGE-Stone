using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class KillEveryTwoTurns : AttackActionSO
{
    public override bool needToIdentify => true;

    private bool readyToAttack=true;
   public override async Task Execute(GameObject attacker)
    {
        Transform lineTransform = attacker.transform.parent;
        if (readyToAttack)
        {
            List<Transform> targets = new List<Transform>();
            foreach (Transform unit in lineTransform)
            {
                if (unit.GetComponent<UnitOnScene>().fraction != attacker.GetComponent<UnitOnScene>().fraction)
                {
                    targets.Add(unit);
                }
            }
            await targets[Random.Range(0, targets.Count)].GetComponent<UnitOnScene>().DestroyCard();
            readyToAttack = !readyToAttack;
            return;
        }
        readyToAttack = !readyToAttack;
    }
}
