using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHolder : MonoBehaviour
{
    private Dictionary<int, CardManager> _cardsInHand = new Dictionary<int, CardManager>();
    private Dictionary<int, CardManager> _cardsInManaZone = new Dictionary<int, CardManager>();

    public Dictionary<int, CardManager> CardsInHand
    {
        get { return _cardsInHand; }
    }
    
    public Dictionary<int, CardManager> CardsInMana
    {
        get { return _cardsInManaZone; }
    }
}
