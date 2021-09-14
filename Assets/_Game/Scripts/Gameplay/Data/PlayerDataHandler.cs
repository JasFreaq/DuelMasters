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
    private List<CreatureObject> _blockersInBattle = new List<CreatureObject>();
    private Dictionary<int, CardObject> _allCards = new Dictionary<int, CardObject>();

    #region Properties

    public List<CardObject> CardsInDeck
    {
        get { return _cardsInDeck; }
    }
    
    public List<CardObject> CardsInGrave
    {
        get { return _cardsInGraveyard; }
    }

    public List<ShieldObject> Shields
    {
        get { return _shields; }
    }

    public Dictionary<int, CardObject> CardsInHand
    {
        get { return _cardsInHand; }
    }

    public List<CardObject> CardsInHandList
    {
        get
        {
            List<CardObject> handList = new List<CardObject>();
            foreach (CardObject cardObj in _cardsInHand.Values)
                handList.Add(cardObj);

            return handList;
        }
    }

    public Dictionary<int, CardObject> CardsInMana
    {
        get { return _cardsInManaZone; }
    }

    public List<CardObject> CardsInManaList
    {
        get
        {
            List<CardObject> manaList = new List<CardObject>();
            foreach (CardObject cardObj in _cardsInManaZone.Values)
                manaList.Add(cardObj);

            return manaList;
        }
    }

    public Dictionary<int, CreatureObject> CardsInBattle
    {
        get { return _cardsInBattleZone; }
    }

    public List<CreatureObject> CardsInBattleList
    {
        get
        {
            List<CreatureObject> battleList = new List<CreatureObject>();
            foreach (CreatureObject cardObj in _cardsInBattleZone.Values)
                battleList.Add(cardObj);

            return battleList;
        }
    }

    public List<CardObject> TappedCards
    {
        get { return _tappedCards; }
    }
    
    public List<CreatureObject> BlockersInBattle
    {
        get { return _blockersInBattle; }
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

            if (!cardObj.CardInst.IsTapped)
            {
                int civValue = CardParams.GetCivValue(cardObj.CardData.Civilization);
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

    #region Battle Functionality

    public List<CreatureObject> GetCreaturesInBattle(CardParams.Race[] checkRaces)
    {
        List<CreatureObject> validCreatures = new List<CreatureObject>();
        foreach (KeyValuePair<int, CreatureObject> pair in _cardsInBattleZone)
        {
            CreatureObject creatureObj = pair.Value;
            CardParams.Race[] races = creatureObj.CardData.Race;
            foreach (CardParams.Race race in races)
            {
                bool matched = false;
                foreach (CardParams.Race checkRace in checkRaces)
                {
                    if (race == checkRace)
                    {
                        validCreatures.Add(creatureObj);
                        matched = true;
                        break;
                    }
                }

                if (matched)
                    break;
            }
        }

        return validCreatures;
    }

    public bool CheckCreaturesInBattle(CardParams.Race[] checkRaces)
    {
        return GetCreaturesInBattle(checkRaces).Count > 0;
    }

    #endregion
}
