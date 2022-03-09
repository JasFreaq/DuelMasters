using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    #region Helper Data Structures

    [System.Serializable]
    public class DestroyParam
    {
        public CardZoneType destroyZone;
        public NumericParamsHolder numericParams = new NumericParamsHolder();
        
        public override string ToString()
        {
            string str = "Destroy " + numericParams.CountQuantifier.ToString().ToLower();
            if (numericParams.CountRangeType == CountRangeType.All)
                str += "all cards";
            else
            {
                str += $"{numericParams.Count} card";
                if (numericParams.Count > 1)
                    str += "s ";
            }

            return str;
        }
    }

    #endregion

    public class DestroyFuncParam : EffectFunctionalityParameter
    {
        private DestroyParam _destroyParam = new DestroyParam();

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.Destroy
                };
            }
        }
#endif

        public DestroyParam DestroyParam
        {
            get { return _destroyParam; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _destroyParam.destroyZone = EditorUtils.DrawFoldout(_destroyParam.destroyZone);
            
            _destroyParam.numericParams.DrawInspector();
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}