using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardData
{
    public string name, set, id, civilization, rarity, type, cost, text, flavor, illustrator, race, power;
    public string[] civilizations, races;
}

[System.Serializable]
public class CardDataList
{
    public List<CardData> cards;
}