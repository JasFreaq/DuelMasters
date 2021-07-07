using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Helper Data Structures

[Serializable]
public enum ConditionType
{
    Check,
    Count,
    Affect
}

[Serializable]
public enum CountType
{
    All,
    Number
}

#endregion

[System.Serializable]
public class EffectTargetingParameter
{
    private ConditionType _type;
    private CountType _countType;
    private int _count = 0;
    private EffectRegionType _region;

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

        str += $"in {_region}";

        return str;
    }
}