using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    #region Helper Data Structures

    [System.Serializable]
    public class RevealParam
    {
        public CardZoneType lookAtZone;
        private NumericParamsHolder _numericParams = new NumericParamsHolder();

        public NumericParamsHolder NumericParams
        {
            get { return _numericParams; }
        }

        public override string ToString()
        {
            string str = "Reveal " + _numericParams.CountQuantifier.ToString().ToLower();
            if (_numericParams.CountRangeType == CountRangeType.All)
                str += "all cards";
            else
            {
                str += $"{_numericParams.Count} card";
                if (_numericParams.Count > 1)
                    str += "s ";
            }

            return str;
        }
    }

    #endregion

    public class RevealFuncParam : EffectFunctionalityParameter
    {
        private RevealParam _revealParam = new RevealParam();

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.Reveal
                };
            }
        }
#endif

        public RevealParam RevealParam
        {
            get { return _revealParam; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _revealParam.lookAtZone = EditorUtils.DrawFoldout(_revealParam.lookAtZone);
            
            _revealParam.NumericParams.DrawInspector();
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}