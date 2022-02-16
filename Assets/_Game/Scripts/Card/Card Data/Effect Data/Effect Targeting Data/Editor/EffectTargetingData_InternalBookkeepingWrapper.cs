using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using UnityEngine;

public class EffectTargetingData_InternalBookkeepingWrapper
{
    private static Dictionary<int, ParameterTargetingType> DataToTargetingDict =
        new Dictionary<int, ParameterTargetingType>();

    public static void SetTargetingType(EffectTargetingData targetingData, ParameterTargetingType targetingType)
    {
        DataToTargetingDict[targetingData.GetHashCode()] = targetingType;
    }
    
    public static ParameterTargetingType GetTargetingType(EffectTargetingData targetingData)
    {
        return DataToTargetingDict[targetingData.GetHashCode()];
    }
}