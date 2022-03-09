using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.Condition;
using DuelMasters.Card.Data.Effects.Condition.Parameters;
using UnityEngine;

namespace DuelMasters.Editor.Data.DataSetters
{
    public static class EffectCondition_ParamSettingWrapper
    {
        public static void AssignConditionParam(EffectCondition condition)
        {
            List<EffectConditionParameter> paramTypes =
                ReflectiveEnumerator.GetClassesOfType<EffectConditionParameter>();

            foreach (EffectConditionParameter parameter in paramTypes)
            {
                if (parameter.ConditionType == condition.Type)
                {
                    condition.ConditionParam = parameter;
                    return;
                }
            }

            condition.ConditionParam = null;
        }
    }
}