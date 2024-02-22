using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DuelMasters.Card.Data.Effects.Functionality.Parameters;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
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
                KeywordFuncParam keywordParam = (KeywordFuncParam) effectData.EffectFunctionality.FunctionalityParam;
                List<RaceHolder> raceHolders = keywordParam.VortexRaces;
                foreach (RaceHolder raceHolder in raceHolders)
                {
                    races.Add(raceHolder.race);
                }
                
                break;
            }
        }

        return races.ToArray();
    }

    public void OnBeforeSerialize()
    {
        List<KeywordType> keywordTypes = new List<KeywordType>();
        foreach (EffectData effectData in ruleEffects)
        {
            if (effectData.EffectFunctionality.Type == EffectFunctionalityType.Keyword)
            {
                KeywordFuncParam keywordParam = (KeywordFuncParam)effectData.EffectFunctionality.FunctionalityParam;
                KeywordType keyword = keywordParam.Keyword;
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
        foreach (EffectTargetingConditionParameter conditionParam in targetingCondition.TargetingConditionParams)
        {
            if (!conditionParam.IsConditionSatisfied(cardInst))
            {
                return false;
            }
        }

        foreach (EffectTargetingConditionParameter conditionParam in targetingCondition.CardIntrinsicTargetingConditionParams)
        {
            if (!conditionParam.IsConditionSatisfied(cardInst))
            {
                return false;
            }
        }

        return true;
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

    public static int GetNumValidCards(EffectTargetingCriterion targetingCriterion, EffectTargetingCondition targetingCondition)
    {
        List<CardBehaviour> cards = GameDataHandler.Instance.GetZoneCards(
            targetingCriterion.OwningPlayer == PlayerTargetType.Player, targetingCriterion.ZoneType,
            targetingCriterion.OwningPlayer == PlayerTargetType.Both);

        List<CardObject> validCards = GetValidCards(cards, targetingCondition);
        return validCards.Count;
    }

    #endregion
}
