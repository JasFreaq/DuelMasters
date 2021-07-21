using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHandler : MonoBehaviour
{
    private List<CardObject> _cardsInDeck = new List<CardObject>();
    private List<CardObject> _cardsInGraveyard = new List<CardObject>();
    private List<ShieldObject> _shields = new List<ShieldObject>();

    private Dictionary<int, CardObject> _cardsInHand = new Dictionary<int, CardObject>();
    private Dictionary<int, CardObject> _cardsInManaZone = new Dictionary<int, CardObject>();
    private Dictionary<int, CreatureObject> _cardsInBattleZone = new Dictionary<int, CreatureObject>();
    
    private List<CardObject> _tappedCards = new List<CardObject>();
    private Dictionary<int, CardObject> _allCards = new Dictionary<int, CardObject>();

    #region Properties

    public List<CardObject> CardsInDeck
    {
        get { return _cardsInDeck; }
    }

    public List<ShieldObject> Shields
    {
        get { return _shields; }
    }

    public List<CardObject> CardsInGrave
    {
        get { return _cardsInGraveyard; }
    }

    public Dictionary<int, CardObject> CardsInHand
    {
        get { return _cardsInHand; }
    }
    
    public Dictionary<int, CardObject> CardsInMana
    {
        get { return _cardsInManaZone; }
    }

    public Dictionary<int, CreatureObject> CardsInBattle
    {
        get { return _cardsInBattleZone; }
    }

    public List<CardObject> TappedCards
    {
        get { return _tappedCards; }
    }
    
    public IReadOnlyDictionary<int, CardObject> AllCards
    {
        get { return _allCards; }
    }

    #endregion

    public void SetAllCards()
    {
        foreach (CardObject card in _cardsInDeck)
        {
            _allCards.Add(card.transform.GetInstanceID(), card);
        }
    }

    #region Mana Functionality

    private Dictionary<int, List<CardObject>> GetAvailableManaCards()
    {
        Dictionary<int, List<CardObject>> availableMana = new Dictionary<int, List<CardObject>>();
        foreach (KeyValuePair<int, CardObject> pair in _cardsInManaZone)
        {
            CardObject cardObj = pair.Value;
            CardInstance cardInst = cardObj.CardInst;

            if (!cardInst.IsTapped)
            {
                int civValue = CardParams.GetCivValue(cardInst.CardData.Civilization);
                if (!availableMana.ContainsKey(civValue))
                {
                    availableMana[civValue] = new List<CardObject>();
                }
                availableMana[civValue].Add(cardObj);
            }
        }

        return availableMana;
    }
    
    private int GetAvailableMana(Dictionary<int, List<CardObject>> availableMana)
    {
        int mana = 0;
        foreach (KeyValuePair<int, List<CardObject>> pair in availableMana)
        {
            mana += pair.Value.Count;
        }

        return mana;
    }

    private List<int> GetCivIDs(CardParams.Civilization[] civilizations)
    {
        List<int> iDs = new List<int>();
        int snum = 0;
        int n = civilizations.Length;
        while (snum < Mathf.Pow(2, n))
        {
            List<CardParams.Civilization> civilization = new List<CardParams.Civilization>();
            for (int i = 0; i < n; i++)
            {
                if ((snum & (1 << i)) != 0)
                {
                    civilization.Add(civilizations[i]);
                }
            }
            snum++;
            iDs.Add(CardParams.GetCivValue(civilization.ToArray()));
        }
        
        return iDs;
    }
    
    public bool CanPayCost(CardParams.Civilization[] civilization, int cost)
    {
        Dictionary<int, List<CardObject>> availableMana = GetAvailableManaCards();

        if (GetAvailableMana(availableMana) >= cost) 
        {
            List<int> iDs = GetCivIDs(civilization);

            int hits = 0;
            int n = civilization.Length;
            foreach (int i in iDs)
            {
                if (availableMana.ContainsKey(i))
                    hits++;

                if (hits == n)
                    return true;
            }
        }
        
        return false;
    }

    public void PayCost(CardParams.Civilization[] civilization, int cost)
    {
        Dictionary<int, List<CardObject>> availableMana = GetAvailableManaCards();

        List<List<CardObject>> correspondingCardLists = new List<List<CardObject>>();
        int smallestAvailableCivLen = int.MaxValue;

        if (GetAvailableMana(availableMana) >= cost)
        {
            List<int> iDs = GetCivIDs(civilization);
            iDs.Sort();

            int hits = 0;
            int n = civilization.Length;
            foreach (int iD in iDs)
            {
                if (availableMana.ContainsKey(iD))
                {
                    hits++;
                    correspondingCardLists.Add(availableMana[iD]);
                    if (availableMana[iD].Count < smallestAvailableCivLen)
                        smallestAvailableCivLen = availableMana[iD].Count;
                }

                if (hits == n)
                    break;
            }

            for (int i = 0; i < smallestAvailableCivLen; i++) 
            {
                foreach (List<CardObject> correspondingCardList in correspondingCardLists)
                {
                    cost--;
                    correspondingCardList[i].ToggleTapState();

                    if (cost == 0)
                        return;
                }
            }

            foreach (List<CardObject> correspondingCardList in correspondingCardLists)
            {
                foreach (CardObject card in correspondingCardList)
                {
                    if (!card.CardInst.IsTapped) 
                    {
                        cost--;
                        card.ToggleTapState();

                        if (cost == 0)
                            return;
                    }
                }
            }
        }
    }

    #endregion
}
