using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using System;
using System.Collections.Generic;

namespace DuelMasters.Editor.Data.DataSetters
{
    public static class EffectTargetingCondition_ParamSettingWrapper
    {
        public static void AssignTargetingConditionParams(EffectTargetingCondition targetingCondition)
        {
            List<EffectTargetingConditionParameter> newParams =
                ReflectiveEnumerator.GetComparableClassesOfType<EffectTargetingConditionParameter>();

            if (targetingCondition.TargetingConditionParams == null)
            {
                targetingCondition.TargetingConditionParams = newParams;

                targetingCondition.CardIntrinsicTargetingConditionParams = new List<EffectTargetingConditionParameter>();
                foreach (EffectTargetingConditionParameter newParam in newParams)
                {
                    if (newParam is ICardIntrinsicParam)
                        targetingCondition.CardIntrinsicTargetingConditionParams.Add(newParam);
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
            
            foreach (EffectTargetingConditionParameter param in targetingCondition.TargetingConditionParams)
            {
                if (!param.IsAssignedValue())
                    unassignedParams.Add(param);
            }

            foreach (EffectTargetingConditionParameter unassignedParam in unassignedParams)
            {
                targetingCondition.TargetingConditionParams.Remove(unassignedParam);
                targetingCondition.CardIntrinsicTargetingConditionParams.Remove(unassignedParam);
            }
        }

        private static void PopulateParamListWithMissingTypes(EffectTargetingCondition targetingCondition,
            List<EffectTargetingConditionParameter> newParams)
        {
            Dictionary<string, EffectTargetingConditionParameter> existingParamsMap =
                new Dictionary<string, EffectTargetingConditionParameter>();

            foreach (EffectTargetingConditionParameter existingParam in targetingCondition.TargetingConditionParams)
            {
                Type type = existingParam.GetType();
                existingParamsMap.Add(type.Name, existingParam);
            }

            foreach (EffectTargetingConditionParameter newParam in newParams)
            {
                Type type = newParam.GetType();
                if (!existingParamsMap.ContainsKey(type.Name))
                {
                    targetingCondition.TargetingConditionParams.Add(newParam);

                    if (newParam is ICardIntrinsicParam)
                        targetingCondition.CardIntrinsicTargetingConditionParams.Add(newParam);
                }
            }

            targetingCondition.TargetingConditionParams.Sort();
            targetingCondition.CardIntrinsicTargetingConditionParams.Sort();
        }
    }
}