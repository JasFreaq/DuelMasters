using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.Condition;
using DuelMasters.Card.Data.Effects.Condition.Parameters;
using DuelMasters.Card.Data.Effects.Functionality.Parameters;
using UnityEngine;

namespace DuelMasters.Editor.Data.DataSetters
{
    public static class EffectFunctionality_ParamSettingWrapper
    {
        public static void AssignFunctionalityParam(EffectFunctionality functionality)
        {
            List<EffectFunctionalityParameter> paramTypes =
                ReflectiveEnumerator.GetClassesOfType<EffectFunctionalityParameter>();

            foreach (EffectFunctionalityParameter parameter in paramTypes)
            {
                IReadOnlyList<EffectFunctionalityType> types = parameter.FunctionalityTypes;
                foreach (EffectFunctionalityType type in types)
                {
                    if (type == functionality.Type)
                    {
                        functionality.FunctionalityParam = parameter;
                        return;
                    }
                }
            }

            functionality.FunctionalityParam = null;
        }
    }
}