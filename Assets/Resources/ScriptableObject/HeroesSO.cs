using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class HeroesSO : ScriptableObject
{
    public int Hp;
    public int Mana;
    public Sprite cardSprite;
    public string Name;
    public Transform HeroPref;
    public Fraction Fraction;
    public RuntimeAnimatorController animatorController;
}
