using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class TargetBehaviourFuncParam : EffectFunctionalityParameter
    {
        private TargetBehaviourType _targetBehaviour;

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.TargetBehaviour
                };
            }
        }
#endif

        public TargetBehaviourType TargetBehaviour
        {
            get { return _targetBehaviour; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _targetBehaviour = EditorUtils.DrawFoldout(_targetBehaviour);
        }

        public override bool ShouldAssignCriterion()
        {
            throw new System.NotImplementedException();
        }

#endif
    }
}