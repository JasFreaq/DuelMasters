using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardLayoutHandler))]
public class PreviewHandler : MonoBehaviour
{
    [SerializeField] private GameObject _invalidOverlay;

    private CardLayoutHandler _cardLayoutHandler;

    private bool _isValid;
    
    public Canvas Canvas
    {
        get { return _cardLayoutHandler.Canvas; }
    }

    private void Awake()
    {
        _cardLayoutHandler = GetComponent<CardLayoutHandler>();
    }

    public void SetupCard(CardData cardData, CardFrameData cardFrameData)
    {
        _cardLayoutHandler.SetupCard(cardData, cardFrameData);
    }

    public void SetValidity(bool isValid)
    {
        _isValid = isValid;
        _cardLayoutHandler.SetGlow(isValid);
        _invalidOverlay.SetActive(!isValid);
    }
}
