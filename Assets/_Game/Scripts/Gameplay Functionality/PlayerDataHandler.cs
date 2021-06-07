using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHandler : MonoBehaviour
{
    private List<CardManager> _cardsInDeck = new List<CardManager>();
    private List<CardManager> _cardsInShields = new List<CardManager>();
    private List<CardManager> _cardsInGraveyard = new List<CardManager>();

    private Dictionary<int, CardManager> _cardsInHand = new Dictionary<int, CardManager>();
    private Dictionary<int, CardManager> _cardsInManaZone = new Dictionary<int, CardManager>();
    private Dictionary<int, CreatureCardManager> _cardsInBattleZone = new Dictionary<int, CreatureCardManager>();
    
    private List<CardManager> _tappedCards = new List<CardManager>();

    #region Properties

    public List<CardManager> CardsInDeck
    {
        get { return _cardsInDeck; }
    }
    
    public List<CardManager> CardsInShields
    {
        get { return _cardsInShields; }
    }
    
    public List<CardManager> CardsInGrave
    {
        get { return _cardsInGraveyard; }
    }

    public Dictionary<int, CardManager> CardsInHand
    {
        get { return _cardsInHand; }
    }
    
    public Dictionary<int, CardManager> CardsInMana
    {
        get { return _cardsInManaZone; }
    }

    public Dictionary<int, CreatureCardManager> CardsInBattle
    {
        get { return _cardsInBattleZone; }
    }

    public List<CardManager> TappedCards
    {
        get { return _tappedCards; }
    }

    #endregion

    private Dictionary<int, List<CardManager>> GetAvailableManaCards()
    {
        Dictionary<int, List<CardManager>> availableMana = new Dictionary<int, List<CardManager>>();
        foreach (KeyValuePair<int, CardManager> pair in _cardsInManaZone)
        {
            CardManager card = pair.Value;
            if (!card.IsTapped)
            {
                int civValue = CardParams.GetCivValue(card.CardData.Civilization);
                if (!availableMana.ContainsKey(civValue))
                {
                    availableMana[civValue] = new List<CardManager>();
                }
                availableMana[civValue].Add(card);
            }
        }

        return availableMana;
    }

    private int GetAvailableMana(Dictionary<int, List<CardManager>> availableMana)
    {
        int mana = 0;
        foreach (KeyValuePair<int, List<CardManager>> pair in availableMana)
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
        Dictionary<int, List<CardManager>> availableMana = GetAvailableManaCards();

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
        Dictionary<int, List<CardManager>> availableMana = GetAvailableManaCards();

        List<List<CardManager>> correspondingCardLists = new List<List<CardManager>>();
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
                foreach (List<CardManager> correspondingCardList in correspondingCardLists)
                {
                    cost--;
                    correspondingCardList[i].SetTap(true);
                    _tappedCards.Add(correspondingCardList[i]);

                    if (cost == 0)
                        return;
                }
            }

            foreach (List<CardManager> correspondingCardList in correspondingCardLists)
            {
                foreach (CardManager card in correspondingCardList)
                {
                    if (!card.IsTapped) 
                    {
                        cost--;
                        card.SetTap(true);
                        _tappedCards.Add(card);

                        if (cost == 0)
                            return;
                    }
                }
            }
        }
    }
}
