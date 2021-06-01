using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataHolder : MonoBehaviour
{
    private Dictionary<int, CardManager> _cardsInHand = new Dictionary<int, CardManager>();

    public Dictionary<int, CardManager> CardsInHand
    {
        get { return _cardsInHand; }
    }
}
