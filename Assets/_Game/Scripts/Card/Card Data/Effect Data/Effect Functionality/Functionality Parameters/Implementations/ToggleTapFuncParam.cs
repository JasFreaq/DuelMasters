using System.Collections;
using System.Collections.Generic;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class ToggleTapFuncParam : EffectFunctionalityParameter
    {
        private TapStateType _tapState;

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.ToggleTap
                };
            }
        }
#endif

        public TapStateType TapState
        {
            get { return _tapState; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _tapState = EditorUtils.DrawFoldout(_tapState);
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}