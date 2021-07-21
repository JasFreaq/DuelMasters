using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance
{
    private CardData _cardData;
    private CardZoneType _currentZone = CardZoneType.Deck;

    private bool _isTapped = false;

    #region Effect

    

    #endregion

    public CardInstance(CardData cardData)
    {
        _cardData = cardData;
    }

    public CardData CardData
    {
        get { return _cardData; }
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
