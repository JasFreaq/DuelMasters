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

    #region Effect Properties

    public bool CanAttackCreatures
    {
        get { return true; }
    }
    
    public bool CanAttackPlayers
    {
        get { return true; }
    }

    //public bool CantBeBlocked

    #endregion

    public void SetCurrentZone(CardZoneType currentZone)
    {
        _currentZone = currentZone;
    }

    public void ToggleTapState()
    {
        _isTapped = !_isTapped;
    }


}
