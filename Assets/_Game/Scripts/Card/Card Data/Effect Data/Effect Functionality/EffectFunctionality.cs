using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.Functionality.Parameters;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using UnityEngine;

public class EffectFunctionality : ScriptableObject
{
    [SerializeReference] private EffectFunctionalityType _type;
    [SerializeReference] private CardTargetType _targetType;
    [SerializeReference] private PlayerTargetType _choosingPlayer;
    [SerializeReference] private PlayerTargetType _targetPlayer;
    [SerializeReference] private EffectFunctionalityParameter _functionalityParam;
    [SerializeReference] private EffectTargetingCriterion _targetingCriterion = new EffectTargetingCriterion();
    [SerializeReference] private EffectTargetingCondition _targetingCondition;
    [SerializeReference] private ConnectorType _connector;
    [SerializeReference] private EffectFunctionality _subFunctionality;

    public EffectFunctionality()
    {
        _functionalityParam = new AlterFunctionFuncParam();
    }

    public EffectFunctionalityType Type
    {
        get { return _type; }

#if UNITY_EDITOR
        set { _type = value; }
#endif
    }

    public CardTargetType TargetType
    {
        get { return _targetType; }

#if UNITY_EDITOR
        set { _targetType = value; }
#endif
    }

    public PlayerTargetType ChoosingPlayer
    {
        get { return _choosingPlayer; }

#if UNITY_EDITOR
        set { _choosingPlayer = value; }
#endif
    }

    public PlayerTargetType TargetPlayer
    {
        get { return _targetPlayer; }

#if UNITY_EDITOR
        set { _targetPlayer = value; }
#endif
    }

    public EffectFunctionalityParameter FunctionalityParam
    {
        get { return _functionalityParam; }

#if UNITY_EDITOR
        set { _functionalityParam = value; }
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

    public EffectFunctionality SubFunctionality
    {
        get { return _subFunctionality; }

#if UNITY_EDITOR
        set { _subFunctionality = value; }
#endif
    }
}