using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#region Helper Data Structures

[System.Serializable]
public enum ConditionType
{
    Check,
    Affect,
    Count
}

#endregion

[System.Serializable]
public class EffectTargetingParameter
{
    [SerializeReference] private ConditionType _type;
    [SerializeReference] private CountType _countType;
    [SerializeReference] private CountChoiceType _countChoice;
    [SerializeReference] private int _count = 0;
    [SerializeReference] private SubZoneType _subZone;

    [SerializeReference] private bool _includeSelf;
    [SerializeReference] private bool _opponentChooses;

#if UNITY_EDITOR
    [SerializeReference] public bool ownerIsCreature;
#endif

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

    public SubZoneType SubZone
    {
        get { return _subZone; }

#if UNITY_EDITOR
        set { _subZone = value; }
#endif
    }

    public bool IncludeSelf
    {
        get { return _includeSelf; }

#if UNITY_EDITOR
        set { _includeSelf = value; }
#endif
    }
    
    public bool OpponentChooses
    {
        get { return _opponentChooses; }

#if UNITY_EDITOR
        set { _opponentChooses = value; }
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
        
        str += $"in {_subZone}";

        int regionValue = (int) Enum.Parse(typeof(SubZoneType), _subZone.ToString());
        if (regionValue < 6 && !_includeSelf && ownerIsCreature) 
        {
            str += " except itself";
        }
        else if (regionValue > 7 && _opponentChooses)
            str += " chosen by opponent";

        return str;
    }
}