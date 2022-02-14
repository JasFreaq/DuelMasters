using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardData : ScriptableObject, ISerializationCallbackReceiver
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

    [HideInInspector] public List<EffectData> ruleEffects = new List<EffectData>();
    [HideInInspector] [SerializeField] private KeywordType[] _keywords;
    
    #region Properties

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

    public KeywordType[] Keywords
    {
        get { return _keywords; }
    }

    #endregion

    public CardParams.Race[] GetVortexEvolutionRaces()
    {
        List<CardParams.Race> races = new List<CardParams.Race>();
        foreach (EffectData effectData in ruleEffects)
        {
            if (effectData.EffectFunctionality.Type == EffectFunctionalityType.Keyword)
            {
                if (effectData.EffectFunctionality.Keyword == KeywordType.VortexEvolution)
                {
                    List<RaceHolder> raceHolders = effectData.EffectFunctionality.VortexRaces;
                    foreach (RaceHolder raceHolder in raceHolders)
                    {
                        races.Add(raceHolder.race);
                    }
                    
                    break;
                }
            }
        }

        return races.ToArray();
    }

    public void OnBeforeSerialize()
    {
        List<KeywordType> keywordTypes = new List<KeywordType>();
        foreach (EffectData effect in ruleEffects)
        {
            if (effect.EffectFunctionality.Type == EffectFunctionalityType.Keyword)
            {
                KeywordType keyword = effect.EffectFunctionality.Keyword;
                if (!keywordTypes.Contains(keyword))
                    keywordTypes.Add(keyword);
                else
                    Debug.LogError($"There are multiple keyword entries of {keyword} in {name}");
            }
        }

        _keywords = keywordTypes.ToArray();
    }

    public void OnAfterDeserialize() { }

    #region Static Methods

    public static bool IsTargetingConditionSatisfied(CardInstance cardInst, EffectTargetingCondition targetingCondition)
    {
        bool result = true;
        CardData cardData = cardInst.CardData;

        //if (targetingCondition.AssignedCardTypeCondition)
        //{
        //    switch (targetingCondition.CardTypeCondition)
        //    {
        //        case CardParams.CardType.EvolutionCreature:
        //            switch (cardData.CardType)
        //            {
        //                case CardParams.CardType.Creature:
        //                case CardParams.CardType.Spell:
        //                    result = false;
        //                    break;
        //            }
        //            break;

        //        case CardParams.CardType.Creature:
        //            switch (cardData.CardType)
        //            {
        //                case CardParams.CardType.Spell:
        //                    result = false;
        //                    break;
        //            }
        //            break;

        //        case CardParams.CardType.Spell:
        //            switch (cardData.CardType)
        //            {
        //                case CardParams.CardType.EvolutionCreature:
        //                case CardParams.CardType.Creature:
        //                    result = false;
        //                    break;
        //            }
        //            break;
        //    }
        //}

        //if (targetingCondition.CivilizationConditions.Count > 0)
        //{
        //    IReadOnlyList<CivilizationCondition> civilizationConditions = targetingCondition.CivilizationConditions;
        //    for (int i = 0, n = civilizationConditions.Count; i < n; i++)
        //    {
        //        CivilizationCondition civilizationCondition = civilizationConditions[i];

        //        bool res0 = cardData.Civilization.SequenceEqual(civilizationCondition.civilization);
        //        if (civilizationCondition.non)
        //            res0 = !res0;

        //        result = result && res0;

        //        if (n > 1 && i < n - 1)
        //        {
        //            if (civilizationCondition.connector == ConnectorType.And && !result)
        //                break;
        //            if (civilizationCondition.connector == ConnectorType.Or && result)
        //                break;
        //        }
        //    }
        //}

        //if (targetingCondition.RaceConditions.Count > 0)
        //{
        //    if (cardData is CreatureData creatureData)
        //    {
        //        IReadOnlyList<RaceCondition> raceConditions = targetingCondition.RaceConditions;
        //        for (int i = 0, n = raceConditions.Count; i < n; i++)
        //        {
        //            RaceCondition raceCondition = raceConditions[i];

        //            bool res0 = false;
        //            foreach (CardParams.Race race in creatureData.Race)
        //            {
        //                if (race == raceCondition.race)
        //                {
        //                    res0 = true;
        //                    break;
        //                }
                        
        //                bool breakLoop = false;
        //                switch (race)
        //                {
        //                    case CardParams.Race.ArmoredDragon:
        //                    case CardParams.Race.EarthDragon:
        //                    case CardParams.Race.VolcanoDragon:
        //                    case CardParams.Race.ZombieDragon:
        //                        switch (raceCondition.race)
        //                        {
        //                            case CardParams.Race.ArmoredDragon:
        //                            case CardParams.Race.EarthDragon:
        //                            case CardParams.Race.VolcanoDragon:
        //                            case CardParams.Race.ZombieDragon:
        //                                res0 = true;
        //                                breakLoop = true;
        //                                break;
        //                        }
        //                        break;
        //                }

        //                if (breakLoop)
        //                    break;
        //            }

        //            if (raceCondition.non)
        //                res0 = !res0;

        //            result = result && res0;

        //            if (n > 1 && i < n - 1)
        //            {
        //                if (raceCondition.connector == ConnectorType.And && !result)
        //                    break;
        //                if (raceCondition.connector == ConnectorType.Or && result)
        //                    break;
        //            }
        //        }
        //    }
        //    else
        //        result = false;
        //}

        //if (targetingCondition.KeywordConditions.Count > 0)
        //{
        //    IReadOnlyList<KeywordCondition> keywordConditions = targetingCondition.KeywordConditions;
        //    for (int i = 0, n = keywordConditions.Count; i < n; i++)
        //    {
        //        KeywordCondition keywordCondition = keywordConditions[i];

        //        bool res0 = false;
        //        foreach (KeywordType keyword in cardData.Keywords)
        //        {
        //            if (keyword == keywordCondition.keyword)
        //            {
        //                res0 = true;
        //                break;
        //            }
        //        }
        //        if (keywordCondition.non)
        //            res0 = !res0;

        //        result = result && res0;

        //        if (n > 1 && i < n - 1)
        //        {
        //            if (keywordCondition.connector == ConnectorType.And && !result)
        //                break;
        //            if (keywordCondition.connector == ConnectorType.Or && result)
        //                break;
        //        }
        //    }
        //}

        //if (targetingCondition.PowerConditions.Count > 0)
        //{
        //    if (cardData is CreatureData creatureData)
        //    {
        //        IReadOnlyList<PowerCondition> powerConditions = targetingCondition.PowerConditions;
        //        for (int i = 0, n = powerConditions.Count; i < n; i++)
        //        {
        //            PowerCondition powerCondition = powerConditions[i];

        //            bool res0 = false;
        //            switch (powerCondition.comparator)
        //            {
        //                case ComparisonType.LessThan:
        //                    res0 = creatureData.Power < powerCondition.power;
        //                    break;

        //                case ComparisonType.GreaterThan:
        //                    res0 = creatureData.Power > powerCondition.power;
        //                    break;

        //                case ComparisonType.EqualTo:
        //                    res0 = creatureData.Power == powerCondition.power;
        //                    break;

        //                case ComparisonType.LessThanOrEqualTo:
        //                    res0 = creatureData.Power <= powerCondition.power;
        //                    break;

        //                case ComparisonType.GreaterThanOrEqualTo:
        //                    res0 = creatureData.Power >= powerCondition.power;
        //                    break;
        //            }

        //            result = result && res0;

        //            if (n > 1 && i < n - 1)
        //            {
        //                if (powerCondition.connector == ConnectorType.And && !result)
        //                    break;
        //                if (powerCondition.connector == ConnectorType.Or && result)
        //                    break;
        //            }
        //        }
        //    }
        //    else
        //        result = false;
        //}

        //if (targetingCondition.CardConditions.Count > 0)
        //{
        //    IReadOnlyList<CardCondition> cardConditions = targetingCondition.CardConditions;
        //    for (int i = 0, n = cardConditions.Count; i < n; i++)
        //    {
        //        CardCondition cardCondition = cardConditions[i];

        //        bool res0 = cardData.Name == cardCondition.cardData.Name;
        //        result = result && res0;

        //        if (n > 1 && i < n - 1)
        //        {
        //            if (cardCondition.connector == ConnectorType.And && !result)
        //                break;
        //            if (cardCondition.connector == ConnectorType.Or && result)
        //                break;
        //        }
        //    }
        //}

        //if (targetingCondition.AssignedTapCondition)
        //{
        //    if (targetingCondition.TapCondition == TapStateType.Tap)
        //        result = result && cardInst.IsTapped;
        //    else if (targetingCondition.TapCondition == TapStateType.Untap)
        //        result = result && !cardInst.IsTapped;
        //}

        return result;
    }

    public static List<CardObject> GetValidCards(List<CardBehaviour> cards, EffectTargetingCondition targetingCondition)
    {
        List<CardObject> cardList = new List<CardObject>();
        foreach (CardBehaviour card in cards)
        {
            cardList.Add((CardObject)card);
        }

        return GetValidCards(cardList, targetingCondition);
    }

    public static List<CardObject> GetValidCards(List<CardObject> cardList, EffectTargetingCondition targetingCondition)
    {
        List<CardObject> validCards = new List<CardObject>();
        foreach (CardObject cardObj in cardList)
        {
            cardObj.SetValidity(targetingCondition);
            if (cardObj.IsValid)
                validCards.Add(cardObj);
        }

        return validCards;
    }

    public static int GetNumValidCards(EffectTargetingData targetingData, EffectTargetingCondition targetingCondition)
    {
        List<CardBehaviour> cards = GameDataHandler.Instance.GetZoneCards(
            targetingData.OwningPlayer == PlayerTargetType.Player, targetingData.ZoneType,
            targetingData.OwningPlayer == PlayerTargetType.Both);

        List<CardObject> validCards = GetValidCards(cards, targetingCondition);
        return validCards.Count;
    }

    #endregion
}
