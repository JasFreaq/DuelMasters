using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private CardParams.Set _set;
    [SerializeField] private CardParams.Civilization[] _civilization;
    [SerializeField] private CardParams.Rarity _rarity;
    [SerializeField] private CardParams.CardType _cardType;
    [SerializeField] private int _cost;
    [SerializeField] private Sprite _artworkImage;
    [SerializeField] [TextArea(5, 8)] private string _rulesText;
    [SerializeField] [TextArea(3, 5)] private string _flavorText;

    public string Name
    {
        get { return _name; }
#if UNITY_EDITOR
        set { _name = value; }
#endif
    }

    public CardParams.Set Set
    {
        get { return _set; }
#if UNITY_EDITOR
        set { _set = value; }
#endif
    }

    public CardParams.Civilization[] Civilization
    {
        get { return _civilization; }
#if UNITY_EDITOR
        set { _civilization = value; }
#endif
    }
    
    public CardParams.Rarity Rarity
    {
        get { return _rarity; }
#if UNITY_EDITOR
        set { _rarity = value; }
#endif
    }
    
    public CardParams.CardType CardType
    {
        get { return _cardType; }
#if UNITY_EDITOR
        set { _cardType = value; }
#endif
    }
    
    public int Cost
    {
        get { return _cost; }
#if UNITY_EDITOR
        set { _cost = value; }
#endif
    }
    
    public Sprite ArtworkImage
    {
        get { return _artworkImage; }
#if UNITY_EDITOR
        set { _artworkImage = value; }
#endif
    }
    
    public string RulesText
    {
        get { return _rulesText; }
#if UNITY_EDITOR
        set { _rulesText = value; }
#endif
    }
    
    public string FlavorText
    {
        get { return _flavorText; }
#if UNITY_EDITOR
        set { _flavorText = value; }
#endif
    }
}
