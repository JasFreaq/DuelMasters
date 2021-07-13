using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCondition : ScriptableObject
{
    [SerializeReference] private EffectConditionType _type;
    [SerializeReference] private bool _assignedParameter, _assignedCondition;
    [SerializeReference] private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    [SerializeReference] private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    [SerializeReference] private EffectCondition _subCondition;
    
    public EffectConditionType Type
    {
        get { return _type; }

#if UNITY_EDITOR
        set { _type = value; }
#endif
    }

    public bool AssignedParameter
    {
        get { return _assignedParameter; }
#if UNITY_EDITOR
        set { _assignedParameter = value; }
#endif
    }

    public bool AssignedCondition
    {
        get { return _assignedCondition; }
#if UNITY_EDITOR
        set { _assignedCondition = value; }
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

    public EffectCondition SubCondition
    {
        get { return _subCondition; }

#if UNITY_EDITOR
        set { _subCondition = value; }
#endif
    }
    
    public override string ToString()
    {
        string str = $"{_type}";

        if (_assignedParameter)
        {
            str += $" {_targetingParameter}";
            if (_assignedCondition)
                str += $" where{_targetingCondition}";
        }
        else if (_assignedCondition)
            str += $" {_targetingCondition}";

        if (_subCondition)
            str += $" and if\n\t{_subCondition}";
        return str;
    }
}
