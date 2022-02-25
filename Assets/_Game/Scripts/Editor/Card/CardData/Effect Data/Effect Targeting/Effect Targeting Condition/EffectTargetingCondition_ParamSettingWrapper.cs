using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using System;
using System.Collections.Generic;

namespace DuelMasters.Editor.Data.DataSetters
{
    public static class EffectTargetingCondition_ParamSettingWrapper
    {
        public static void AssignConditionParams(EffectTargetingCondition targetingCondition)
        {
            List<EffectTargetingConditionParameter> newParams =
                ReflectiveEnumerator.GetComparableClassesOfType<EffectTargetingConditionParameter>();

            if (targetingCondition.ConditionParams == null)
            {
                targetingCondition.ConditionParams = newParams;

                targetingCondition.CardIntrinsicConditionParams = new List<EffectTargetingConditionParameter>();
                foreach (EffectTargetingConditionParameter newParam in newParams)
                {
                    if (newParam is ICardIntrinsicParam)
                        targetingCondition.CardIntrinsicConditionParams.Add(newParam);
                }
            }
            else
            {
                PopulateParamListWithMissingTypes(targetingCondition, newParams);
            }
        }

        public static void ClearUnassignedParams(EffectTargetingCondition targetingCondition)
        {
            List<EffectTargetingConditionParameter> unassignedParams = new List<EffectTargetingConditionParameter>();
            
            foreach (EffectTargetingConditionParameter param in targetingCondition.ConditionParams)
            {
                if (!param.IsAssignedValue())
                    unassignedParams.Add(param);
            }

            foreach (EffectTargetingConditionParameter unassignedParam in unassignedParams)
            {
                targetingCondition.ConditionParams.Remove(unassignedParam);
                targetingCondition.CardIntrinsicConditionParams.Remove(unassignedParam);
            }
        }

        private static void PopulateParamListWithMissingTypes(EffectTargetingCondition targetingCondition,
            List<EffectTargetingConditionParameter> newParams)
        {
            Dictionary<string, EffectTargetingConditionParameter> existingParamsMap =
                new Dictionary<string, EffectTargetingConditionParameter>();

            foreach (EffectTargetingConditionParameter existingParam in targetingCondition.ConditionParams)
            {
                Type type = existingParam.GetType();
                existingParamsMap.Add(type.Name, existingParam);
            }

            foreach (EffectTargetingConditionParameter newParam in newParams)
            {
                Type type = newParam.GetType();
                if (!existingParamsMap.ContainsKey(type.Name))
                {
                    targetingCondition.ConditionParams.Add(newParam);

                    if (newParam is ICardIntrinsicParam)
                        targetingCondition.CardIntrinsicConditionParams.Add(newParam);
                }
            }

            targetingCondition.ConditionParams.Sort();
            targetingCondition.CardIntrinsicConditionParams.Sort();
        }
    }
}