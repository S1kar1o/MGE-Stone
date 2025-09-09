using System.Threading.Tasks;
using UnityEngine;

public abstract class AttackActionSO : ScriptableObject
{
    public virtual bool needToIdentify => false;
    public abstract Task Execute(GameObject attacker);
}

