using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class CostAdjustmentFuncParam : EffectFunctionalityParameter
    {
        private int _costAdjustmentAmount;

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.CostAdjustment
                };
            }
        }
#endif

        public int CostAdjustmentAmount
        {
            get { return _costAdjustmentAmount; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            if (int.TryParse(EditorGUILayout.TextField($"{_costAdjustmentAmount}"), out int num))
                _costAdjustmentAmount = num;
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}