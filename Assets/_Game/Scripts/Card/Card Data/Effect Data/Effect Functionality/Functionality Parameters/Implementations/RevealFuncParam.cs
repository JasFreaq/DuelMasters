using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class RevealFuncParam : EffectFunctionalityParameter
    {
        private RevealData _revealData = new RevealData();

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.Reveal
                };
            }
        }
#endif

        public RevealData RevealData
        {
            get { return _revealData; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _revealData.lookAtZone = EditorUtils.DrawFoldout(_revealData.lookAtZone);
            
            _revealData.numericParams.DrawInspector();
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}