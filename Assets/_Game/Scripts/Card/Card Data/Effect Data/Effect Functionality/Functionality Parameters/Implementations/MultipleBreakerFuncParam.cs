using System.Collections;
using System.Collections.Generic;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class MultipleBreakerFuncParam : EffectFunctionalityParameter
    {
        private MultipleBreakerType _multipleBreaker;

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.MultipleBreaker
                };
            }
        }
#endif

        public MultipleBreakerType MultipleBreaker
        {
            get { return _multipleBreaker; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _multipleBreaker = EditorUtils.DrawFoldout(_multipleBreaker);
        }

        public override bool ShouldAssignCriterion()
        {
            if (_multipleBreaker == MultipleBreakerType.CrewBreaker)
                return true;

            return false;
        }

#endif
    }
}