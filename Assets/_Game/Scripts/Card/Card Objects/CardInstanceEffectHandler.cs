using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardInstanceEffectHandler
{
    #region Helper Data Structures
    struct WhileConditionHolder
    {
        public EffectTargetingParameter targetingParameter;
        public EffectTargetingCondition targetingCondition;

        public Action<bool> processFunction;
        public bool effectActive;
    }

    #endregion

    #region Data Members

    private CardObject _cardObj;
    private CardData _cardData;

    private readonly bool _isBlocker;

    private bool _isMultipleBreaker;
    private MultipleBreakerType _multipleBreakerType;

    private bool _isPowerAttacker;
    private int _powerBoost;

    private Action _whenPlayed;
    private Action _whenPutIntoBattle;
    private Action _whenDestroyed;
    private Action _whenWouldBeDestroyed;

    private List<WhileConditionHolder> _whileConditions = new List<WhileConditionHolder>();
    
    #endregion

    public CardInstanceEffectHandler(CardData cardData)
    {
        _cardData = cardData;
        foreach (EffectData effectData in cardData.ruleEffects)
        {
            EffectFunctionality functionality = effectData.EffectFunctionality;
            switch (functionality.Type)
            {
                case EffectFunctionalityType.GrantFunction:
                case EffectFunctionalityType.DisableFunction:
                    break;

                case EffectFunctionalityType.RegionMovement:
                    break;

                case EffectFunctionalityType.AttackTarget:
                    break;

                case EffectFunctionalityType.TargetBehaviour:
                    break;

                case EffectFunctionalityType.Keyword:
                    if (functionality.Keyword == KeywordType.Blocker)
                        _isBlocker = true;
                    break;

                case EffectFunctionalityType.MultipleBreaker:
                    _isMultipleBreaker = true;
                    _multipleBreakerType = functionality.MultipleBreaker;
                    break;

                case EffectFunctionalityType.ToggleTap:
                    break;

                case EffectFunctionalityType.Destroy:
                    break;

                case EffectFunctionalityType.Discard:
                    break;

                case EffectFunctionalityType.LookAtRegion:
                    break;

                case EffectFunctionalityType.PowerAttacker:
                    _isPowerAttacker = true;
                    _powerBoost = functionality.PowerBoost;
                    break;

                case EffectFunctionalityType.CostAdjustment:
                    break;
            }
        }
    }

    #region Properties

    public CardObject CardObj
    {
        set { _cardObj = value; }
    }

    public bool IsBlocker
    {
        get { return _isBlocker; }
    }

    public bool IsMultipleBreaker
    {
        get { return _isMultipleBreaker; }
    }

    public MultipleBreakerType MultipleBreakerType
    {
        get { return _multipleBreakerType; }
    }

    public bool IsPowerAttacker
    {
        get { return _isPowerAttacker; }
    }

    public int PowerBoost
    {
        get { return _powerBoost; }
    }

    public bool HasWhileCondition
    {
        get { return _whileConditions.Count > 0; }
    }

    #endregion

    public void SetupRuleEffects()
    {
        foreach (EffectData effectData in _cardData.ruleEffects)
        {
            switch (effectData.EffectFunctionality.Type)
            {
                case EffectFunctionalityType.GrantFunction:
                case EffectFunctionalityType.GrantPower:
                    Action<bool> function1 = SetupEffectFunctionality(effectData.EffectFunctionality);
                    if (effectData.EffectCondition)
                        SetupEffectCondition(effectData.EffectCondition, function1);
                    else
                        Debug.LogError("GrantFunction or GrantPower is being processed without a condition");
                    break;

                default:
                    Action function0 = SetupEffectFunctionality(effectData.EffectFunctionality, effectData.MayUseFunction);
                    if (effectData.EffectCondition)
                        SetupEffectCondition(effectData.EffectCondition, function0);
                    else
                        _whenPlayed += function0;
                    break;
            }
        }
    }

    private Action SetupEffectFunctionality(EffectFunctionality functionality, bool mayUse)
    {
        switch (functionality.Type)
        {
            case EffectFunctionalityType.RegionMovement:
                return InvokeRegionMovementFunctionality;
        }

        return null;

        #region Local Functions

        void InvokeRegionMovementFunctionality()
        {
            ProcessRegionMovementFunctionality(this, functionality, mayUse);
        }

        #endregion
    }
    
    private Action<bool> SetupEffectFunctionality(EffectFunctionality functionality)
    {
        switch (functionality.Type)
        {
            case EffectFunctionalityType.GrantFunction:
                return InvokeGrantFunctionFunctionality;
        }

        return null;

        #region Local Functions

        void InvokeGrantFunctionFunctionality(bool activate)
        {
            ProcessGrantFunctionFunctionality(this, functionality.SubFunctionality, activate);
        }

        #endregion
    }

    private void SetupEffectCondition(EffectCondition condition, Action function)
    {
        switch (condition.Type)
        {
            case EffectConditionType.WhenPutIntoBattle:
                _whenPutIntoBattle += function;
                break;

            case EffectConditionType.WhenDestroyed:
                _whenDestroyed += function;
                break;

            case EffectConditionType.WhenWouldBeDestroyed:
                _whenWouldBeDestroyed += function;
                break;
        }
    }
    
    private void SetupEffectCondition(EffectCondition condition, Action<bool> function)
    {
        switch (condition.Type)
        {
            case EffectConditionType.WhileCondition:
                _whileConditions.Add(new WhileConditionHolder
                {
                    targetingParameter = condition.TargetingParameter,
                    targetingCondition = condition.TargetingCondition,
                    processFunction = function
                });


                break;
        }
    }

    #region Effect Trigger Methods

    public bool TriggerWhenPlayed()
    {
        if (_whenPlayed != null)
        {
            _whenPlayed.Invoke();
            return true;
        }

        return false;
    }

    public bool TriggerWhenPutIntoBattle()
    {
        if (_whenPutIntoBattle != null)
        {
            _whenPutIntoBattle.Invoke();
            return true;
        }

        return false;
    }

    public bool TriggerWhenDestroyed()
    {
        if (_whenDestroyed != null)
        {
            _whenDestroyed.Invoke();
            return true;
        }

        return false;
    }

    public bool TriggerWhenWouldBeDestroyed()
    {
        if (_whenWouldBeDestroyed != null)
        {
            _whenWouldBeDestroyed.Invoke();
            return true;
        }

        return false;
    }

    public void TriggerWhileConditions()
    {
        for (int i = 0, n = _whileConditions.Count; i < n; i++)
        {
            WhileConditionHolder whileCondition = _whileConditions[i];
            bool meetsCondition = ProcessTargeting(whileCondition.targetingParameter, whileCondition.targetingCondition);

            if (meetsCondition != whileCondition.effectActive)
            {
                whileCondition.effectActive = meetsCondition;
                whileCondition.processFunction.Invoke(meetsCondition);
            }

            _whileConditions[i] = whileCondition;
        }
    }

    #endregion
    
    #region Static Methods

    private static void ProcessRegionMovementFunctionality(CardInstanceEffectHandler instanceEffect, EffectFunctionality functionality, bool mayUse)
    {
        switch (functionality.TargetCard)
        {
            case CardTargetType.AutoTarget:
            case CardTargetType.NoTarget:
                Debug.LogError($"{instanceEffect._cardData.Name} has RegionMovement as functionality type with the wrong target type.");
                break;

            case CardTargetType.TargetSelf:
                CardEffectsManager.Instance.ProcessRegionMovement(instanceEffect._cardObj, functionality.MovementZones);
                break;

            case CardTargetType.TargetOther:
                CardEffectsManager.Instance.ProcessRegionMovement(functionality.ChoosingPlayer == PlayerTargetType.Player,
                    functionality.TargetPlayer == PlayerTargetType.Player,
                    functionality.MovementZones, functionality.TargetingCondition, mayUse);
                break;
        }
    }

    private static void ProcessGrantFunctionFunctionality(CardInstanceEffectHandler instanceEffect,
        EffectFunctionality functionality, bool activate)
    {
        if (activate)
            ActivateGrantFunctionFunctionality(instanceEffect, functionality);
        else
            DeactivateGrantFunctionFunctionality(instanceEffect, functionality);
    }

    private static void ActivateGrantFunctionFunctionality(CardInstanceEffectHandler instanceEffect, EffectFunctionality functionality)
    {
        switch (functionality.Type)
        {
            case EffectFunctionalityType.PowerAttacker:
                instanceEffect._isPowerAttacker = true;
                instanceEffect._powerBoost = functionality.PowerBoost;
                break;
        }
    }
    
    private static void DeactivateGrantFunctionFunctionality(CardInstanceEffectHandler instanceEffect, EffectFunctionality functionality)
    {
        switch (functionality.Type)
        {
            case EffectFunctionalityType.PowerAttacker:
                instanceEffect._isPowerAttacker = false;
                instanceEffect._powerBoost = 0;
                break;
        }
    }

    private static bool ProcessTargeting(EffectTargetingParameter targetingParameter,
        EffectTargetingCondition targetingCondition)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();
        switch (targetingParameter.OwningPlayer)
        {
            case PlayerTargetType.Player:
                cards = GameDataHandler.Instance.GetZoneCards(true, targetingParameter.ZoneType);
                break;

            case PlayerTargetType.Opponent:
                cards = GameDataHandler.Instance.GetZoneCards(false, targetingParameter.ZoneType);
                break;

            case PlayerTargetType.Both:
                cards = GameDataHandler.Instance.GetZoneCards(true, targetingParameter.ZoneType);
                cards.AddRange(GameDataHandler.Instance.GetZoneCards(false, targetingParameter.ZoneType));
                break;
        }

        int count = 0, targetCount = 0;
        if (targetingParameter.CountType == CountType.All)
            targetCount = cards.Count;
        else if (targetingParameter.CountType == CountType.Number)
            targetCount = targetingParameter.Count;

        foreach (CardBehaviour card in cards)
        {
            CardObject cardObj = card as CardObject;
            if (!cardObj)
                cardObj = ((ShieldObject) card).CardObj;

            if (CardData.IsTargetingConditionSatisfied(cardObj.CardInst, targetingCondition))
                count++;
        }

        if (targetingParameter.CountType == CountType.Number)
        {
            switch (targetingParameter.CountChoice)
            {
                case CountChoiceType.Exactly:
                    return count == targetCount;

                case CountChoiceType.AtLeast:
                    return count >= targetCount;

                case CountChoiceType.Upto:
                    return count <= targetCount;
            }
        }

        return count == targetCount;
    }

    #endregion
}
