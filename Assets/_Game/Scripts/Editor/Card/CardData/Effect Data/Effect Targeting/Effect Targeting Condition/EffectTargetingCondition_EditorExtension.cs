using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using DuelMasters.Editor.Data.DataSetters;
using UnityEditor;
using UnityEngine;

namespace DuelMasters.Editor.Data.Extensions
{
    public static class EffectTargetingCondition_EditorExtension
    {
        public static void DrawInspector(this EffectTargetingCondition targetingCondition)
        {
            if (targetingCondition != null)
            {
                EditorGUILayout.LabelField("Targeting Condition:", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.BeginVertical();

                AssignConditionParams(targetingCondition);
                DrawConditionParamsLayout(targetingCondition);

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        private static void AssignConditionParams(EffectTargetingCondition targetingCondition)
        {
            EffectTargetingCondition_ParamSettingWrapper.AssignTargetingConditionParams(targetingCondition);
        }

        private static void DrawConditionParamsLayout(EffectTargetingCondition targetingCondition)
        {
            foreach (EffectTargetingConditionParameter param in targetingCondition.TargetingConditionParams)
            {
                param.DrawInspector();
            }
        }

        public static void ClearUnassignedParams(this EffectTargetingCondition targetingCondition)
        {
            EffectTargetingCondition_ParamSettingWrapper.ClearUnassignedParams(targetingCondition);
        }

        public static string GetEditorRepresentationString(this EffectTargetingCondition targetingCondition)
        {
            string str = "";

            foreach (EffectTargetingConditionParameter param in targetingCondition.TargetingConditionParams)
                str += param.GetEditorRepresentationString();

            return str;
        }
    }
}