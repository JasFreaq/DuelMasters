using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.Condition.Parameters;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Condition
{
    public class EffectCondition : ScriptableObject
    {
        [SerializeReference] private EffectConditionType _type;
        [SerializeReference] private EffectConditionParameter _conditionParam;
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
        
        public EffectConditionType Type
        {
            get { return _type; }

#if UNITY_EDITOR
            set { _type = value; }
#endif
        }

        public EffectConditionParameter ConditionParam
        {
            get { return _conditionParam; }

#if UNITY_EDITOR
            set { _conditionParam = value; }
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
    }
}