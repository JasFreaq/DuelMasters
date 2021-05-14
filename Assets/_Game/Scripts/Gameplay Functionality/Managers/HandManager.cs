using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HandManager : MonoBehaviour
{
    private HandLayoutHandler _handLayoutHandler;

    private void Awake()
    {
        _handLayoutHandler = GetComponent<HandLayoutHandler>();
    }

    public Transform AssignTempCard()
    {
        return _handLayoutHandler.AssignTempCard();
    }

    public void AddCard(Transform cardTransform)
    {
        _handLayoutHandler.AddCard(cardTransform);
    }

    public CardManager GetCardAtIndex(int index)
    {
        return _handLayoutHandler.GetCardAtIndex(index);
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        return _handLayoutHandler.RemoveCardAtIndex(index);
    }
}