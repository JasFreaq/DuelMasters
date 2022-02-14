using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectTargetingData_EditorExtension
{
    public static void SetTargetingType(this EffectTargetingData targetingData, ParameterTargetingType targetingType)
    {
        EffectTargetingData_InternalBookkeepingWrapper.SetTargetingType(targetingData, targetingType);
        if (targetingType == ParameterTargetingType.Count)
            targetingData.CountRangeType = CountRangeType.All;

    }

    public static ParameterTargetingType GetTargetingType(this EffectTargetingData targetingData)
    {
        return EffectTargetingData_InternalBookkeepingWrapper.GetTargetingType(targetingData);
    }

    public static bool TypeEquals(this EffectTargetingData targetingData, ParameterTargetingType targetingType)
    {
        return GetTargetingType(targetingData) == targetingType;
    }

    public static string GetEditorRepresentationString(this EffectTargetingData targetingData)
    {
        ParameterTargetingType targetingType = targetingData.GetTargetingType();
        string str = $"{targetingType} ";

        if (targetingType != ParameterTargetingType.Count)
        {
            if (targetingData.CountRangeType == CountRangeType.Number)
                str += $"{targetingData.CountQuantifier} {targetingData.Count} ";
            else
                str += $"{targetingData.CountRangeType} ";
        }

        PlayerTargetType owningPlayer = targetingData.OwningPlayer;
        str += $"in {owningPlayer} {targetingData.ZoneType}";

        if (owningPlayer == PlayerTargetType.Player && 
            !targetingData.IncludeSelf)
            str += " except itself";
        else if (owningPlayer == PlayerTargetType.Opponent && targetingData.OpponentChooses)
            str += " chosen by opponent";

        return str;
    }
}
