using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectFunctionality
{
    private EffectFunctionType _type;
    private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    private EffectFunctionality _subFunctionality = null;

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

    public EffectTargetingParameter TargetingParameter
    {
        get { return _targetingParameter; }

#if UNITY_EDITOR
        set { _targetingParameter = value; }
#endif
    }

    public EffectTargetingCondition TargetingCondition
    {
        get { return _targetingCondition; }

#if UNITY_EDITOR
        set { _targetingCondition = value; }
#endif
    }

    public EffectFunctionality SubFunctionality
    {
        get { return _subFunctionality; }

#if UNITY_EDITOR
        set { _subFunctionality = value; }
#endif
    }
    
    public override string ToString()
    {
        return $"{_type} {_targetingParameter} {_targetingCondition}";
    }
}
