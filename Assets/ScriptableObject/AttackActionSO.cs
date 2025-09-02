using System.Threading.Tasks;
using UnityEngine;

public abstract class AttackActionSO : ScriptableObject
{
    public abstract Task Execute(GameObject attacker);
}

