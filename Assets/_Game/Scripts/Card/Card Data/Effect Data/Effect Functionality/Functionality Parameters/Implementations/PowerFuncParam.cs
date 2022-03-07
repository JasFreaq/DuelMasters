using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class PowerFuncParam : MultipliableFuncParam
    {
        private int _powerBoost;

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.PowerAttacker,
                    EffectFunctionalityType.GrantPower
                };
            }
        }
#endif

        public int PowerBoost
        {
            get { return _powerBoost; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            if (int.TryParse(EditorGUILayout.TextField($"{_powerBoost}"), out int num))
                _powerBoost = num;
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}