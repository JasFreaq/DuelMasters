using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using UnityEngine;

public class EffectTargetingData_InternalBookkeepingWrapper
{
    private static Dictionary<int, ParameterTargetingType> DataToTargetingDict =
        new Dictionary<int, ParameterTargetingType>();

    public static void SetTargetingType(EffectTargetingCriterion targetingCriterion, ParameterTargetingType targetingType)
    {
        DataToTargetingDict[targetingCriterion.GetHashCode()] = targetingType;
    }
    
    public static ParameterTargetingType GetTargetingType(EffectTargetingCriterion targetingCriterion)
    {
        return DataToTargetingDict[targetingCriterion.GetHashCode()];
    }
}