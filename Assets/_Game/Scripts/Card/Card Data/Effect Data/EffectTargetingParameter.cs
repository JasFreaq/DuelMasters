using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
public enum CountChoiceType
{
    Upto,
    Exactly,
    AtLeast
}

#endregion

[System.Serializable]
public class EffectTargetingParameter
{
    [SerializeReference] private ConditionType _type;
    [SerializeReference] private CountType _countType;
    [SerializeReference] private CountChoiceType _countChoice;
    [SerializeReference] private int _count = 0;
    [SerializeReference] private EffectRegionType _region;
    
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

    public CountChoiceType CountChoice
    {
        get { return _countChoice; }

#if UNITY_EDITOR
        set { _countChoice = value; }
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

    public override string ToString()
    {
        string str = $"{_type} ";

        if (_type != ConditionType.Count)
        {
            if (_countType == CountType.Number)
                str += $"{_countChoice} {_count} ";
            else
                str += $"{_countType} ";
        }

        str += $"in {_region}";

        return str;
    }
}