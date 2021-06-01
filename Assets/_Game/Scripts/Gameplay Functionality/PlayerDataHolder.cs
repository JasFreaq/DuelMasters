using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHolder : MonoBehaviour
{
    private Dictionary<int, CardManager> _cardsInHand = new Dictionary<int, CardManager>();
    private Dictionary<int, CardManager> _cardsInManaZone = new Dictionary<int, CardManager>();
    private Dictionary<int, CreatureCardManager> _cardsInBattleZone = new Dictionary<int, CreatureCardManager>();

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
