using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Helper Data Structures

[System.Serializable]
public enum ConditionType
{
    Check,
    Count,
    Affect
}

[System.Serializable]
public enum CountType
{
    All,
    Number
}

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

#endregion

[System.Serializable]
public class EffectTargetingCondition
{
    private ConditionType _type;
    private CountType _countType;
    private int _count = 0;
    private EffectRegionType _region;

    [SerializeField] private CardTypeCondition _cardTypeCondition = new CardTypeCondition();
    private List<CivilizationCondition> _civilizationConditions = new List<CivilizationCondition>();
    private List<RaceCondition> _raceConditions = new List<RaceCondition>();

    [HideInInspector] public CardParams.Civilization[] tempCivilization;
    [HideInInspector] public CardParams.Race tempRace;

    public ConditionType Type
    {
        get { return _type; }

#if UNITY_EDITOR
        set
        {
            _type = value;
            if (_type == ConditionType.Count)
                _countType = CountType.All;
        }
#endif
    }

    public CountType CountType
    {
        get { return _countType; }

#if UNITY_EDITOR
        set { _countType = value; }
#endif
    }

    public int Count
    {
        get { return _count; }

#if UNITY_EDITOR
        set { _count = value; }
#endif
    }
    
    public EffectRegionType Region
    {
        get { return _region; }

#if UNITY_EDITOR
        set { _region = value; }
#endif
    }

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
#endif
    
    public override string ToString()
    {
        string str = $"{_type} ";

        if (_type != ConditionType.Count)
        {
            if (_countType == CountType.Number)
                str += $"{_count} ";
            else
                str += $"{_countType} ";
        }

        bool writeWhere = true;

        str += $"in {_region}";
        if (_cardTypeCondition.isAssigned)
        {
            WriteWhere();
            str += $"Card Type is {CardParams.StringFromCardType(_cardTypeCondition.cardType)}\n";
        }
        if (_civilizationConditions.Count > 0)
        {
            WriteWhere();
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
            WriteWhere();
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

        #region Local Functions

        void WriteWhere()
        {
            if (writeWhere)
            {
                str += " where";
                writeWhere = false;
            }
        }

        #endregion
    }
}
