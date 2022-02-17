using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using UnityEngine;

public static class EffectTargetingCriterion_EditorExtension
{
    public static void SetTargetingType(this EffectTargetingCriterion targetingCriterion, ParameterTargetingType targetingType)
    {
        EffectTargetingData_InternalBookkeepingWrapper.SetTargetingType(targetingCriterion, targetingType);
        if (targetingType == ParameterTargetingType.Count)
            targetingCriterion.CountRangeType = CountRangeType.All;

    }

    public static ParameterTargetingType GetTargetingType(this EffectTargetingCriterion targetingCriterion)
    {
        return EffectTargetingData_InternalBookkeepingWrapper.GetTargetingType(targetingCriterion);
    }

    public static bool TypeEquals(this EffectTargetingCriterion targetingCriterion, ParameterTargetingType targetingType)
    {
        return GetTargetingType(targetingCriterion) == targetingType;
    }

    public static string GetEditorRepresentationString(this EffectTargetingCriterion targetingCriterion)
    {
        ParameterTargetingType targetingType = targetingCriterion.GetTargetingType();
        string str = $"{targetingType} ";

        if (targetingType != ParameterTargetingType.Count)
        {
            if (targetingCriterion.CountRangeType == CountRangeType.Number)
                str += $"{targetingCriterion.CountQuantifier} {targetingCriterion.Count} ";
            else
                str += $"{targetingCriterion.CountRangeType} ";
        }

        PlayerTargetType owningPlayer = targetingCriterion.OwningPlayer;
        str += $"in {owningPlayer} {targetingCriterion.ZoneType}";

        if (owningPlayer == PlayerTargetType.Player && 
            !targetingCriterion.IncludeSelf)
            str += " except itself";
        else if (owningPlayer == PlayerTargetType.Opponent && targetingCriterion.OpponentChooses)
            str += " chosen by opponent";

        return str;
    }
}
