using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHandler : MonoBehaviour
{
    private List<CardInstanceObject> _cardsInDeck = new List<CardInstanceObject>();
    private List<CardInstanceObject> _cardsInGraveyard = new List<CardInstanceObject>();
    private List<ShieldObject> _shields = new List<ShieldObject>();

    private Dictionary<int, CardInstanceObject> _cardsInHand = new Dictionary<int, CardInstanceObject>();
    private Dictionary<int, CardInstanceObject> _cardsInManaZone = new Dictionary<int, CardInstanceObject>();
    private Dictionary<int, CreatureInstanceObject> _cardsInBattleZone = new Dictionary<int, CreatureInstanceObject>();
    
    private List<CardInstanceObject> _tappedCards = new List<CardInstanceObject>();
    private Dictionary<int, CardBehaviour> _allCards = new Dictionary<int, CardBehaviour>();

    #region Properties

    public List<CardInstanceObject> CardsInDeck
    {
        get { return _cardsInDeck; }
    }

    public List<ShieldObject> Shields
    {
        get { return _shields; }
    }

    public List<CardInstanceObject> CardsInGrave
    {
        get { return _cardsInGraveyard; }
    }

    public Dictionary<int, CardInstanceObject> CardsInHand
    {
        get { return _cardsInHand; }
    }
    
    public Dictionary<int, CardInstanceObject> CardsInMana
    {
        get { return _cardsInManaZone; }
    }

    public Dictionary<int, CreatureInstanceObject> CardsInBattle
    {
        get { return _cardsInBattleZone; }
    }

    public List<CardInstanceObject> TappedCards
    {
        get { return _tappedCards; }
    }
    
    public IReadOnlyDictionary<int, CardBehaviour> AllCards
    {
        get { return _allCards; }
    }

    #endregion

    public void SetAllCards()
    {
        foreach (CardInstanceObject card in _cardsInDeck)
        {
            _allCards.Add(card.transform.GetInstanceID(), card);
        }
    }

    #region Mana Functionality

    private Dictionary<int, List<CardInstanceObject>> GetAvailableManaCards()
    {
        Dictionary<int, List<CardInstanceObject>> availableMana = new Dictionary<int, List<CardInstanceObject>>();
        foreach (KeyValuePair<int, CardInstanceObject> pair in _cardsInManaZone)
        {
            CardInstanceObject card = pair.Value;
            if (!card.IsTapped)
            {
                int civValue = CardParams.GetCivValue(card.CardData.Civilization);
                if (!availableMana.ContainsKey(civValue))
                {
                    availableMana[civValue] = new List<CardInstanceObject>();
                }
                availableMana[civValue].Add(card);
            }
        }

        return availableMana;
    }
    
    private int GetAvailableMana(Dictionary<int, List<CardInstanceObject>> availableMana)
    {
        int mana = 0;
        foreach (KeyValuePair<int, List<CardInstanceObject>> pair in availableMana)
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
        Dictionary<int, List<CardInstanceObject>> availableMana = GetAvailableManaCards();

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
        Dictionary<int, List<CardInstanceObject>> availableMana = GetAvailableManaCards();

        List<List<CardInstanceObject>> correspondingCardLists = new List<List<CardInstanceObject>>();
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
                foreach (List<CardInstanceObject> correspondingCardList in correspondingCardLists)
                {
                    cost--;
                    correspondingCardList[i].ToggleTap();

                    if (cost == 0)
                        return;
                }
            }

            foreach (List<CardInstanceObject> correspondingCardList in correspondingCardLists)
            {
                foreach (CardInstanceObject card in correspondingCardList)
                {
                    if (!card.IsTapped) 
                    {
                        cost--;
                        card.ToggleTap();

                        if (cost == 0)
                            return;
                    }
                }
            }
        }
    }

    #endregion
}
