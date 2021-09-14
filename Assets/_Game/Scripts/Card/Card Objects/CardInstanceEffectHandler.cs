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

        public Action function;
        public bool triggeredEffect;
    }

    #endregion

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
    
    public void SetupRuleEffects()
    {
        foreach (EffectData effectData in _cardData.ruleEffects)
        {
            Action function = SetupEffectFunctionality(effectData.EffectFunctionality, effectData.MayUseFunction);
            if (effectData.EffectCondition)
                SetupEffectCondition(effectData.EffectCondition, function);
            else
                _whenPlayed += function;
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
            ProcessRegionMovementFunctionality(functionality, mayUse);
        }

        #endregion
    }

    private void SetupEffectCondition(EffectCondition condition, Action function)
    {
        switch (condition.Type)
        {
            case EffectConditionType.WhileCondition:
                _whileConditions.Add(new WhileConditionHolder
                {
                    targetingParameter = condition.TargetingParameter,
                    targetingCondition = condition.TargetingCondition,
                    function = function
                });
                break;

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
    
    public void TriggerWhileCondition()
    {
        for (int i = 0, n = _whileConditions.Count; i < n; i++)
        {
            WhileConditionHolder whileCondition = _whileConditions[i];
            bool meetsCondition = false;



            if (meetsCondition && !whileCondition.triggeredEffect)
            {
                whileCondition.triggeredEffect = true;
                whileCondition.function.Invoke();
            }
        }
    }

    private void ProcessRegionMovementFunctionality(EffectFunctionality functionality, bool mayUse)
    {
        switch (functionality.TargetCard)
        {
            case CardTargetType.AutoTarget:
            case CardTargetType.NoTarget:
                Debug.LogError($"{_cardData.Name} has RegionMovement as functionality type with the wrong target type.");
                break;

            case CardTargetType.TargetSelf:
                CardEffectsManager.Instance.ProcessRegionMovement(_cardObj, functionality.MovementZones);
                break;

            case CardTargetType.TargetOther:
                CardEffectsManager.Instance.ProcessRegionMovement(functionality.ChoosingPlayer == PlayerTargetType.Player,
                    functionality.TargetPlayer == PlayerTargetType.Player,
                    functionality.MovementZones, functionality.TargetingCondition, mayUse);
                break;
        }
    }

    private bool ProcessTargeting(EffectTargetingParameter targetingParameter,
        EffectTargetingCondition targetingCondition)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();
        switch (targetingParameter.OwningPlayer)
        {
            case PlayerTargetType.Player:
                cards = GetZoneCards(true, targetingParameter.ZoneType);
                break;

            case PlayerTargetType.Opponent:
                cards = GetZoneCards(false, targetingParameter.ZoneType);
                break;

            case PlayerTargetType.Both:
                cards = GetZoneCards(true, targetingParameter.ZoneType);
                cards.AddRange(GetZoneCards(false, targetingParameter.ZoneType));
                break;
        }

        int count = 0, targetCount = 0;
        if (targetingParameter.CountType == CountType.All)
        {
            targetCount = cards.Count;
        }
        else if (targetingParameter.CountType == CountType.Number)
        {
            targetCount = targetingParameter.Count;
        }

        foreach (CardBehaviour card in cards)
        {
            CardObject cardObj = card as CardObject;
            if (!cardObj)
                cardObj = ((ShieldObject) card).CardObj;

            if (CardData.IsTargetingConditionSatisfied(_cardObj.CardInst, targetingCondition))
                count++;
        }

        return count == targetCount;
    }

    private List<CardBehaviour> GetZoneCards(bool isPlayer, CardZoneType zoneType)
    {
        PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(isPlayer);
        switch (zoneType)
        {
            case CardZoneType.Deck:
                return new List<CardBehaviour>(dataHandler.CardsInDeck);
            case CardZoneType.Hand:
                 return new List<CardBehaviour>(dataHandler.CardsInHandList);
            case CardZoneType.Shields:
                 return new List<CardBehaviour>(dataHandler.Shields);
            case CardZoneType.Graveyard:
                 return new List<CardBehaviour>(dataHandler.CardsInGrave);
            case CardZoneType.ManaZone:
                 return new List<CardBehaviour>(dataHandler.CardsInManaList);
            case CardZoneType.BattleZone:
                return new List<CardBehaviour>(dataHandler.CardsInBattleList);
        }

        return null;
    }
}
