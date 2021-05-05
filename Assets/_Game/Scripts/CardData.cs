using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardList
{
    public List<Card> cards;
}

[System.Serializable]
public class Card
{
    public string name, set, id, civilization, rarity, type, cost, text, flavor, illustrator, race, power;
    public string[] civilizations, races;
}