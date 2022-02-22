using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Editor.Data.Extensions
{
    public static class EffectTargetingCriterion_EditorExtension
    {
        public static void DrawInspector(this EffectTargetingCriterion criterion, CardData cardData)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Targeting Criterion:", EditorStyles.boldLabel);

            ParameterTargetingType targetingType = criterion.targetingType;
            GUILayout.Label($"{targetingType}");

            if (targetingType != ParameterTargetingType.Count)
            {
                criterion.CountRangeType = EditorUtils.DrawFoldout(criterion.CountRangeType);
                if (criterion.CountRangeType == CountRangeType.Number)
                {
                    criterion.CountQuantifier = EditorUtils.DrawFoldout(criterion.CountQuantifier);
                    if (int.TryParse(EditorGUILayout.TextField($"{criterion.Count}"), out int num))
                        criterion.Count = num;
                }
            }

            GUILayout.Label("In");
            criterion.OwningPlayer = EditorUtils.DrawFoldout(criterion.OwningPlayer);
            criterion.ZoneType = EditorUtils.DrawFoldout(criterion.ZoneType);
            if (criterion.OwningPlayer == PlayerTargetType.Player)
            {
                if (cardData is CreatureData)
                    criterion.IncludeSelf = GUILayout.Toggle(criterion.IncludeSelf, "Include Self");
            }
            else if (criterion.OwningPlayer == PlayerTargetType.Opponent)
                criterion.OpponentChooses = GUILayout.Toggle(criterion.OpponentChooses, "Opponent Chooses");

            GUILayout.EndHorizontal();
        }

        public static string GetEditorRepresentationString(this EffectTargetingCriterion criterion)
        {
            ParameterTargetingType targetingType = criterion.targetingType;
            string str = $"{targetingType} ";

            if (targetingType != ParameterTargetingType.Count)
            {
                if (criterion.CountRangeType == CountRangeType.Number)
                    str += $"{criterion.CountQuantifier} {criterion.Count} ";
                else
                    str += $"{criterion.CountRangeType} ";
            }

            PlayerTargetType owningPlayer = criterion.OwningPlayer;
            str += $"in {owningPlayer} {criterion.ZoneType}";

            if (owningPlayer == PlayerTargetType.Player &&
                !criterion.IncludeSelf)
                str += " except itself";
            else if (owningPlayer == PlayerTargetType.Opponent && criterion.OpponentChooses)
                str += " chosen by opponent";

            return str;
        }
    }
}