using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardManager : MonoBehaviour
{
    private GraveyardLayoutHandler _graveyardLayoutHandler;

    private void Awake()
    {
        _graveyardLayoutHandler = GetComponent<GraveyardLayoutHandler>();
    }

    public void AddCard(Transform cardTransform)
    {
        _graveyardLayoutHandler.AddCard(cardTransform);
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        return _graveyardLayoutHandler.RemoveCardAtIndex(index);
    }
}
