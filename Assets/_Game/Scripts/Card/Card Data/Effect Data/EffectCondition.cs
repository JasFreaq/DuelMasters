using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCondition : ScriptableObject
{
    private EffectConditionType _type;
    private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    private bool _assignedCondition;
    private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    private bool _mayUse = false;
    [HideInInspector] [SerializeField] private EffectCondition _subCondition;
    
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

    public bool AssignedCondition
    {
        get { return _assignedCondition; }

#if UNITY_EDITOR
        set { _assignedCondition = value; }
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
