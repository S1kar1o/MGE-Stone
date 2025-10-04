using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IAttackAction
{
    void Execute(GameObject attacker, GameObject target);
}

