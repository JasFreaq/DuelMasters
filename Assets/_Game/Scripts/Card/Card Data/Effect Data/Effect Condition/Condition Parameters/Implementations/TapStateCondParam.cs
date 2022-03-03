using System.Collections;
using System.Collections.Generic;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Condition.Parameters
{
    public class TapStateCondParam : EffectConditionParameter
    {
        private TapStateType _tapState;

#if UNITY_EDITOR

        public override EffectConditionType ConditionType
        {
            get { return EffectConditionType.WhileTapState; }
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

#endif
    }
}