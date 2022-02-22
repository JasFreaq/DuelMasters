using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using DuelMasters.Editor.Data.DataSetters;

namespace DuelMasters.Editor.Data.Extensions
{
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
}