using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Helper Data Structures

[System.Serializable]
public enum ConnectorType
{
    And,
    Or
}

[System.Serializable]
public class CardTypeCondition
{
    public bool isAssigned = false;
    public CardParams.CardType cardType;
}

[System.Serializable]
public class CivilizationCondition
{
    public bool non = false;
    public CardParams.Civilization[] civilization;
    public ConnectorType connector;
}

[System.Serializable]
public class RaceCondition
{
    public bool non = false;
    public CardParams.Race race;
    public ConnectorType connector;
}

[System.Serializable]
public class KeywordCondition
{
    public bool non = false;
    public KeywordType keyword;
    public ConnectorType connector;
}

[System.Serializable]
public enum ComparisonType
{
    LessThan,
    GreaterThan,
    EqualTo,
    LessThanOrEqualTo,
    GreaterThanOrEqualTo
}

[System.Serializable]
public class PowerCondition
{
    public ComparisonType comparator;
    public int power;
    public ConnectorType connector;
}

[System.Serializable]
public class CardCondition
{
    public CardData cardData;
    public ConnectorType connector;
}

#endregion

[System.Serializable]
public class EffectTargetingCondition
{
    private CardTypeCondition _cardTypeCondition = new CardTypeCondition();
    private List<CivilizationCondition> _civilizationConditions = new List<CivilizationCondition>();
    private List<RaceCondition> _raceConditions = new List<RaceCondition>();
    private List<KeywordCondition> _keywordConditions = new List<KeywordCondition>();
    private List<PowerCondition> _powerConditions = new List<PowerCondition>();
    private List<CardCondition> _cardConditions = new List<CardCondition>();

    public CardTypeCondition CardTypeCondition
    {
        get { return _cardTypeCondition; }

#if UNITY_EDITOR
        set { _cardTypeCondition = value; }
#endif
    }
    
    public IReadOnlyList<CivilizationCondition> CivilizationConditions
    {
        get { return _civilizationConditions; }
    }
    
    public IReadOnlyList<RaceCondition> RaceConditions
    {
        get { return _raceConditions; }
    }

    public IReadOnlyList<KeywordCondition> KeywordConditions
    {
        get { return _keywordConditions; }
    }
    
    public IReadOnlyList<PowerCondition> PowerConditions
    {
        get { return _powerConditions; }
    }

    public IReadOnlyList<CardCondition> CardConditions
    {
        get { return _cardConditions; }
    }

#if UNITY_EDITOR
    public void AddCivilizationCondition(CivilizationCondition condition)
    {
        _civilizationConditions.Add(condition);
    }
    
    public void RemoveCivilizationCondition(CivilizationCondition condition)
    {
        _civilizationConditions.Remove(condition);
    }
    
    public void AddRaceCondition(RaceCondition condition)
    {
        _raceConditions.Add(condition);
    }
    
    public void RemoveRaceCondition(RaceCondition condition)
    {
        _raceConditions.Remove(condition);
    }

    public void AddKeywordCondition(KeywordCondition condition)
    {
        _keywordConditions.Add(condition);
    }
    
    public void RemoveKeywordCondition(KeywordCondition condition)
    {
        _keywordConditions.Remove(condition);
    }
    
    public void AddPowerCondition(PowerCondition condition)
    {
        _powerConditions.Add(condition);
    }
    
    public void RemovePowerCondition(PowerCondition condition)
    {
        _powerConditions.Remove(condition);
    }
    
    public void AddCardCondition(CardCondition condition)
    {
        _cardConditions.Add(condition);
    }
    
    public void RemoveCardCondition(CardCondition condition)
    {
        _cardConditions.Remove(condition);
    }
    
#endif
    
    public override string ToString()
    {
        string str = "";

        if (_cardTypeCondition.isAssigned)
        {
            str += $"Card Type is {CardParams.StringFromCardType(_cardTypeCondition.cardType)}\n";
        }
        if (_civilizationConditions.Count > 0)
        {
            str += "\nCivilization is ";
            for (int i = 0, n = _civilizationConditions.Count; i < n; i++)
            {
                CivilizationCondition civilizationCondition = _civilizationConditions[i];
                if (civilizationCondition.non)
                    str += "non-";
                str += $"{CardParams.StringFromCivilization(civilizationCondition.civilization)} ";
                if (n > 1 && i < n - 1)
                    str += $"{_civilizationConditions[i].connector} ";
            }
        }
        if (_raceConditions.Count > 0)
        {
            str += "\nRace is ";
            for (int i = 0, n = _raceConditions.Count; i < n; i++) 
            {
                RaceCondition raceCondition = _raceConditions[i];
                if (raceCondition.non)
                    str += "non-";
                str += $"{CardParams.StringFromRace(raceCondition.race)} ";
                if (n > 1 && i < n - 1)
                    str += $"{_raceConditions[i].connector} ";
            }
        }

        return str;
    }
}
