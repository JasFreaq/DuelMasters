using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public class AlterFunctionFuncParam : EffectFunctionalityParameter
    {
        private bool _alterFunctionUntilEndOfTurn = true;

#if UNITY_EDITOR

        public override IReadOnlyList<EffectFunctionalityType> FunctionalityTypes
        {
            get
            {
                return new List<EffectFunctionalityType>()
                {
                    EffectFunctionalityType.GrantFunction,
                    EffectFunctionalityType.DisableFunction
                };
            }
        }
#endif

        public bool AlterFunctionUntilEndOfTurn
        {
            get { return _alterFunctionUntilEndOfTurn; }
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            _alterFunctionUntilEndOfTurn = GUILayout.Toggle(_alterFunctionUntilEndOfTurn, "Until End Of Turn");
        }

        public override bool ShouldAssignCriterion()
        {
            return false;
        }

#endif
    }
}