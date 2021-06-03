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
            if (!_tappedCards.Contains(card))
            {
                int civValue = CardParams.GetCivValue(card.Card.Civilization);
                if (!availableMana.ContainsKey(civValue))
                {
                    availableMana[civValue] = new List<CardManager>();
                }
                availableMana[civValue].Add(card);
            }
        }
        print($"Count: {availableMana.Count}");
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
    
    public bool CanPayCost(Card cardData, int costReduction)
    {
        Dictionary<int, List<CardManager>> availableMana = GetAvailableManaCards();

        if (GetAvailableMana(availableMana) >= cardData.Cost - costReduction) 
        {
            List<int> iDs = GetCivIDs(cardData.Civilization);

            int hits = 0;
            int n = cardData.Civilization.Length;
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

    public void PayCost(Card cardData, int costReduction)
    {
        Dictionary<int, List<CardManager>> availableMana = GetAvailableManaCards();
        int cost = Mathf.Max(cardData.Cost - costReduction, 1);

        List<List<CardManager>> correspondingCards = new List<List<CardManager>>();
        int smallestAvailableCivLen = int.MaxValue;

        if (GetAvailableMana(availableMana) >= cost)
        {
            List<int> iDs = GetCivIDs(cardData.Civilization);
            iDs.Sort();

            int hits = 0;
            int n = cardData.Civilization.Length;
            foreach (int iD in iDs)
            {
                if (availableMana.ContainsKey(iD))
                {
                    hits++;
                    correspondingCards.Add(availableMana[iD]);
                    if (availableMana[iD].Count < smallestAvailableCivLen)
                        smallestAvailableCivLen = availableMana[iD].Count;
                }

                if (hits == n)
                    break;
            }

            for (int i = 0; i < smallestAvailableCivLen; i++) 
            {
                foreach (List<CardManager> correspondingCardList in correspondingCards)
                {
                    cost--;
                    correspondingCardList[i].SetTap(true);

                    if (cost == 0)
                        return;
                }
            }

            foreach (List<CardManager> correspondingCardList in correspondingCards)
            {
                foreach (CardManager card in correspondingCardList)
                {
                    if (!card.IsTapped) 
                    {
                        cost--;
                        card.SetTap(true);

                        if (cost == 0)
                            return;
                    }
                }
            }
        }
    }
}
