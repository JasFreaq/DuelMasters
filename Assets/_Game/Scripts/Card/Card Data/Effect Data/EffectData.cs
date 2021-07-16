using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EffectData : ScriptableObject
{
    [HideInInspector] [SerializeField] private bool _mayUseFunction, _triggerPenaltyIfUsed;
    [HideInInspector] [SerializeField] private EffectCondition _effectCondition;
    [HideInInspector] [SerializeField] private EffectFunctionality _effectFunctionality;

#if UNITY_EDITOR
    [HideInInspector] [SerializeField] public bool isBeingEdited = false;
#endif

    public bool MayUseFunction
    {
        get { return _mayUseFunction; }

#if UNITY_EDITOR
        set { _mayUseFunction = value; }
#endif
    }
    
    public bool TriggerPenaltyIfUsed
    {
        get { return _triggerPenaltyIfUsed; }

#if UNITY_EDITOR
        set { _triggerPenaltyIfUsed = value; }
#endif
    }

    public EffectCondition EffectCondition
    {
        get { return _effectCondition; }

#if UNITY_EDITOR
        set { _effectCondition = value; }
#endif
    }

    public EffectFunctionality EffectFunctionality
    {
        get { return _effectFunctionality; }

#if UNITY_EDITOR
        set { _effectFunctionality = value; }
#endif
    }
    
    public override string ToString()
    {
        string str = _effectCondition ? $"{_effectCondition}\n" : "";

        if (_mayUseFunction)
            str += "May ";

        str += $"{_effectFunctionality}";

        if (_triggerPenaltyIfUsed)
            str += $", if does, then\n{_effectCondition.SubFunctionality}";

        return str;
    }
}
