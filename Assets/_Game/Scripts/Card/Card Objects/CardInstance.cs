using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance
{
    private CardData _cardData;
    private CardInstanceEffectHandler _instanceEffectHandler;
    private CardZoneType _currentZone = CardZoneType.Deck;

    private bool _isTapped;

    public CardInstance(CardData cardData)
    {
        _cardData = cardData;
        _instanceEffectHandler = new CardInstanceEffectHandler(cardData);
    }
    
    public CardData CardData
    {
        get { return _cardData; }
    }

    public CardInstanceEffectHandler InstanceEffectHandler
    {
        get { return _instanceEffectHandler; }
    }

    public CardZoneType CurrentZone
    {
        get { return _currentZone;}
    }

    public bool IsTapped
    {
        get { return _isTapped; }
    }
    
    public void SetCurrentZone(CardZoneType currentZone)
    {
        _currentZone = currentZone;
    }

    public void ToggleTapState()
    {
        _isTapped = !_isTapped;
    }

    public bool CanAttack()
    {
        return !_isTapped;
    }
}
