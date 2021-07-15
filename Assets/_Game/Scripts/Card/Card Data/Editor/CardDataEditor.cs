using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CustomEditor(typeof(CardData), true)]
public class CardDataEditor : Editor
{
    private CardData _cardData;

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
                if (effect.EffectCondition)
                {
                    effect.MayUseCondition = GUILayout.Toggle(effect.MayUseCondition, "May Use");
                    DrawCondition(effect.EffectCondition);

                    EditorGUILayout.Space(5);
                    if (GUILayout.Button("Remove Condition"))
                        RemoveCondition(effect.EffectCondition);
                }
                else if (GUILayout.Button("Add Condition"))
                    effect.EffectCondition = CreateCondition($"{effect.name}/Effect Cond");

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
        condition.TargetingParameter.Type = ConditionType.Check;

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
        if (condition && condition.SubCondition)
            RemoveCondition(condition.SubCondition);

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

    private void DrawCondition(EffectCondition condition)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Condition:", EditorStyles.boldLabel);
        condition.Type = DrawFoldout(condition.Type);

        switch (condition.Type)
        {
            case EffectConditionType.WhileTapState:
                condition.TapState = DrawFoldout(condition.TapState);
                break;
        }

        condition.ConnectSubCondition = GUILayout.Toggle(condition.ConnectSubCondition, "Connect");
        if (condition.ConnectSubCondition)
            condition.Connector = DrawFoldout(condition.Connector);

        GUILayout.EndHorizontal();
        
        if (condition.AssignedParameter)
        {
            DrawTargetingParameter(condition.TargetingParameter);
            if (GUILayout.Button("Remove Targeting Parameter"))
                condition.AssignedParameter = false;
        }
        else if (GUILayout.Button("Add Targeting Parameter"))
            condition.AssignedParameter = true;
        if (condition.AssignedCondition)
        {
            DrawTargetingCondition(condition.TargetingCondition);
            if (GUILayout.Button("Remove Targeting Condition"))
                condition.AssignedCondition = false;
        }
        else if (GUILayout.Button("Add Targeting Condition"))
            condition.AssignedCondition = true;

        DrawSubCondition(condition);
        
