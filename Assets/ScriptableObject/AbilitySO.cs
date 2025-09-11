using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public abstract class AbilitySO : AttackActionSO
{
    public bool needToIdentify = false;
    public bool readyToAttack = true;
    public bool executeOnStart = false;
    public bool attackSkill=false;
    public override abstract Task Execute(GameObject attacker);
    
}
