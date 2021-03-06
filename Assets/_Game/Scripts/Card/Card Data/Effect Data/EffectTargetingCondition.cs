using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Helper Data Structures

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
    [SerializeReference] private bool _assignedCardTypeCondition, _assignedTapCondition;
    [SerializeReference] private CardParams.CardType _cardTypeCondition;
    [SerializeReference] private List<CivilizationCondition> _civilizationConditions = new List<CivilizationCondition>();
    [SerializeReference] private List<RaceCondition> _raceConditions = new List<RaceCondition>();
    [SerializeReference] private List<KeywordCondition> _keywordConditions = new List<KeywordCondition>();
    [SerializeReference] private List<PowerCondition> _powerConditions = new List<PowerCondition>();
    [SerializeReference] private List<CardCondition> _cardConditions = new List<CardCondition>();
    [SerializeReference] private TapStateType _tapCondition;

    public CardParams.CardType CardTypeCondition
    {
        get { return _cardTypeCondition; }

#if UNITY_EDITOR
        set { _cardTypeCondition = value; }
#endif
    }

    public bool AssignedCardTypeCondition
    {
        get { return _assignedCardTypeCondition; }

#if UNITY_EDITOR
        set { _assignedCardTypeCondition = value; }
#endif
    }
    
    public bool AssignedTapCondition
    {
        get { return _assignedTapCondition; }

#if UNITY_EDITOR
        set { _assignedTapCondition = value; }
#endif
    }

    public TapStateType TapCondition
    {
        get { return _tapCondition; }

#if UNITY_EDITOR
        set { _tapCondition = value; }
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

    #region String Formatting

    public string GetCardTypeString()
    {
        return $" {CardParams.StringFromCardType(_cardTypeCondition)}";
    }

    public string GetCivilizationString()
    {
        string str = "";
        for (int i = 0, n = _civilizationConditions.Count; i < n; i++)
        {
            CivilizationCondition civilizationCondition = _civilizationConditions[i];
            if (civilizationCondition.non)
                str += $" non-{CardParams.StringFromCivilization(civilizationCondition.civilization)}";
            else
                str += $" {CardParams.StringFromCivilization(civilizationCondition.civilization)}";
            
            if (n > 1 && i < n - 1)
                str += $" {_civilizationConditions[i].connector} ";
        }

        return str;
    }
    
    public string GetRaceString()
    {
        string str = "";
        for (int i = 0, n = _raceConditions.Count; i < n; i++)
        {
            RaceCondition raceCondition = _raceConditions[i];
            if (raceCondition.non)
                str += $" non-{CardParams.StringFromRace(raceCondition.race)}";
            else 
                str += $" {CardParams.StringFromRace(raceCondition.race)}";

            if (n > 1 && i < n - 1)
                str += $" {_raceConditions[i].connector} ";
        }

        return str;
    }
    
    public string GetKeywordString()
    {
        string str = "";
        for (int i = 0, n = _keywordConditions.Count; i < n; i++)
        {
            KeywordCondition keywordCondition = _keywordConditions[i];
            if (keywordCondition.non)
                str += $" non-{keywordCondition.keyword}";
            else
                str += $" {keywordCondition.keyword}";

            if (n > 1 && i < n - 1)
                str += $" {_keywordConditions[i].connector} ";
        }

        return str;
    }
    
    public string GetPowerString()
    {
        string str = "";
        for (int i = 0, n = _powerConditions.Count; i < n; i++)
        {
            PowerCondition powerCondition = _powerConditions[i];
            str += $" {powerCondition.comparator}";
            str += $" {powerCondition.power}";

            if (n > 1 && i < n - 1)
                str += $" {_powerConditions[i].connector} ";
        }

        return str;
    }
    
    public string GetCardString()
    {
        string str = "";
        for (int i = 0, n = _cardConditions.Count; i < n; i++)
        {
            CardCondition cardCondition = _cardConditions[i];
            str += $" {cardCondition.cardData.Name}";

            if (n > 1 && i < n - 1)
                str += $" {_cardConditions[i].connector} ";
        }

        return str;
    }

    public string GetConditionParametersString()
    {
        string str = "";

        if (_assignedCardTypeCondition)
            str += GetCardTypeString();
        else if (_civilizationConditions.Count > 0)
            str += GetCivilizationString() + " card";
        else if (_raceConditions.Count > 0)
            str += GetRaceString();
        else if (_keywordConditions.Count > 0)
            str += GetKeywordString();
        else if (_powerConditions.Count > 0)
            str += $"creature with power {GetPowerString()}";
        else if (_cardConditions.Count > 0)
            str += GetCardString();

        return str;
    }

    public override string ToString()
    {
        string str = "";

        if (_assignedCardTypeCondition)
            str += $"\nCard Type is{GetCardTypeString()}";

        if (_civilizationConditions.Count > 0)
            str += $"\nCivilization is{GetCivilizationString()}";

        if (_raceConditions.Count > 0)
            str += $"\nRace is{GetRaceString()}";

        if (_keywordConditions.Count > 0)
            str += $"\nKeyword is{GetKeywordString()}";

        if (_powerConditions.Count > 0)
            str += $"\nPower is{GetPowerString()}";

        if (_cardConditions.Count > 0)
            str += $"\nCard is{GetCardString()}";

        if (_assignedTapCondition)
        {
            str += "\nTap state is ";
            if (_tapCondition == TapStateType.Tap)
                str += "tapped";
            else if (_tapCondition == TapStateType.Untap)
                str += "untapped";
        }

        return str;
    }

    #endregion
}
