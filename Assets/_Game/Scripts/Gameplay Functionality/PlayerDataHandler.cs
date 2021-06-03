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
}
