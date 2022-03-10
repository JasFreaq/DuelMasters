using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class DestroyFuncParam : EffectFunctionalityParameter
    {
        private DestroyData _destroyData = new DestroyData();

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

        public DestroyData DestroyData
        {
            get { return _destroyData; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _destroyData.destroyZone = EditorUtils.DrawFoldout(_destroyData.destroyZone);
            
            _destroyData.numericParams.DrawInspector();
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}