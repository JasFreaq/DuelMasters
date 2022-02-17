using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;
using DuelMasters.Card.Data.Effects.TargetingCondition.Parameters;
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

        condition.TargetingCriterion.SetTargetingType(ParameterTargetingType.Check);

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

        if (condition.AssignedCriterion)
        {
            DrawTargetingParameter(condition.TargetingCriterion);
            if (GUILayout.Button("Remove Targeting Parameter"))
                condition.AssignedCriterion = false;
        }
        else if (GUILayout.Button("Add Targeting Parameter"))
            condition.AssignedCriterion = true;
        if (condition.AssignedCondition)
        {
            DrawTargetingCondition(condition.TargetingCondition);
            if (GUILayout.Button("Remove Targeting Condition"))
            {
                condition.AssignedCondition = false;
            }
        }
        else if (GUILayout.Button("Add Targeting Condition"))
        {
            condition.AssignedCondition = true;
        }

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
                if (!functionality.TargetingCriterion.TypeEquals(ParameterTargetingType.Affect))
                    functionality.TargetingCriterion.SetTargetingType(ParameterTargetingType.Affect);
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
            if (functionality.ShouldMultiplyVal &&
                !functionality.TargetingCriterion.TypeEquals(ParameterTargetingType.Count))
                functionality.TargetingCriterion.SetTargetingType(ParameterTargetingType.Count);
        }

        functionality.ConnectSubFunctionality = GUILayout.Toggle(functionality.ConnectSubFunctionality, "Connect");
        if (functionality.ConnectSubFunctionality)
            functionality.Connector = DrawFoldout(functionality.Connector);

        GUILayout.EndHorizontal();

        if (functionality.TargetUnspecified() && (functionality.ShouldMultiplyVal || functionality.TargetCard == CardTargetType.AutoTarget))
            DrawTargetingParameter(functionality.TargetingCriterion);

        if (functionality.AssignedCondition)
        {
            DrawTargetingCondition(functionality.TargetingCondition);
            if (GUILayout.Button("Remove Targeting Condition"))
            {
                functionality.AssignedCondition = false;
            }
        }
        else if (GUILayout.Button("Add Targeting Condition"))
        {
            functionality.AssignedCondition = true;
        }

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
                movementZones.countQuantifier = DrawFoldout(movementZones.countQuantifier);
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
            destroyParam.countRangeType = DrawFoldout(destroyParam.countRangeType);
            if (destroyParam.countRangeType == CountRangeType.Number)
            {
                if (int.TryParse(EditorGUILayout.TextField($"{destroyParam.destroyCount}"), out int num))
                    destroyParam.destroyCount = num;
            }
        }

        void DrawDiscardParam()
        {
            DiscardParam discardParam = functionality.DiscardParam;
            discardParam.countRangeType = DrawFoldout(discardParam.countRangeType);
            if (discardParam.countRangeType == CountRangeType.Number)
            {
                if (int.TryParse(EditorGUILayout.TextField($"{discardParam.discardCount}"), out int num))
                    discardParam.discardCount = num;
            }
        }

        void DrawLookAtParam()
        {
            LookAtParam lookAtParam = functionality.LookAtParam;
            lookAtParam.lookAtZone = DrawFoldout(lookAtParam.lookAtZone);
            lookAtParam.countRangeType = DrawFoldout(lookAtParam.countRangeType);
            if (lookAtParam.countRangeType == CountRangeType.Number)
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

    private void DrawTargetingParameter(EffectTargetingCriterion criterion)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Targeting Parameter:", EditorStyles.boldLabel);

        ParameterTargetingType targetingType = criterion.GetTargetingType();
        GUILayout.Label($"{targetingType}");

        if (targetingType != ParameterTargetingType.Count)
        {
            criterion.CountRangeType = DrawFoldout(criterion.CountRangeType);
            if (criterion.CountRangeType == CountRangeType.Number)
            {
                criterion.CountQuantifier = DrawFoldout(criterion.CountQuantifier);
                if (int.TryParse(EditorGUILayout.TextField($"{criterion.Count}"), out int num))
                    criterion.Count = num;
            }
        }

        GUILayout.Label("In");
        criterion.OwningPlayer = DrawFoldout(criterion.OwningPlayer);
        criterion.ZoneType = DrawFoldout(criterion.ZoneType);
        if (criterion.OwningPlayer == PlayerTargetType.Player) 
        {
            if (_cardData is CreatureData)
                criterion.IncludeSelf = GUILayout.Toggle(criterion.IncludeSelf, "Include Self");
        }
        else if (criterion.OwningPlayer == PlayerTargetType.Opponent)
            criterion.OpponentChooses = GUILayout.Toggle(criterion.OpponentChooses, "Opponent Chooses");

        GUILayout.EndHorizontal();
    }

    private void DrawTargetingCondition(EffectTargetingCondition condition)
    {
        EditorGUILayout.LabelField("Targeting Condition:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        GUILayout.BeginVertical();

        //TODO: Remove Assign from below and put elsewhere
        EffectTargetingCondition_ParamSettingWrapper.AssignConditionParams(condition);
        EffectTargetingCondition_ParamSettingWrapper.DrawConditionParamsLayout(condition);
        
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
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
