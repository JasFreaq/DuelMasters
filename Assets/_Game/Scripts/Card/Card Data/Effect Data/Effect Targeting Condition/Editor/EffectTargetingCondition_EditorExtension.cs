using System.Collections;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using UnityEngine;

public static class EffectTargetingCondition_EditorExtension
{
    public static void AssignConditionParams(this EffectTargetingCondition condition)
    {
        EffectTargetingCondition_ParamSettingWrapper.AssignConditionParams(condition);
    }

    public static void DrawConditionParamsLayout(this EffectTargetingCondition condition)
    {
        EffectTargetingCondition_ParamSettingWrapper.DrawConditionParamsLayout(condition);
    }

    public static string GetEditorRepresentationString(this EffectTargetingCondition condition)
    {
        string str = "";

        foreach (EffectTargetingConditionParameter param in condition.ConditionParams)
            str += param.GetEditorRepresentationString();

        return str;
    }
}
