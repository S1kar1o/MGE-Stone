using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu (menuName = "Ability/KillEveryTwoTurns")]
public class KillEveryTwoTurns : AbilitySO
{
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
            if (targets.Count > 0)
            {
                await targets[Random.Range(0, targets.Count)].GetComponent<UnitOnScene>().DestroyCard();
                readyToAttack = !readyToAttack;
            }
            return;
        }
        readyToAttack = !readyToAttack;
    }
}
