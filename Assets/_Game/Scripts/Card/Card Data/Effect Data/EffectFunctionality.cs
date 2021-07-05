using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectFunctionality
{
    private EffectFunctionType _type;
    private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();

    #region Type Specific Members

    private EffectRegionType[] _fromToRegions = new EffectRegionType[2];

    public EffectRegionType[] FromToRegions
    {
        get { return _fromToRegions; }
    }

    #endregion

    public EffectFunctionType Type
    {
        get { return _type; }

#if UNITY_EDITOR
        set { _type = value; }
#endif
    }

    public EffectTargetingCondition TargetingCondition
    {
        get { return _targetingCondition; }

#if UNITY_EDITOR
        set { _targetingCondition = value; }
#endif
    }

    public override string ToString()
    {
        string str = $"{_type} ";
        str += _targetingCondition.ToString();
        return str;
    }
}
