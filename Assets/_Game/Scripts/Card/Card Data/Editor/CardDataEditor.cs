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

    private void DrawCondition(EffectCondition condition, string labelText)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(labelText, EditorStyles.boldLabel);
        condition.Type = DrawFoldout(condition.Type);

        switch (condition.Type)
        {
            case EffectConditionType.WhileTapState:
                condition.TapState = DrawFoldout(condition.TapState);
                break;

            case EffectConditionType.CheckFunction:
                condition.CheckHasFunction = GUILayout.Toggle(condition.CheckHasFunction, "Has Function");
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
        DrawSubFunctionality(condition);

        EditorUtility.SetDirty(condition);
    }

    private void DrawFunctionality(EffectFunctionality functionality, string labelText)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(labelText, EditorStyles.boldLabel);
        
        functionality.Type = DrawFoldout(functionality.Type);

        bool showMultiplyVal = false;
        DrawFunctionTypeSpecificParams();

        functionality.TargetCard = DrawFoldout(functionality.TargetCard);
        switch (functionality.TargetCard)
        {
            case CardTargetType.AutoTarget:
                if (functionality.TargetingParameter.Type != ConditionType.Affect)
                    functionality.TargetingParameter.Type = ConditionType.Affect;
                break;

            case CardTargetType.TargetOther:
                GUILayout.Label("Chooser:");
                functionality.ChoosingPlayer = DrawFoldout(functionality.ChoosingPlayer);
                GUILayout.Label("Target:");
                functionality.TargetPlayer = DrawFoldout(functionality.TargetPlayer);
                break;
        }

        if (showMultiplyVal)
        {
            functionality.ShouldMultiplyVal = GUILayout.Toggle(functionality.ShouldMultiplyVal, "Multiply val");
            if (functionality.ShouldMultiplyVal && functionality.TargetingParameter.Type != ConditionType.Count)
                functionality.TargetingParameter.Type = ConditionType.Count;
        }

        functionality.ConnectSubFunctionality = GUILayout.Toggle(functionality.ConnectSubFunctionality, "Connect");
        if (functionality.ConnectSubFunctionality)
            functionality.Connector = DrawFoldout(functionality.Connector);

        GUILayout.EndHorizontal();

        if (functionality.TargetUnspecified() && functionality.ShouldMultiplyVal || functionality.TargetCard == CardTargetType.AutoTarget)
            DrawTargetingParameter(functionality.TargetingParameter);

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

        void DrawFunctionTypeSpecificParams()
        {
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
                    if (functionality.Keyword == KeywordType.VortexEvolution)
                        DrawVortexRaces();
                    break;

                case EffectFunctionalityType.MultipleBreaker:
                    functionality.MultipleBreaker = DrawFoldout(functionality.MultipleBreaker);
                    break;

                case EffectFunctionalityType.ToggleTap:
                    functionality.TapState = DrawFoldout(functionality.TapState);
                    break;

                case EffectFunctionalityType.Destroy:
                    DrawDestroyParam();
                    break;

                case EffectFunctionalityType.Discard:
                    DrawDiscardParam();
                    break;

                case EffectFunctionalityType.LookAtRegion:
                    DrawLookAtParam();
                    break;

                case EffectFunctionalityType.PowerAttacker:
                case EffectFunctionalityType.GrantPower:
                    if (int.TryParse(EditorGUILayout.TextField($"{functionality.PowerBoost}"), out int num1))
                        functionality.PowerBoost = num1;
                    DrawMultiplyVal();
                    break;

                case EffectFunctionalityType.CostAdjustment:
                    if (int.TryParse(EditorGUILayout.TextField($"{functionality.CostAdjustmentAmount}"), out int num2))
                        functionality.CostAdjustmentAmount = num2;
                    break;
            }
        }

        void DrawMovementRegions()
        {
            MovementZones movementZones = functionality.MovementZones;

            movementZones.fromZone = DrawFoldout(movementZones.fromZone);

            if (movementZones.fromZone == CardZoneType.Deck)
            {
                movementZones.deckCardMove = DrawFoldout(movementZones.deckCardMove);
                if (movementZones.deckCardMove == DeckCardMoveType.SearchShuffle)
                    movementZones.showSearchedCard = GUILayout.Toggle(movementZones.showSearchedCard, "Show Card");
            }

            if (movementZones.moveCount > 1)
                movementZones.countChoice = DrawFoldout(movementZones.countChoice);
            if (int.TryParse(EditorGUILayout.TextField($"{movementZones.moveCount}"), out int num))
                movementZones.moveCount = num;
            DrawMultiplyVal();

            movementZones.toZone = DrawFoldout(movementZones.toZone);

            if (movementZones.toZone == CardZoneType.Deck)
                movementZones.deckCardMove = DrawFoldout(movementZones.deckCardMove);
        }

        void DrawVortexRaces()
        {
            GUILayout.BeginVertical();
            
            if (functionality.VortexRaces.Count > 0)
            {
                List<RaceHolder> removedConditions = new List<RaceHolder>();
                GUILayout.Label("Races:");

                for (int i = 0, n = functionality.VortexRaces.Count; i < n; i++)
                {
                    RaceHolder raceHolder = functionality.VortexRaces[i];

                    GUILayout.BeginHorizontal();

                    raceHolder.race = DrawFoldout(raceHolder.race, 1);

                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Remove Race"))
                    {
                        EditorGUILayout.Space(5);
                        removedConditions.Add(raceHolder);
                    }
                }

                foreach (RaceHolder raceHolder in removedConditions)
                {
                    functionality.VortexRaces.Remove(raceHolder);
                }
            }

            if (GUILayout.Button("Add Race"))
            {
                RaceHolder raceHolder = new RaceHolder();
                functionality.VortexRaces.Add(raceHolder);
            }
            
            GUILayout.EndVertical();
        }

        void DrawDestroyParam()
        {
            DestroyParam destroyParam = functionality.DestroyParam;
            destroyParam.destroyZone = DrawFoldout(destroyParam.destroyZone);
            destroyParam.countType = DrawFoldout(destroyParam.countType);
            if (destroyParam.countType == CountType.Number)
            {
                if (int.TryParse(EditorGUILayout.TextField($"{destroyParam.destroyCount}"), out int num))
                    destroyParam.destroyCount = num;
            }
        }

        void DrawDiscardParam()
        {
            DiscardParam discardParam = functionality.DiscardParam;
            discardParam.countType = DrawFoldout(discardParam.countType);
            if (discardParam.countType == CountType.Number)
            {
                if (int.TryParse(EditorGUILayout.TextField($"{discardParam.discardCount}"), out int num))
                    discardParam.discardCount = num;
            }
        }

        void DrawLookAtParam()
        {
            LookAtParam lookAtParam = functionality.LookAtParam;
            lookAtParam.lookAtZone = DrawFoldout(lookAtParam.lookAtZone);
            lookAtParam.countType = DrawFoldout(lookAtParam.countType);
            if (lookAtParam.countType == CountType.Number)
            {
                if (int.TryParse(EditorGUILayout.TextField($"{lookAtParam.lookCount}"), out int num))
                    lookAtParam.lookCount = num;
            }
        }

        void DrawMultiplyVal()
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
        parameter.OwningPlayer = DrawFoldout(parameter.OwningPlayer);
        parameter.ZoneType = DrawFoldout(parameter.ZoneType);
        if (parameter.OwningPlayer == PlayerTargetType.Player) 
        {
            if (_cardData is CreatureData)
            {
                parameter.IncludeSelf = GUILayout.Toggle(parameter.IncludeSelf, "Include Self");
                parameter.ownerIsCreature = true;
            }
        }
        else if (parameter.OwningPlayer == PlayerTargetType.Opponent)
            parameter.OpponentChooses = GUILayout.Toggle(parameter.OpponentChooses, "Opponent Chooses");

        GUILayout.EndHorizontal();
    }

    private void DrawTargetingCondition(EffectTargetingCondition condition)
    {
        EditorGUILayout.LabelField("Targeting Condition:", EditorStyles.boldLabel);

        GUIStyle labelStyle = new GUIStyle { fontStyle = FontStyle.Italic, fontSize = 12 };

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
                        civilization[index] = (CardParams.Civilization)EditorGUI.EnumPopup(elementRect, civilization[index]);
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

                    cardCondition.cardData = (CardData)EditorGUILayout.ObjectField(cardCondition.cardData, typeof(CardData), false);
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
