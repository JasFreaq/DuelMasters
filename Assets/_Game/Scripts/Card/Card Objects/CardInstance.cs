using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance
{
    protected CardData _cardData;
    protected CardObject _cardObj;
    
    private CardInstanceEffectHandler _instanceEffectHandler;
    private CardZoneType _currentZone = CardZoneType.Deck;

    protected bool _isTapped;

    public CardInstance(CardData cardData)
    {
        _cardData = cardData;
        _instanceEffectHandler = new CardInstanceEffectHandler(this);
    }
    
    public CardData CardData
    {
        get { return _cardData; }
    }

    public CardObject CardObj
    {
        get { return _cardObj; }
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

    public void AssignCardObject(CardObject cardObj)
    {
        _cardObj = cardObj;
    }
}
