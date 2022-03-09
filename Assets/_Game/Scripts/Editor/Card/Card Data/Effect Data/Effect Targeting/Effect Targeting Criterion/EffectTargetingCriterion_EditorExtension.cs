using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Editor.Data.Extensions
{
    public static class EffectTargetingCriterion_EditorExtension
    {
        public static void DrawInspector(this EffectTargetingCriterion targetingCriterion, CardData cardData)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Targeting Criterion:", EditorStyles.boldLabel);

            ParameterTargetingType targetingType = targetingCriterion.targetingType;
            GUILayout.Label($"{targetingType}");

            if (targetingType != ParameterTargetingType.Count)
            {
                targetingCriterion.CountRangeType = EditorUtils.DrawFoldout(targetingCriterion.CountRangeType);
                if (targetingCriterion.CountRangeType == CountRangeType.Number)
                {
                    targetingCriterion.CountQuantifier = EditorUtils.DrawFoldout(targetingCriterion.CountQuantifier);
                    if (int.TryParse(EditorGUILayout.TextField($"{targetingCriterion.Count}"), out int num))
                        targetingCriterion.Count = num;
                }
            }

            GUILayout.Label("In");
            targetingCriterion.OwningPlayer = EditorUtils.DrawFoldout(targetingCriterion.OwningPlayer);
            targetingCriterion.ZoneType = EditorUtils.DrawFoldout(targetingCriterion.ZoneType);
            if (targetingCriterion.OwningPlayer == PlayerTargetType.Player)
            {
                if (cardData is CreatureData)
                    targetingCriterion.IncludeSelf = GUILayout.Toggle(targetingCriterion.IncludeSelf, "Include Self");
            }
            else if (targetingCriterion.OwningPlayer == PlayerTargetType.Opponent)
                targetingCriterion.OpponentChooses = GUILayout.Toggle(targetingCriterion.OpponentChooses, "Opponent Chooses");

            GUILayout.EndHorizontal();
        }

        public static string GetEditorRepresentationString(this EffectTargetingCriterion targetingCriterion)
        {
            ParameterTargetingType targetingType = targetingCriterion.targetingType;
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
}