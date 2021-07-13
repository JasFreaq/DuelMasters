using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EffectData : ScriptableObject
{
    [HideInInspector] [SerializeField] private EffectCondition _effectCondition;
    [HideInInspector] [SerializeField] private EffectFunctionality _effectFunctionality;

#if UNITY_EDITOR
    [HideInInspector] public bool isBeingEdited = false;
#endif
    
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
        str += $"{_effectFunctionality}";
        return str;
    }
}
