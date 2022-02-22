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
        [SerializeReference] private List<EffectTargetingConditionParameter> _conditionParams;
        [SerializeReference] private List<EffectTargetingConditionParameter> _cardIntrinsicConditionParams;

        public List<EffectTargetingConditionParameter> ConditionParams
        {
            get { return _conditionParams; }

#if UNITY_EDITOR
            set { _conditionParams = value; }
#endif
        }

        public List<EffectTargetingConditionParameter> CardIntrinsicConditionParams
        {
            get { return _cardIntrinsicConditionParams; }

#if UNITY_EDITOR
            set { _cardIntrinsicConditionParams = value; }
#endif
        }

        #region String Formatting

        public string GetConditionParametersString()
        {
            string str = "";

            foreach (EffectTargetingConditionParameter param in CardIntrinsicConditionParams)
                str += param.GetGameRepresentationString();

            return str;
        }

        #endregion
    }
}