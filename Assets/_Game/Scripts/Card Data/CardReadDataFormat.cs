using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardReadDataFormat
{
    public string name, set, id, civilization, rarity, type, cost, text, flavor, illustrator, race, power;
    public string[] civilizations, races;
}

[System.Serializable]
public class CardReadDataList
{
    public List<CardReadDataFormat> cards;
}