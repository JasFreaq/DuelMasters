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

    #region Static Methods

    public static List<CardObject> CheckValidity(List<CardObject> cardList, bool setHighlight = false)
    {
        List<CardObject> validCards = new List<CardObject>();
        foreach (CardObject cardObj in cardList)
        {
            if (Random.Range(0f, 10f) < 5f)
            {
                cardObj.PreviewLayoutHandler.SetValidity(true);
                if (setHighlight) 
                    cardObj.SetHighlight(true);

                validCards.Add(cardObj);
            }
            else
                cardObj.PreviewLayoutHandler.SetValidity(false);
        }
        
        foreach (CardObject cardObj in validCards)
            cardList.Remove(cardObj);

        cardList.AddRange(validCards);

        return cardList;
    }
    
    public static List<CardBehaviour> CheckValidity(List<CardBehaviour> cards)
    {
        List<CardObject> cardList = new List<CardObject>();
        foreach (CardBehaviour card in cards)
            cardList.Add((CardObject) card);

        return new List<CardBehaviour>(CheckValidity(cardList, true));
    }
    
    #endregion
}
