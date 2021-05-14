using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaZoneManager : MonoBehaviour
{
    private ManaZoneLayoutHandler _manaZoneLayoutHandler;

    private void Awake()
    {
        _manaZoneLayoutHandler = GetComponent<ManaZoneLayoutHandler>();
    }

    public Transform AssignTempCard(CardData cardData)
    {
        return _manaZoneLayoutHandler.AssignTempCard(cardData);
    }

    public void AddCard(Transform cardTransform)
    {
        _manaZoneLayoutHandler.AddCard(cardTransform);
    }

    public CardManager GetCardAtIndex(int index)
    {
        return _manaZoneLayoutHandler.GetCardAtIndex(index);
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        return _manaZoneLayoutHandler.RemoveCardAtIndex(index);
    }
}
