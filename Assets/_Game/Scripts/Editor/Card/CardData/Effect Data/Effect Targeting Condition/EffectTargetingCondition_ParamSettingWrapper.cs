using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using System;
using System.Collections.Generic;

namespace DuelMasters.Editor.Data.DataSetters
{
    public static class EffectTargetingCondition_ParamSettingWrapper
    {
        public static void AssignConditionParams(EffectTargetingCondition condition)
        {
            List<EffectTargetingConditionParameter> newParams =
                ReflectiveEnumerator.GetClassesOfType<EffectTargetingConditionParameter>();
            if (condition.ConditionParams == null)
            {
                condition.ConditionParams = newParams;

                condition.CardIntrinsicConditionParams = new List<EffectTargetingConditionParameter>();
                foreach (EffectTargetingConditionParameter newParam in newParams)
                {
                    if (newParam is ICardIntrinsicParam)
                        condition.CardIntrinsicConditionParams.Add(newParam);
                }
            }
            else
            {
                PopulateParamListWithMissingTypes(condition, newParams);
            }
        }

        public static void ClearUnassignedParams(EffectTargetingCondition condition)
        {
            List<EffectTargetingConditionParameter> unassignedParams = new List<EffectTargetingConditionParameter>();
            foreach (EffectTargetingConditionParameter param in condition.ConditionParams)
            {
                if (!param.IsAssignedValue())
                    unassignedParams.Add(param);
            }

            foreach (EffectTargetingConditionParameter unassignedParam in unassignedParams)
            {
                condition.ConditionParams.Remove(unassignedParam);
                condition.CardIntrinsicConditionParams.Remove(unassignedParam);
            }
        }

        private static void PopulateParamListWithMissingTypes(EffectTargetingCondition condition,
            List<EffectTargetingConditionParameter> newParams)
        {
            Dictionary<string, EffectTargetingConditionParameter> existingParamsMap =
                new Dictionary<string, EffectTargetingConditionParameter>();

            foreach (EffectTargetingConditionParameter existingParam in condition.ConditionParams)
            {
                Type type = existingParam.GetType();
                existingParamsMap.Add(type.Name, existingParam);
            }

            foreach (EffectTargetingConditionParameter newParam in newParams)
            {
                Type type = newParam.GetType();
                if (!existingParamsMap.ContainsKey(type.Name))
                {
                    condition.ConditionParams.Add(newParam);

                    if (newParam is ICardIntrinsicParam)
                        condition.CardIntrinsicConditionParams.Add(newParam);
                }
            }
        }

        public static void DrawConditionParamsLayout(EffectTargetingCondition condition)
        {
            foreach (EffectTargetingConditionParameter param in condition.ConditionParams)
            {
                param.DrawParamInspector();
            }
        }
    }
}