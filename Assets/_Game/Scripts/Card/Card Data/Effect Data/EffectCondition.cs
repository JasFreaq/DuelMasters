using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using UnityEngine;

public class EffectCondition : ScriptableObject
{
    [SerializeReference] private EffectConditionType _type;
    [SerializeReference] private bool _assignedCriterion, _assignedCondition, _connectSubCondition;
    [SerializeReference] private EffectTargetingCriterion _targetingCriterion = new EffectTargetingCriterion();
    [SerializeReference] private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    [SerializeReference] private ConnectorType _connector;
    [SerializeReference] private EffectCondition _subCondition;
    [SerializeReference] private EffectFunctionality _subFunctionality;

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

    public bool AssignedCriterion
    {
        get { return _assignedCriterion; }
#if UNITY_EDITOR
        set { _assignedCriterion = value; }
#endif
    }

    public bool AssignedCondition
    {
        get { return _assignedCondition; }
#if UNITY_EDITOR
        set { _assignedCondition = value; }
#endif 
    }

    public bool ConnectSubCondition
    {
        get { return _connectSubCondition; }

#if UNITY_EDITOR
        set { _connectSubCondition = value; }
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

    public override string ToString()
    {
        string str = GetTypeRepresentation();

        if (_assignedCriterion)
            str += $" {_targetingCriterion}";
        
        if (_assignedCondition)
            str += $" where{_targetingCondition}";

        if (_connectSubCondition)
            str += $" {_connector}";

        if (_subCondition)
            str += $" if\n\t{_subCondition}";

        return str;

        #region Local Functions

        string GetTypeRepresentation()
        {
            switch (_type)
            {
                case EffectConditionType.WhileTapState:
                    return $"While {_tapState}ped";

                case EffectConditionType.CheckFunction:
                    string str1 = "Check if target ";
                    str1 += _checkHasFunction ? "has" : "doesn't have";
                    str1 += $" function(s)\n\t{_subFunctionality}";
                    return str1;

                default:
                    return _type.ToString();
            }
        }

        #endregion
    }
}
