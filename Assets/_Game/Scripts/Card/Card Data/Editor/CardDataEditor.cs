using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[CustomEditor(typeof(CardData), true)]
public class CardDataEditor : Editor
{
    private SerializedProperty _ruleEffects;
    private CardData _cardData;

    private void OnEnable()
    {
        _ruleEffects = serializedObject.FindProperty("ruleEffects");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        _cardData = (CardData) target;

        EditorGUILayout.Space(10);
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontStyle = FontStyle.BoldAndItalic;
        labelStyle.fontSize = 16;
        EditorGUILayout.LabelField("Effect Data", labelStyle);
        GUILayout.BeginVertical();

        List<EffectData> _removedEffects = new List<EffectData>();

        foreach (EffectData effect in _cardData.ruleEffects)
        {
            if (effect.isBeingEdited)
            {
                DrawCondition(effect);
                EditorGUILayout.Space(5f);
                DrawFunctionality(effect);

                EditorGUILayout.Space(7.5f);
                if (GUILayout.Button("End Edit"))
                    effect.isBeingEdited = false;
            }
            else
            {
                GUILayout.Label(effect.ToString());

                EditorGUILayout.Space(2.5f);
                if (GUILayout.Button("Edit"))
                    effect.isBeingEdited = true;
            }

            if (GUILayout.Button("Remove Effect"))
                _removedEffects.Add(effect);
        }

        foreach (EffectData effect in _removedEffects)
        {
            _cardData.ruleEffects.Remove(effect);
            DestroyImmediate(effect, true);
        }

        EditorGUILayout.Space(12.5f);
        if (GUILayout.Button("Add Effect"))
        {
            EffectData effect = ScriptableObject.CreateInstance<EffectData>();
            effect.name = $"Effect Data {_cardData.ruleEffects.Count + 1}";

            AssetDatabase.AddObjectToAsset(effect, _cardData);
            _cardData.ruleEffects.Add(effect);
        }

        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCondition(EffectData effect)
    {
        if (effect.effectCondition.IsAssigned)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Effect Condition:", EditorStyles.boldLabel);
            effect.effectCondition.Type = DrawFoldout(effect.effectCondition.Type);
            GUILayout.EndHorizontal();

            DrawTargetingCondition(effect.effectCondition.TargetingCondition);

            EditorGUILayout.Space(5);
            if (GUILayout.Button("Remove Effect Condition"))
                effect.effectCondition.IsAssigned = false;

        }
        else
        {
            if (GUILayout.Button("Add Effect Condition"))
                effect.effectCondition.IsAssigned = true;
        }
    }
    
    private void DrawFunctionality(EffectData effect)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Effect Function:", EditorStyles.boldLabel);
        effect.effectFunctionality.Type = DrawFoldout(effect.effectFunctionality.Type);
        GUILayout.EndHorizontal();

