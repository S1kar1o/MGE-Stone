using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectedCardLogic : MonoBehaviour
{
    [SerializeField] public Transform SelectedCard;
    [SerializeField] public Transform CardSequel;
    [SerializeField] public UnitsSOList unitsSOList;
    public static SelectedCardLogic Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public void SetelectedCard(Transform card)
    {
        SelectedCard = card;
    }
}
