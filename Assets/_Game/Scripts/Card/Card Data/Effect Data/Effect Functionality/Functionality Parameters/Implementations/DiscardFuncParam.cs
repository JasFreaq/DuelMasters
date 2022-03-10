using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class DiscardFuncParam : EffectFunctionalityParameter
    {
        private DiscardData _discardData = new DiscardData();

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

        public DiscardData DiscardData
        {
            get { return _discardData; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _discardData.discardType = EditorUtils.DrawFoldout(_discardData.discardType);

            _discardData.numericParams.DrawInspector();
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}