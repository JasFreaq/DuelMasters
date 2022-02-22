using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EffectConditionParameter
{
    protected EffectConditionType _conditionType;

#if UNITY_EDITOR

    public EffectConditionType ConditionType
    {
        get { return _conditionType; }
    }

#endif
}