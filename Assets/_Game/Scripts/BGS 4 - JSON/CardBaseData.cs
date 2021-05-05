using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardBases
{
    public List<CardBaseData> cards;
}

[System.Serializable]
public class CardBaseData
{
    public string name;
    public string id;
}

