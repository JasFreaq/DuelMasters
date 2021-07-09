using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFunctionality : ScriptableObject
{
    private EffectFunctionType _type;
    private EffectTargetingParameter _targetingParameter = new EffectTargetingParameter();
    private bool _assignedCondition;
    private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    [HideInInspector] [SerializeField] private EffectFunctionality _subFunctionality;

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
