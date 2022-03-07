using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
using DuelMasters.Editor.Data.Extensions;
using System;
using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.Condition;
using DuelMasters.Card.Data.Effects.Functionality.Parameters;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Editor.Data
{
    [CustomEditor(typeof(CardData), true)]
    public class CardDataEditor : UnityEditor.Editor
    {
        private CardData _cardData;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            _cardData = (CardData)target;

            EditorGUILayout.Space(10);
            GUIStyle labelStyle = new GUIStyle { fontStyle = FontStyle.BoldAndItalic, fontSize = 16 };
            EditorGUILayout.LabelField("Effect Data", labelStyle);
            GUILayout.BeginVertical();

            List<EffectData> removedEffects = new List<EffectData>();

            foreach (EffectData effect in _cardData.ruleEffects)
            {
                if (effect.isBeingEdited)
                {
                    if (effect.EffectCondition)
                    {
                        GUILayout.BeginHorizontal();
                        effect.MayUseFunction = GUILayout.Toggle(effect.MayUseFunction, "May Use");
                        if (effect.MayUseFunction)
                        {
                            effect.TriggerPenaltyIfUsed = GUILayout.Toggle(effect.TriggerPenaltyIfUsed, "Trigger Penalty If Used");
                            if (effect.TriggerPenaltyIfUsed && !effect.EffectCondition.SubFunctionality)
                            {
                                effect.EffectCondition.SubFunctionality = CreateFunctionality($"{effect.name}/Penalty Func");
                            }
                            else if (!effect.TriggerPenaltyIfUsed && effect.EffectCondition.SubFunctionality)
                            {
                                RemoveFunctionality(effect.EffectCondition.SubFunctionality);
                            }
                        }
                        GUILayout.EndHorizontal();

                        DrawCondition(effect.EffectCondition, "Condition:");

                        EditorGUILayout.Space(5);
                        if (GUILayout.Button("Remove Condition"))
                            RemoveCondition(effect.EffectCondition);
                    }
                    else if (GUILayout.Button("Add Condition"))
                        effect.EffectCondition = CreateCondition($"{effect.name}/Effect Cond");

                    EditorGUILayout.Space(5f);
                    DrawFunctionality(effect.EffectFunctionality, "Function:");

                    EditorGUILayout.Space(7.5f);
                    if (GUILayout.Button("End Edit"))
                    {
                        effect.isBeingEdited = false;
                        if (effect.EffectCondition) 
                            effect.EffectCondition.TargetingCondition?.ClearUnassignedParams();

                        //TODO: Refactor Functionality and Condition under same base class
                    }
                }
                else
                {
                    GUILayout.Label(effect.ToString());

                    EditorGUILayout.Space(2.5f);
                    if (GUILayout.Button("Edit"))
                    {
                        effect.isBeingEdited = true;
                    }
                }

                if (GUILayout.Button("Remove Effect"))
                {
                    removedEffects.Add(effect);
                }
            }

            foreach (EffectData effect in removedEffects)
            {
                _cardData.ruleEffects.Remove(effect);
                RemoveCondition(effect.EffectCondition);
                RemoveFunctionality(effect.EffectFunctionality);

                DestroyImmediate(effect, true);
            }

            EditorGUILayout.Space(12.5f);
            if (GUILayout.Button("Add Effect"))
            {
                EffectData effect = CreateInstance<EffectData>();
                effect.name = $"Effect Data {_cardData.ruleEffects.Count + 1}";

                _cardData.ruleEffects.Add(effect);
                AssetDatabase.AddObjectToAsset(effect, _cardData);

                effect.EffectFunctionality = CreateFunctionality($"{effect.name}/Effect Func");

                EditorUtility.SetDirty(effect);
            }

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                AssetDatabase.SaveAssets();
        }

        #region Serialization Methods

        private EffectCondition CreateCondition(string conditionName)
        {
            EffectCondition condition = CreateInstance<EffectCondition>();
            condition.name = conditionName;

            condition.TargetingCriterion = new EffectTargetingCriterion();
            condition.TargetingCriterion.targetingType = ParameterTargetingType.Check;

            AssetDatabase.AddObjectToAsset(condition, _cardData);
            EditorUtility.SetDirty(condition);

            return condition;
        }

        private EffectFunctionality CreateFunctionality(string functionalityName)
        {
            EffectFunctionality functionality = CreateInstance<EffectFunctionality>();
            functionality.name = functionalityName;
            AssetDatabase.AddObjectToAsset(functionality, _cardData);
            EditorUtility.SetDirty(functionality);

            return functionality;
        }

        private void RemoveCondition(EffectCondition condition)
        {
            if (condition)
            {
                if (condition.SubCondition)
                    RemoveCondition(condition.SubCondition);
            }

            DestroyImmediate(condition, true);
        }

        private void RemoveFunctionality(EffectFunctionality functionality)
        {
            if (functionality && functionality.SubFunctionality)
                RemoveFunctionality(functionality.SubFunctionality);

            DestroyImmediate(functionality, true);
        }

        #endregion

        #region Drawing Methods

        private void DrawCondition(EffectCondition condition, string labelText)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(labelText, EditorStyles.boldLabel);

            condition.Type = EditorUtils.DrawFoldout(condition.Type, out bool changed);
            if (changed) 
                condition.AssignConditionParam();
            condition.ConditionParam?.DrawInspector();

            condition.connectToSubCondition = (GUILayout.Toggle(condition.connectToSubCondition, "Connect"));
            if (condition.connectToSubCondition)
                condition.Connector = EditorUtils.DrawFoldout(condition.Connector);

            GUILayout.EndHorizontal();

            if (condition.TargetingCriterion != null)
            {
                condition.TargetingCriterion.DrawInspector(_cardData);
                if (GUILayout.Button("Remove Targeting Criterion"))
                {
                    condition.TargetingCriterion = null;
                }
            }
            else if (GUILayout.Button("Add Targeting Criterion"))
            {
                condition.TargetingCriterion = new EffectTargetingCriterion();
                condition.TargetingCriterion.targetingType = ParameterTargetingType.Check;
            }

            if (condition.TargetingCondition != null)
            {
                condition.TargetingCondition.DrawInspector();
                if (GUILayout.Button("Remove Targeting Condition"))
                    condition.TargetingCondition = null;
            }
            else if (GUILayout.Button("Add Targeting Condition"))
                condition.TargetingCondition = new EffectTargetingCondition();

            DrawSubCondition(condition);
            DrawSubFunctionality(condition);

            EditorUtility.SetDirty(condition);
        }

        private void DrawFunctionality(EffectFunctionality functionality, string labelText)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(labelText, EditorStyles.boldLabel);

            functionality.Type = EditorUtils.DrawFoldout(functionality.Type, out bool changed);
            if (changed) 
                functionality.AssignConditionParam();
            functionality.FunctionalityParam?.DrawInspector();

            functionality.TargetType = EditorUtils.DrawFoldout(functionality.TargetType);
            switch (functionality.TargetType)
            {
                case CardTargetType.AutoTarget:
                    if (functionality.TargetingCriterion.targetingType != ParameterTargetingType.Affect)
                        functionality.TargetingCriterion.targetingType = ParameterTargetingType.Affect;
                    break;

                case CardTargetType.TargetOther:
                    GUILayout.Label("Chooser:");
                    functionality.ChoosingPlayer = EditorUtils.DrawFoldout(functionality.ChoosingPlayer);
                    GUILayout.Label("Target:");
                    functionality.TargetPlayer = EditorUtils.DrawFoldout(functionality.TargetPlayer);
                    break;
            }

            MultipliableFuncParam multipliable = functionality.FunctionalityParam as MultipliableFuncParam;
            if (multipliable != null) 
            {
                multipliable.DrawInspector();
                if (multipliable.ShouldMultiplyVal &&
                    functionality.TargetingCriterion.targetingType != ParameterTargetingType.Count)
                    functionality.TargetingCriterion.targetingType = ParameterTargetingType.Count;
            }

            functionality.Connector = EditorUtils.DrawFoldout(functionality.Connector);

            GUILayout.EndHorizontal();

            if (functionality.FunctionalityParam.ShouldAssignCriterion())
            {
                if ((multipliable != null && multipliable.ShouldMultiplyVal) ||
                    functionality.TargetType == CardTargetType.AutoTarget) 
                {
                    functionality.TargetingCriterion.DrawInspector(_cardData);
                }
            }

            if (functionality.TargetingCondition != null)
            {
                functionality.TargetingCondition.DrawInspector();
                if (GUILayout.Button("Remove Targeting Condition"))
                    functionality.TargetingCondition = null;
            }
            else if (GUILayout.Button("Add Targeting Condition"))
                functionality.TargetingCondition = new EffectTargetingCondition();

            DrawSubFunctionality(functionality);

            EditorUtility.SetDirty(functionality);
        }

        private void DrawSubCondition(EffectCondition parentCondition)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            GUILayout.BeginVertical();

            if (parentCondition.SubCondition)
            {
                DrawCondition(parentCondition.SubCondition, "Sub-condition:");

                EditorGUILayout.Space(5);
                if (GUILayout.Button("Remove Sub Condition"))
                    RemoveCondition(parentCondition.SubCondition);
            }
            else
            {
                if (GUILayout.Button("Add Sub Condition"))
                    parentCondition.SubCondition = CreateCondition($"{parentCondition.name}/Sub Cond");
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawSubFunctionality(EffectCondition parentCondition)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            GUILayout.BeginVertical();

            if (parentCondition.SubFunctionality)
            {
                DrawFunctionality(parentCondition.SubFunctionality, "Sub-function:");

                EditorGUILayout.Space(5);
                if (GUILayout.Button("Remove Sub Functionality"))
                    RemoveFunctionality(parentCondition.SubFunctionality);
            }
            else
            {
                if (GUILayout.Button("Add Sub Functionality"))
                    parentCondition.SubFunctionality = CreateFunctionality($"{parentCondition.name}/Sub Func");
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawSubFunctionality(EffectFunctionality parentCondition)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            GUILayout.BeginVertical();

            if (parentCondition.SubFunctionality)
            {
                DrawFunctionality(parentCondition.SubFunctionality, "Sub-function:");

                EditorGUILayout.Space(5);
                if (GUILayout.Button("Remove Sub Functionality"))
                    RemoveFunctionality(parentCondition.SubFunctionality);
            }
            else
            {
                if (GUILayout.Button("Add Sub Functionality"))
                    parentCondition.SubFunctionality = CreateFunctionality($"{parentCondition.name}/Sub Func");
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        #endregion
    }
}