using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectCondition
{
    private EffectConditionType _type;
    private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    private bool _mayUse = false;
    private EffectCondition _subCondition = null;
    
    public EffectConditionType Type
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

    public bool MayUse
    {
        get { return _mayUse; }

#if UNITY_EDITOR
        set { _mayUse = value; }
#endif
    }

    public EffectCondition SubCondition
    {
        get { return _subCondition; }

#if UNITY_EDITOR
        set { _subCondition = value; }
#endif
    }
    
    public override string ToString()
    {
        string str = $"{_type} {_targetingParameter} {_targetingCondition}";
        if (_mayUse)
            str += "may";
        return str;
    }
}
