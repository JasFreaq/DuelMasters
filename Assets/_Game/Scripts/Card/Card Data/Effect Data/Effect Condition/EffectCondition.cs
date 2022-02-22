using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using UnityEngine;

public class EffectCondition : ScriptableObject
{
    [SerializeReference] private EffectConditionType _type;
    [SerializeReference] private EffectTargetingCriterion _targetingCriterion;
    [SerializeReference] private EffectTargetingCondition _targetingCondition;
    [SerializeReference] private ConnectorType _connector;
    [SerializeReference] private EffectCondition _subCondition;
    [SerializeReference] private EffectFunctionality _subFunctionality;

#if UNITY_EDITOR

    #region Editor Only Members

    [SerializeField] public bool connectToSubCondition;

    #endregion
#endif

    #region Type Specific Members

    [SerializeReference] private TapStateType _tapState;
    [SerializeReference] private bool _checkHasFunction = true;

    #endregion

    public EffectConditionType Type
    {
        get { return _type; }

#if UNITY_EDITOR
        set { _type = value; }
#endif
    }
    
    public EffectTargetingCriterion TargetingCriterion
    {
        get { return _targetingCriterion; }

#if UNITY_EDITOR
        set { _targetingCriterion = value; }
#endif
    }

    public EffectTargetingCondition TargetingCondition
    {
        get { return _targetingCondition; }

#if UNITY_EDITOR
        set { _targetingCondition = value; }
#endif
    }

    public ConnectorType Connector
    {
        get { return _connector; }

#if UNITY_EDITOR
        set { _connector = value; }
#endif
    }

    public EffectCondition SubCondition
    {
        get { return _subCondition; }

#if UNITY_EDITOR
        set { _subCondition = value; }
#endif
    }

    public EffectFunctionality SubFunctionality
    {
        get { return _subFunctionality; }

#if UNITY_EDITOR
        set { _subFunctionality = value; }
#endif
    }

    public TapStateType TapState
    {
        get { return _tapState; }

#if UNITY_EDITOR
        set { _tapState = value; }
#endif
    }

    public bool CheckHasFunction
    {
        get { return _checkHasFunction; }

#if UNITY_EDITOR
        set { _checkHasFunction = value; }
#endif
    }
}