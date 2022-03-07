using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class AttackTargetFuncParam : EffectFunctionalityParameter
    {
        private AttackType _attackType;

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.AttackTarget
                };
            }
        }
#endif

        public AttackType AttackType
        {
            get { return _attackType; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _attackType = EditorUtils.DrawFoldout(_attackType);
        }

        public override bool ShouldAssignCriterion()
        {
            return true;
        }

#endif
    }
}