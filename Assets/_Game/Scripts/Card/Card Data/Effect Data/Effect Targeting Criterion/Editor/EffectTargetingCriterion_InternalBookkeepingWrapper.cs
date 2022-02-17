using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using UnityEngine;

public static class EffectTargetingCriterion_InternalBookkeepingWrapper
{
    private static Dictionary<int, ParameterTargetingType> DataToTargetingDict =
        new Dictionary<int, ParameterTargetingType>();

    public static void SetTargetingType(EffectTargetingCriterion criterion, ParameterTargetingType targetingType)
    {
        DataToTargetingDict[criterion.GetHashCode()] = targetingType;
    }
    
    public static ParameterTargetingType GetTargetingType(EffectTargetingCriterion criterion)
    {
        if (DataToTargetingDict.ContainsKey(criterion.GetHashCode()))
            return DataToTargetingDict[criterion.GetHashCode()];

        return ParameterTargetingType.Check;
    }
}