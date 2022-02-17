using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectCondition_EditorExtension
{
    public static void SetConnectToSubCondition(this EffectCondition condition, bool connect)
    {
        EffectCondition_InternalBookkeepingWrapper.SetConnectToSubCondition(condition, connect);
    }

    public static bool GetConnectToSubCondition(this EffectCondition condition)
    {
        return EffectCondition_InternalBookkeepingWrapper.GetConnectToSubCondition(condition);
    }

    public static string GetEditorRepresentationString(this EffectCondition condition)
    {
        string str = GetTypeRepresentation();

        if (condition.TargetingCriterion != null)
            str += $" {condition.TargetingCriterion}";

        if (condition.TargetingCondition != null)
            str += $" where{condition.TargetingCondition}";

        if (condition.GetConnectToSubCondition())
            str += $" {condition.Connector}";

        if (condition.SubCondition)
            str += $" if\n\t{condition.SubCondition}";

        return str;

        #region Local Functions

        string GetTypeRepresentation()
        {
            switch (condition.Type)
            {
                case EffectConditionType.WhileTapState:
                    return $"While {condition.TapState}ped";

                case EffectConditionType.CheckFunction:
                    string str1 = "Check if target ";
                    str1 += condition.CheckHasFunction ? "has" : "doesn't have";
                    str1 += $" function(s)\n\t{condition.SubFunctionality}";
                    return str1;

                default:
                    return condition.Type.ToString();
            }
        }

        #endregion
    }
}