        DrawTargetingCondition(effect.effectFunctionality.TargetingCondition);
    }

    private void DrawTargetingCondition(EffectTargetingCondition condition)
    {
        EditorGUILayout.LabelField("Effect Targeting Condition:", EditorStyles.boldLabel);

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontStyle = FontStyle.Italic;
        labelStyle.fontSize = 12;

        DrawTargetingConditionBaseParameters();
        DrawCardTypeCondition();
        DrawCivilizationCondition();
        DrawRaceCondition();
        
        #region Local Functions

        void DrawTargetingConditionBaseParameters()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Condition Type:");
            condition.Type = DrawFoldout(condition.Type);
            if (condition.Type != ConditionType.Count)
            {
                condition.CountType = DrawFoldout(condition.CountType);
                if (condition.CountType == CountType.Number)
                {
                    if (int.TryParse(EditorGUILayout.TextField($"{condition.Count}"), out int num))
                        condition.Count = num;
                }
            }

            GUILayout.Label("In");
            condition.Region = DrawFoldout(condition.Region);
            GUILayout.Label("Where");

            GUILayout.EndHorizontal();
        }

        void DrawCardTypeCondition()
        {
            GUILayout.BeginHorizontal();

            if (condition.CardTypeCondition.isAssigned)
            {
                GUILayout.Label("Card Type:");
                CardTypeCondition cardTypeCondition = new CardTypeCondition
                {
                    isAssigned = true,
                    cardType = DrawFoldout(condition.CardTypeCondition.cardType)
                };
                condition.CardTypeCondition = cardTypeCondition;

                if (GUILayout.Button("Remove Card Type"))
                    condition.CardTypeCondition.isAssigned = false;
            }
            else
            {
                if (GUILayout.Button("Add Card Type"))
                    condition.CardTypeCondition.isAssigned = true;
            }

            GUILayout.EndHorizontal();
        }

        void DrawCivilizationCondition()
        {
            if (condition.CivilizationConditions.Count > 0)
            {
                List<CivilizationCondition> removedConditions = new List<CivilizationCondition>();
                GUILayout.Label("Civilizations:", labelStyle);
                
                for (int i = 0, n = condition.CivilizationConditions.Count; i < n; i++)
                {
                    CivilizationCondition civilizationCondition = condition.CivilizationConditions[i];
                    CardParams.Civilization[] civilization = civilizationCondition.civilization;

                    GUILayout.BeginHorizontal();
                    civilizationCondition.non = GUILayout.Toggle(civilizationCondition.non, "Non");
                    
                    ReorderableList list = new ReorderableList(civilization,
                        typeof(CardParams.Civilization), true, true, true, false);
                    list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        rect.y += 2;
                        Rect elementRect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);
                        civilization[index] = (CardParams.Civilization) EditorGUI.EnumPopup(elementRect, civilization[index]);
                    };
                    list.drawHeaderCallback = (Rect rect) =>
                    {
                        EditorGUI.LabelField(rect, "Civilization");
                    };
                    list.onAddCallback = reorderableList =>
                    {
                        List<CardParams.Civilization> tempCivilization = new List<CardParams.Civilization>(civilization);
                        tempCivilization.Add(0);
                        civilization = tempCivilization.ToArray();
                    };
                    list.DoLayoutList();

                    civilizationCondition.civilization = civilization;
                    GUILayout.EndHorizontal();
                    
                    if (n > 1 && i < n - 1)
                        civilizationCondition.connector = DrawFoldout(civilizationCondition.connector);

                    if (GUILayout.Button("Remove Civilization"))
                    {
                        EditorGUILayout.Space(5);
                        removedConditions.Add(civilizationCondition);
                    }
                }

                foreach (CivilizationCondition civilizationCondition in removedConditions)
                {
                    condition.RemoveCivilizationCondition(civilizationCondition);
                }
            }

            if (GUILayout.Button("Add Civilization"))
            {
                CardParams.Civilization[] civilization = new CardParams.Civilization[1];
                CivilizationCondition civilizationCondition = new CivilizationCondition
                {
                    civilization = civilization
                };
                condition.AddCivilizationCondition(civilizationCondition);
            }
        }

        void DrawRaceCondition()
        {
            if (condition.RaceConditions.Count > 0)
            {
                List<RaceCondition> removedConditions = new List<RaceCondition>();
                GUILayout.Label("Races:", labelStyle);

                for (int i = 0, n = condition.RaceConditions.Count; i < n; i++)
                {
                    RaceCondition raceCondition = condition.RaceConditions[i];

                    GUILayout.BeginHorizontal();

                    raceCondition.non = GUILayout.Toggle(raceCondition.non, "Non");
                    raceCondition.race = DrawFoldout(raceCondition.race, 1);
                    if (n > 1 && i < n - 1)
                        raceCondition.connector = DrawFoldout(raceCondition.connector);

                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Remove Race"))
                    {
                        EditorGUILayout.Space(5);
                        removedConditions.Add(raceCondition);
                    }
                }

                foreach (RaceCondition raceCondition in removedConditions)
                {
                    condition.RemoveRaceCondition(raceCondition);
                }
            }

            if (GUILayout.Button("Add Race"))
            {
                RaceCondition raceCondition = new RaceCondition();
                condition.AddRaceCondition(raceCondition);
            }
        }

        #endregion
    }

    private T DrawFoldout<T>(T currentValue, int enumIndexAdjustment = 0) where T : Enum
    {
        Type enumType = typeof(T);
        int currentInt = (int) Enum.Parse(enumType, currentValue.ToString());
        int newInt = EditorGUILayout.Popup(currentInt - enumIndexAdjustment, Enum.GetNames(enumType));
        if (currentInt != newInt)
            newInt += enumIndexAdjustment;
        return (T) Enum.Parse(enumType, newInt.ToString());
    }
}
