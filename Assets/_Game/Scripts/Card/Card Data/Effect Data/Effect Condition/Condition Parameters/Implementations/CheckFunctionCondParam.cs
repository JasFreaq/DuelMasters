using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Condition.Parameters
{
    public class CheckFunctionCondParam : EffectConditionParameter
    {
        private bool _checkHasFunction = true;

#if UNITY_EDITOR

        public override EffectConditionType ConditionType
        {
            get { return EffectConditionType.CheckFunction; }
        }

#endif

        public bool CheckHasFunction
        {
            get { return _checkHasFunction; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _checkHasFunction = GUILayout.Toggle(_checkHasFunction, "Has Function");
        }

#endif
    }
}