        EditorUtility.SetDirty(condition);
    }
    
    private void DrawFunctionality(EffectFunctionality functionality)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Function:", EditorStyles.boldLabel);
        functionality.Type = DrawFoldout(functionality.Type);

        bool showMultiplyVal = false;
        switch (functionality.Type)
        {
            case EffectFunctionalityType.GrantFunction:
            case EffectFunctionalityType.DisableFunction:
                functionality.AlterFunctionUntilEndOfTurn = GUILayout.Toggle(functionality.AlterFunctionUntilEndOfTurn, "Until End Of Turn");
                break;

            case EffectFunctionalityType.RegionMovement:
                DrawMovementRegions();
                break;

            case EffectFunctionalityType.AttackTarget:
                functionality.AttackType = DrawFoldout(functionality.AttackType);
                break;

            case EffectFunctionalityType.TargetBehaviour:
                functionality.TargetBehaviour = DrawFoldout(functionality.TargetBehaviour);
                break;

            case EffectFunctionalityType.Keyword:
                functionality.Keyword = DrawFoldout(functionality.Keyword);
                break;

            case EffectFunctionalityType.MultipleBreaker:
                functionality.MultipleBreaker = DrawFoldout(functionality.MultipleBreaker);
                break;

            case EffectFunctionalityType.ToggleTap:
                functionality.TapState = DrawFoldout(functionality.TapState);
                break;

            case EffectFunctionalityType.Discard:
                functionality.DiscardType = DrawFoldout(functionality.DiscardType);
                break;

            case EffectFunctionalityType.PowerAttacker:
                if (int.TryParse(EditorGUILayout.TextField($"{functionality.PowerAttackerBoost}"), out int num2))
                    functionality.PowerAttackerBoost = num2;
                break;

            case EffectFunctionalityType.GrantPower:
                if (int.TryParse(EditorGUILayout.TextField($"{functionality.AttackBoostGrant}"), out int num3))
                    functionality.AttackBoostGrant = num3;
                ShowMultiplyVal();
                break;

            case EffectFunctionalityType.CostAdjustment:
                if (int.TryParse(EditorGUILayout.TextField($"{functionality.CostAdjustmentAmount}"), out int num4))
                    functionality.CostAdjustmentAmount = num4;
                break;
        }

        FunctionTargetType initialTarget = functionality.Target;
        functionality.Target = DrawFoldout(functionality.Target);

        if (showMultiplyVal) 
        {
            bool initialMultiplyVal = functionality.ShouldMultiplyVal;
            functionality.ShouldMultiplyVal = GUILayout.Toggle(functionality.ShouldMultiplyVal, "Multiply val");
            if (functionality.ShouldMultiplyVal && !initialMultiplyVal)
                functionality.TargetingParameter.Type = ConditionType.Count;
        }

        functionality.ConnectSubFunctionality = GUILayout.Toggle(functionality.ConnectSubFunctionality, "Connect");
        if (functionality.ConnectSubFunctionality)
            functionality.Connector = DrawFoldout(functionality.Connector);

        GUILayout.EndHorizontal();

        if (initialTarget != FunctionTargetType.TargetOther &&
            functionality.Target == FunctionTargetType.TargetOther)
        {
            functionality.TargetingParameter.Type = ConditionType.Affect;
        }

        if (functionality.TargetUnspecified())
        {
            if (functionality.Target == FunctionTargetType.TargetOther || functionality.ShouldMultiplyVal)
                DrawTargetingParameter(functionality.TargetingParameter);
        }

        if (functionality.AssignedCondition)
        {
            DrawTargetingCondition(functionality.TargetingCondition);
            if (GUILayout.Button("Remove Targeting Condition"))
                functionality.AssignedCondition = false;
        }
        else if (GUILayout.Button("Add Targeting Condition"))
            functionality.AssignedCondition = true;

        DrawSubFunctionality(functionality);

        EditorUtility.SetDirty(functionality);

        #region Local Functions

        void DrawMovementRegions()
        {
            MovementRegions movementRegions = functionality.MovementRegions;

            movementRegions.fromRegion = DrawFoldout(movementRegions.fromRegion);

            if (movementRegions.fromRegion == RegionType.Deck)
            {
                movementRegions.deckCardMove = DrawFoldout(movementRegions.deckCardMove);
                if (movementRegions.deckCardMove == DeckCardMoveType.SearchShuffle)
                    movementRegions.showSearchedCard = GUILayout.Toggle(movementRegions.showSearchedCard, "Show Card");
            }

            CardMoveParam moveParam = functionality.MoveParam;
            if (moveParam.moveCount > 1)
                moveParam.countChoice = DrawFoldout(moveParam.countChoice);
            if (int.TryParse(EditorGUILayout.TextField($"{moveParam.moveCount}"), out int num1))
                moveParam.moveCount = num1;
            functionality.MoveParam = moveParam;
            
            movementRegions.toRegion = DrawFoldout(movementRegions.toRegion);

            if (movementRegions.toRegion == RegionType.Deck)
                movementRegions.deckCardMove = DrawFoldout(movementRegions.deckCardMove);

            functionality.MovementRegions = movementRegions;
        }

        void ShowMultiplyVal()
        {
            showMultiplyVal = true;
            GUILayout.Label(": val");
        }

        #endregion
    }

    private void DrawTargetingParameter(EffectTargetingParameter parameter)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Targeting Parameter:", EditorStyles.boldLabel);
        GUILayout.Label($"{parameter.Type}");
        if (parameter.Type != ConditionType.Count)
        {
            parameter.CountType = DrawFoldout(parameter.CountType);
            if (parameter.CountType == CountType.Number)
            {
                parameter.CountChoice = DrawFoldout(parameter.CountChoice);
                if (int.TryParse(EditorGUILayout.TextField($"{parameter.Count}"), out int num))
                    parameter.Count = num;
            }
        }

        GUILayout.Label("In");
        parameter.Region = DrawFoldout(parameter.Region);
        int regionValue = (int) Enum.Parse(typeof(GameRegionType), parameter.Region.ToString());
        if (regionValue < 6) 
        {
            if (_cardData is CreatureData)
            {
                parameter.IncludeSelf = GUILayout.Toggle(parameter.IncludeSelf, "Include Self");
                parameter.ownerIsCreature = true;
            }
        }
        else if (regionValue > 7)
                parameter.OpponentChooses = GUILayout.Toggle(parameter.OpponentChooses, "Opponent Chooses");
        
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
        DrawTapStateCondition();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        
        #region Local Functions

        void DrawCardTypeCondition()
        {
            GUILayout.BeginHorizontal();

            if (condition.AssignedCardTypeCondition)
            {
                GUILayout.Label("Card Type:");
                condition.CardTypeCondition = DrawFoldout(condition.CardTypeCondition, 1);

                if (GUILayout.Button("Remove Card Type"))
                    condition.AssignedCardTypeCondition = false;
            }
            else if (GUILayout.Button("Add Card Type"))
                condition.AssignedCardTypeCondition = true;

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
                    keywordCondition.keyword = DrawFoldout(keywordCondition.keyword);
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

        void DrawTapStateCondition()
        {
            GUILayout.BeginHorizontal();

            if (condition.AssignedTapCondition)
            {
                GUILayout.Label("Tap State:");
                condition.TapCondition = DrawFoldout(condition.TapCondition);

                if (GUILayout.Button("Remove Tap State"))
                    condition.AssignedTapCondition = false;
            }
            else if (GUILayout.Button("Add Tap State"))
                condition.AssignedTapCondition = true;

            GUILayout.EndHorizontal();
        }

        #endregion
    }

    private void DrawSubCondition(EffectCondition parentCondition)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);

        GUILayout.BeginVertical();

        if (parentCondition.SubCondition)
        {
            DrawCondition(parentCondition.SubCondition);

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
    
    private void DrawSubFunctionality(EffectFunctionality parentFunctionality)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);

        GUILayout.BeginVertical();

        if (parentFunctionality.SubFunctionality) 
        {
            DrawFunctionality(parentFunctionality.SubFunctionality);

            EditorGUILayout.Space(5);
            if (GUILayout.Button("Remove Sub Functionality"))
                RemoveFunctionality(parentFunctionality.SubFunctionality);
        }
        else
        {
            if (GUILayout.Button("Add Sub Functionality"))
                parentFunctionality.SubFunctionality = CreateFunctionality($"{parentFunctionality.name}/Sub Func");
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
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

    #endregion
}
