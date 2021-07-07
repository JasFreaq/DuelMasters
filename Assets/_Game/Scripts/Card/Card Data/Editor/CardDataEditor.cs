using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

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
        GUIStyle labelStyle = new GUIStyle {fontStyle = FontStyle.BoldAndItalic, fontSize = 16};
        EditorGUILayout.LabelField("Effect Data", labelStyle);
        GUILayout.BeginVertical();

        List<EffectData> removedEffects = new List<EffectData>();

        foreach (EffectData effect in _cardData.ruleEffects)
        {
            if (effect.isBeingEdited)
            {
                if (effect.ConditionAssigned)
                {
                    DrawCondition(effect.EffectCondition);

                    EditorGUILayout.Space(5);
                    if (GUILayout.Button("Remove Condition"))
                        effect.ConditionAssigned = false;
                }
                else if (GUILayout.Button("Add Condition"))
                    effect.ConditionAssigned = true;

                EditorGUILayout.Space(5f);
                DrawFunctionality(effect.EffectFunctionality);

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
                removedEffects.Add(effect);
        }

        foreach (EffectData effect in removedEffects)
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

            effect.EffectCondition = CreateCondition($"{effect.name}/Effect Cond");
            effect.EffectFunctionality = CreateFunctionality($"{effect.name}/Effect Func");
        }

        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
    
    private EffectCondition CreateCondition(string conditionName)
    {
        EffectCondition condition = ScriptableObject.CreateInstance<EffectCondition>();
        condition.name = conditionName;
        AssetDatabase.AddObjectToAsset(condition, _cardData);

        return condition;
    }
    
    private EffectFunctionality CreateFunctionality(string functionalityName)
    {
        EffectFunctionality functionality = ScriptableObject.CreateInstance<EffectFunctionality>();
        functionality.name = functionalityName;
        AssetDatabase.AddObjectToAsset(functionality, _cardData);

        return functionality;
    }

    private void DrawCondition(EffectCondition condition)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Condition:", EditorStyles.boldLabel);
        condition.Type = DrawFoldout(condition.Type);
        condition.MayUse = GUILayout.Toggle(condition.MayUse, "may");
        GUILayout.EndHorizontal();

        DrawTargetingParameter(condition.TargetingParameter);
        DrawTargetingCondition(condition.TargetingCondition);
        DrawSubCondition(condition);
    }
    
    private void DrawFunctionality(EffectFunctionality functionality)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Function:", EditorStyles.boldLabel);
        functionality.Type = DrawFoldout(functionality.Type);
        GUILayout.EndHorizontal();

        DrawTargetingParameter(functionality.TargetingParameter);
        DrawTargetingCondition(functionality.TargetingCondition);
        DrawSubFunctionality(functionality);
    }

    private void DrawTargetingParameter(EffectTargetingParameter parameter)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Targeting Parameter:", EditorStyles.boldLabel);
        parameter.Type = DrawFoldout(parameter.Type);
        if (parameter.Type != ConditionType.Count)
        {
            parameter.CountType = DrawFoldout(parameter.CountType);
            if (parameter.CountType == CountType.Number)
            {
                if (int.TryParse(EditorGUILayout.TextField($"{parameter.Count}"), out int num))
                    parameter.Count = num;
            }
        }

        GUILayout.Label("In");
        parameter.Region = DrawFoldout(parameter.Region);

        GUILayout.EndHorizontal();
    }

    private void DrawTargetingCondition(EffectTargetingCondition condition)
    {
        EditorGUILayout.LabelField("Targeting Condition:", EditorStyles.boldLabel);

        GUIStyle labelStyle = new GUIStyle {fontStyle = FontStyle.Italic, fontSize = 12};

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);

        GUILayout.BeginVertical();

        DrawCardTypeCondition();
        DrawCivilizationCondition();
        DrawRaceCondition();
        DrawKeywordCondition();
        DrawPowerCondition();
        DrawCardCondition();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        
        #region Local Functions

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
            else if (GUILayout.Button("Add Card Type"))
                condition.CardTypeCondition.isAssigned = true;

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
        
        void DrawKeywordCondition()
        {
            if (condition.KeywordConditions.Count > 0)
            {
                List<KeywordCondition> removedConditions = new List<KeywordCondition>();
                GUILayout.Label("Keywords:", labelStyle);

                for (int i = 0, n = condition.KeywordConditions.Count; i < n; i++)
                {
                    KeywordCondition keywordCondition = condition.KeywordConditions[i];

                    GUILayout.BeginHorizontal();

                    keywordCondition.non = GUILayout.Toggle(keywordCondition.non, "Non");
                    keywordCondition.keyword = DrawFoldout(keywordCondition.keyword, 1);
                    if (n > 1 && i < n - 1)
                        keywordCondition.connector = DrawFoldout(keywordCondition.connector);

                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Remove Keyword"))
                    {
                        EditorGUILayout.Space(5);
                        removedConditions.Add(keywordCondition);
                    }
                }

                foreach (KeywordCondition keywordCondition in removedConditions)
                {
                    condition.RemoveKeywordCondition(keywordCondition);
                }
            }

            if (GUILayout.Button("Add Keyword"))
            {
                KeywordCondition keywordCondition = new KeywordCondition();
                condition.AddKeywordCondition(keywordCondition);
            }
        }
        
        void DrawPowerCondition()
        {
            if (condition.PowerConditions.Count > 0)
            {
                List<PowerCondition> removedConditions = new List<PowerCondition>();
                GUILayout.Label("Powers:", labelStyle);

                for (int i = 0, n = condition.PowerConditions.Count; i < n; i++)
                {
                    PowerCondition powerCondition = condition.PowerConditions[i];

                    GUILayout.BeginHorizontal();

                    powerCondition.comparator = DrawFoldout(powerCondition.comparator);
                    powerCondition.power = EditorGUILayout.IntField(powerCondition.power);
                    if (n > 1 && i < n - 1)
                        powerCondition.connector = DrawFoldout(powerCondition.connector);

                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Remove Power"))
                    {
                        EditorGUILayout.Space(5);
                        removedConditions.Add(powerCondition);
                    }
                }

                foreach (PowerCondition powerCondition in removedConditions)
                {
                    condition.RemovePowerCondition(powerCondition);
                }
            }

            if (GUILayout.Button("Add Power"))
            {
                PowerCondition powerCondition = new PowerCondition();
                condition.AddPowerCondition(powerCondition);
            }
        }
        
        void DrawCardCondition()
        {
            if (condition.CardConditions.Count > 0)
            {
                List<CardCondition> removedConditions = new List<CardCondition>();
                GUILayout.Label("Cards:", labelStyle);

                for (int i = 0, n = condition.CardConditions.Count; i < n; i++)
                {
                    CardCondition cardCondition = condition.CardConditions[i];

                    GUILayout.BeginHorizontal();

                    cardCondition.cardData = (CardData) EditorGUILayout.ObjectField(cardCondition.cardData, typeof(CardData), false);
                    if (n > 1 && i < n - 1)
                        cardCondition.connector = DrawFoldout(cardCondition.connector);

                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Remove Card"))
                    {
                        EditorGUILayout.Space(5);
                        removedConditions.Add(cardCondition);
                    }
                }

                foreach (CardCondition cardCondition in removedConditions)
                {
                    condition.RemoveCardCondition(cardCondition);
                }
            }

            if (GUILayout.Button("Add Card"))
            {
                CardCondition cardCondition = new CardCondition();
                condition.AddCardCondition(cardCondition);
            }
        }

        #endregion
    }

    private void DrawSubCondition(EffectCondition parentCondition)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(25);

        GUILayout.BeginVertical();

        if (parentCondition.SubCondition == null)
        {
            if (GUILayout.Button("Add Sub Condition"))
                parentCondition.SubCondition = CreateCondition($"{parentCondition.name}/Sub Cond");
        }
        else
        {
            DrawCondition(parentCondition.SubCondition);

            EditorGUILayout.Space(5);
            if (GUILayout.Button("Remove Sub Condition"))
                RemoveSubCondition(parentCondition);
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        #region Local Functions

        void RemoveSubCondition(EffectCondition superCondition)
        {
            EffectCondition subCondition = superCondition.SubCondition;
            if (subCondition.SubCondition) 
                RemoveSubCondition(subCondition);

            superCondition.SubCondition = null;
            DestroyImmediate(subCondition, true);
        }

        #endregion
    }
    
    private void DrawSubFunctionality(EffectFunctionality parentFunctionality)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(25);

        GUILayout.BeginVertical();

        if (parentFunctionality.SubFunctionality == null) 
        {
            if (GUILayout.Button("Add Sub Functionality"))
                parentFunctionality.SubFunctionality = CreateFunctionality($"{parentFunctionality.name}/Sub Func");
        }
        else
        {
            DrawFunctionality(parentFunctionality.SubFunctionality);

            EditorGUILayout.Space(5);
            if (GUILayout.Button("Remove Sub Functionality"))
                RemoveSubFunctionality(parentFunctionality);
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        #region Local Functions

        void RemoveSubFunctionality(EffectFunctionality superFunctionality)
        {
            EffectFunctionality subFunctionality = superFunctionality.SubFunctionality;
            if (subFunctionality.SubFunctionality)
                RemoveSubFunctionality(subFunctionality);

            superFunctionality.SubFunctionality = null;
            DestroyImmediate(subFunctionality, true);
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
