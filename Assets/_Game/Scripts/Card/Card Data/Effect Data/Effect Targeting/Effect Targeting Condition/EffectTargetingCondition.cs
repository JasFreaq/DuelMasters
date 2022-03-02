using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Parameters
{
    [System.Serializable]
    public class EffectTargetingCondition
    {
        [SerializeReference] private List<EffectTargetingConditionParameter> _targetingConditionParams;
        [SerializeReference] private List<EffectTargetingConditionParameter> _cardIntrinsicTargetingConditionParams;

        public List<EffectTargetingConditionParameter> TargetingConditionParams
        {
            get { return _targetingConditionParams; }

#if UNITY_EDITOR
            set { _targetingConditionParams = value; }
#endif
        }

        public List<EffectTargetingConditionParameter> CardIntrinsicTargetingConditionParams
        {
            get { return _cardIntrinsicTargetingConditionParams; }

#if UNITY_EDITOR
            set { _cardIntrinsicTargetingConditionParams = value; }
#endif
        }

        #region String Formatting

        public string GetConditionParametersString()
        {
            string str = "";

            foreach (EffectTargetingConditionParameter param in CardIntrinsicTargetingConditionParams)
                str += param.GetGameRepresentationString();

            return str;
        }

        #endregion
    }
}