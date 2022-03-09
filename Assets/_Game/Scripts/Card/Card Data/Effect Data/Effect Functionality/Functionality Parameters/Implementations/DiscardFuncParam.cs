using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    #region Helper Data Structures

    [System.Serializable]
    public class DiscardParam
    {
        public DiscardType discardType;
        public NumericParamsHolder numericParams = new NumericParamsHolder();
        
        public override string ToString()
        {
            string str = "discards " + numericParams.CountQuantifier.ToString().ToLower();
            if (numericParams.CountRangeType == CountRangeType.All)
                str += "all cards";
            else
            {
                str += $"{numericParams.Count} card";
                if (numericParams.Count > 1)
                    str += "s ";

                switch (discardType)
                {
                    case DiscardType.Random:
                        str += "at random";
                        break;
                    case DiscardType.PlayerChoose:
                        str += "chosen by player";
                        break;
                    case DiscardType.OpponentChoose:
                        str += "chosen by opponent";
                        break;
                }
            }

            return str;
        }
    }

    #endregion

    public class DiscardFuncParam : EffectFunctionalityParameter
    {
        private DiscardParam _discardParam = new DiscardParam();

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.Discard
                };
            }
        }
#endif

        public DiscardParam DiscardParam
        {
            get { return _discardParam; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _discardParam.discardType = EditorUtils.DrawFoldout(_discardParam.discardType);

            _discardParam.numericParams.DrawInspector();
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}