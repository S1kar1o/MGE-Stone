using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class UnitSO : ScriptableObject
{
    public int Hp,Damage;
    public int cost;
    public Sprite cardSprite;
    public string cardName;
    public Transform UnitPref;
    public string HeroDescription;
    public Fraction Fraction;
    public Type Type;
    public AttackActionSO attackAction;
    public AbilitySO actionOnStart;

}
public enum Fraction{
    MGE,
    Furry,
}
public enum Type{
    Tank,
    None,
    DamageDealer,
    Support,
}