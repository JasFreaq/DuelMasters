using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using DuelMasters.Editor.Data.DataSetters;
using UnityEditor;
using UnityEngine;

namespace DuelMasters.Editor.Data.Extensions
{
    public static class EffectTargetingCondition_EditorExtension
    {
        public static void DrawInspector(this EffectTargetingCondition condition)
        {
            if (condition != null)
            {
                EditorGUILayout.LabelField("Targeting Condition:", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.BeginVertical();

                AssignConditionParams(condition);
                DrawConditionParamsLayout(condition);

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        private static void AssignConditionParams(EffectTargetingCondition condition)
        {
            EffectTargetingCondition_ParamSettingWrapper.AssignConditionParams(condition);
        }

        private static void DrawConditionParamsLayout(EffectTargetingCondition condition)
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