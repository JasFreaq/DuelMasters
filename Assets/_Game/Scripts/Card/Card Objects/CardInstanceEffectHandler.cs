using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardInstanceEffectHandler
{
    #region Helper Data Structures
    struct BoolActionHolder
    {
        public Action<bool> processFunction;
        public bool effectActive;
    }

    struct WhileConditionHolder
    {
        public EffectTargetingParameter targetingParameter;
        public EffectTargetingCondition targetingCondition;

        public BoolActionHolder boolAction;
    }
    
    #endregion

    #region Data Members

    private CardObject _cardObj;
    private CardData _cardData;

    private readonly bool _isBlocker;

    private bool _isMultipleBreaker;
    private MultipleBreakerType _multipleBreakerType;

    private bool _isPowerAttacker, _multiplyPowerAttackValue;
    private int _powerAttackBoost;
    private EffectTargetingParameter _multiplyPowerAttackParameter;
    private EffectTargetingCondition _multiplyPowerAttackCondition;
    
    private bool _multiplyGrantedPowerValue;
    private int _grantedPowerBoost = 0;
    private EffectTargetingParameter _multiplyGrantedPowerParameter;
    private EffectTargetingCondition _multiplyGrantedPowerCondition;

    private Action _whenPlayed;
    private Action _whenPutIntoBattle;
    private Action _whenDestroyed;
    private Action _whenWouldBeDestroyed;

    private BoolActionHolder _whenAttacking = new BoolActionHolder();
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
                    _multiplyPowerAttackValue = functionality.ShouldMultiplyVal;
                    _powerAttackBoost = functionality.PowerBoost;
                    if (_multiplyPowerAttackValue)
                    {
                        _multiplyPowerAttackParameter = functionality.TargetingParameter;
                        _multiplyPowerAttackCondition = functionality.TargetingCondition;
                    }
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
    
    public int PowerAttackBoost
    {
        get
        {
            if (_multiplyPowerAttackValue)
                return _powerAttackBoost *
                       CardData.GetNumValidCards(_multiplyPowerAttackParameter, _multiplyPowerAttackCondition);

            return _powerAttackBoost;
        }
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
            if (effectData.EffectCondition && effectData.EffectCondition.Type == EffectConditionType.WhileCondition)
            {
                switch (effectData.EffectFunctionality.Type)
                {
                    case EffectFunctionalityType.GrantFunction:
                    case EffectFunctionalityType.GrantPower:
                        Action<bool> function = SetupEffectFunctionality(effectData.EffectFunctionality);
                        SetupEffectCondition(effectData.EffectCondition, function);
                        break;
                }
            }
            else
            {
                Action function = SetupEffectFunctionality(effectData.EffectFunctionality, effectData.MayUseFunction);
                if (effectData.EffectCondition)
                    SetupEffectCondition(effectData.EffectCondition, function);
                else
                    _whenPlayed += function;
            }
        }
    }

    private Action SetupEffectFunctionality(EffectFunctionality functionality, bool mayUse)
    {
        switch (functionality.Type)
        {
            case EffectFunctionalityType.RegionMovement:
                return () => { ProcessRegionMovementFunctionality(this, functionality, mayUse); };

            case EffectFunctionalityType.Destroy:
                return () => { ProcessDestroyFunctionality(this, functionality, mayUse); };

            case EffectFunctionalityType.GrantFunction:
                return () => { ProcessGrantFunctionFunctionality(this, functionality, true,
                        functionality.AlterFunctionUntilEndOfTurn); };
        }

        return null;
    }
    
    private Action<bool> SetupEffectFunctionality(EffectFunctionality functionality)
    {
        switch (functionality.Type)
        {
            case EffectFunctionalityType.GrantFunction:
                return activate => { ProcessGrantFunctionFunctionality(this, functionality, activate,
                        functionality.AlterFunctionUntilEndOfTurn);
                };

            case EffectFunctionalityType.GrantPower:
                return activate => { ProcessGrantPowerFunctionality(this, functionality, activate,
                        functionality.AlterFunctionUntilEndOfTurn);
                };
        }

        return null;
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
                    boolAction = new BoolActionHolder { processFunction = function }
                });
                break;

            case EffectConditionType.WhenAttacking:
                _whenAttacking.processFunction += function;
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

    public bool TriggerWhenAttacking(bool activate)
    {
        if (_whenAttacking.processFunction != null)
        {
            _whenAttacking.processFunction.Invoke(activate);
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

            if (meetsCondition != whileCondition.boolAction.effectActive)
            {
                whileCondition.boolAction.effectActive = meetsCondition;
                whileCondition.boolAction.processFunction.Invoke(meetsCondition);
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
                
            case CardTargetType.TargetOther:
                CardEffectsManager.Instance.ProcessRegionMovement(functionality, mayUse);
                break;
            
            case CardTargetType.TargetSelf:
                CardEffectsManager.Instance.ProcessRegionMovement(instanceEffect._cardObj,
                    functionality.MovementZones.fromZone, functionality.MovementZones.toZone);
                break;
        }
    }
    
    private static void ProcessDestroyFunctionality(CardInstanceEffectHandler instanceEffect, EffectFunctionality functionality, bool mayUse)
    {
        switch (functionality.TargetCard)
        {
            case CardTargetType.AutoTarget:
            case CardTargetType.NoTarget:
                Debug.LogError($"{instanceEffect._cardData.Name} has Destroy as functionality type with the wrong target type.");
                break;
                
            case CardTargetType.TargetOther:
                instanceEffect._cardObj.StartCoroutine(ProcessCardSelectionRoutine(functionality, instance =>
                {
                    instance._cardObj.SendToGraveyard();
                }));
                break;
            
            case CardTargetType.TargetSelf:
                instanceEffect._cardObj.SendToGraveyard();
                break;
        }
    }

    private static void ProcessGrantFunctionFunctionality(CardInstanceEffectHandler instanceEffect,
        EffectFunctionality functionality, bool activate, bool untilEndOfTurn)
    {
        switch (functionality.TargetCard)
        {
            case CardTargetType.AutoTarget:
                List<CardBehaviour> cards = GameDataHandler.Instance.GetZoneCards(functionality.TargetPlayer == PlayerTargetType.Player, functionality.TargetingParameter.ZoneType);
                foreach (CardBehaviour card in cards)
                {
                    CardObject cardObj = card as CardObject;
                    if (!cardObj)
                        cardObj = ((ShieldObject)card).CardObj;

                    ProcessPowerGrant(cardObj.CardInst.InstanceEffectHandler);
                }
                break;

            case CardTargetType.TargetOther:
                instanceEffect._cardObj.StartCoroutine(ProcessCardSelectionRoutine(functionality, ProcessPowerGrant));
                break;

            case CardTargetType.TargetSelf:
                ProcessPowerGrant(instanceEffect);
                break;
        }

        #region Local Functions

        void ProcessPowerGrant(CardInstanceEffectHandler instance)
        {
            if (activate)
            {
                ActivateEffectFunctionality(instance, functionality.SubFunctionality);
                if (untilEndOfTurn)
                    CardEffectsManager.Instance.RegisterOnEndOfTurn(() =>
                    {
                        DeactivateEffectFunctionality(instanceEffect, functionality.SubFunctionality);
                    });
            }
            else
                DeactivateEffectFunctionality(instance, functionality.SubFunctionality);
        }

        #endregion
    }
    
    private static void ProcessDisableFunctionFunctionality(CardInstanceEffectHandler instanceEffect,
        EffectFunctionality functionality, bool deactivate, bool untilEndOfTurn)
    {
        switch (functionality.TargetCard)
        {
            case CardTargetType.AutoTarget:
                List<CardBehaviour> cards = GameDataHandler.Instance.GetZoneCards(functionality.TargetPlayer == PlayerTargetType.Player, functionality.TargetingParameter.ZoneType);
                foreach (CardBehaviour card in cards)
                {
                    CardObject cardObj = card as CardObject;
                    if (!cardObj)
                        cardObj = ((ShieldObject) card).CardObj;

                    ProcessFunctionDisable(cardObj.CardInst.InstanceEffectHandler);
                }
                break;

            case CardTargetType.TargetOther:
                instanceEffect._cardObj.StartCoroutine(ProcessCardSelectionRoutine(functionality, ProcessFunctionDisable));
                break;

            case CardTargetType.TargetSelf:
                ProcessFunctionDisable(instanceEffect);
                break;
        }

        #region Local Functions

        void ProcessFunctionDisable(CardInstanceEffectHandler instance)
        {
            if (deactivate)
            {
                DeactivateEffectFunctionality(instanceEffect, functionality.SubFunctionality);
                if (untilEndOfTurn)
                    CardEffectsManager.Instance.RegisterOnEndOfTurn(() =>
                    {
                        ActivateEffectFunctionality(instanceEffect, functionality.SubFunctionality);
                    });
            }
            else
                ActivateEffectFunctionality(instanceEffect, functionality.SubFunctionality);
        }

        #endregion
    }

    private static void ProcessGrantPowerFunctionality(CardInstanceEffectHandler instanceEffect,
        EffectFunctionality functionality, bool activate, bool untilEndOfTurn)
    {
        switch (functionality.TargetCard)
        {
            case CardTargetType.AutoTarget:
                List<CardBehaviour> cards = GameDataHandler.Instance.GetZoneCards(functionality.TargetPlayer == PlayerTargetType.Player, functionality.TargetingParameter.ZoneType);
                foreach (CardBehaviour card in cards)
                {
                    CardObject cardObj = card as CardObject;
                    if (!cardObj)
                        cardObj = ((ShieldObject)card).CardObj;

                    ProcessPowerGrant(cardObj.CardInst.InstanceEffectHandler);
                }
                break;

            case CardTargetType.TargetOther:
                instanceEffect._cardObj.StartCoroutine(ProcessCardSelectionRoutine(functionality, ProcessPowerGrant));
                break;

            case CardTargetType.TargetSelf:
                ProcessPowerGrant(instanceEffect);
                break;
        }

        #region Local Functions

        void ProcessPowerGrant(CardInstanceEffectHandler instance)
        {
            if (activate)
            {
                instance._grantedPowerBoost += functionality.PowerBoost;
                if (instance._multiplyGrantedPowerValue)
                    instance._grantedPowerBoost *= CardData.GetNumValidCards(instance._multiplyGrantedPowerParameter,
                        instance._multiplyGrantedPowerCondition);
                //((CreatureObject)instance._cardObj).card

                if (untilEndOfTurn)
                    CardEffectsManager.Instance.RegisterOnEndOfTurn(() =>
                    {
                        DeactivatePowerGrant(instance);
                    });
            }
            else
                DeactivatePowerGrant(instance);
        }

        void DeactivatePowerGrant(CardInstanceEffectHandler instance)
        {
            if (instance._multiplyGrantedPowerValue)
                instance._grantedPowerBoost -= instance._grantedPowerBoost * CardData.GetNumValidCards(instance._multiplyGrantedPowerParameter,
                    instance._multiplyGrantedPowerCondition);
            else
                instance._grantedPowerBoost -= functionality.PowerBoost;
        }

        #endregion
    }

    private static void ActivateEffectFunctionality(CardInstanceEffectHandler instanceEffect, EffectFunctionality functionality)
    {
        switch (functionality.Type)
        {
            case EffectFunctionalityType.PowerAttacker:
                instanceEffect._isPowerAttacker = true;
                instanceEffect._powerAttackBoost = functionality.PowerBoost;
                break;
        }
    }
    
    private static void DeactivateEffectFunctionality(CardInstanceEffectHandler instanceEffect, EffectFunctionality functionality)
    {
        switch (functionality.Type)
        {
            case EffectFunctionalityType.PowerAttacker:
                instanceEffect._isPowerAttacker = false;
                instanceEffect._powerAttackBoost = 0;
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
                cards = GameDataHandler.Instance.GetZoneCards(true, targetingParameter.ZoneType, true);
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

    private static IEnumerator ProcessCardSelectionRoutine(EffectFunctionality functionality, Action<CardInstanceEffectHandler> callback)
    {
        Coroutine<List<CardBehaviour>> routine =
            CardEffectsManager.Instance.StartCoroutine<List<CardBehaviour>>(CardEffectsManager.Instance.ProcessCardSelectionRoutine(
                functionality.ChoosingPlayer == PlayerTargetType.Player,
                functionality.TargetPlayer == PlayerTargetType.Player,
                new CardEffectsManager.CardSelectionData(functionality.DestroyParam, functionality.TargetPlayer), 
                functionality.TargetingCondition, false));
        yield return routine.coroutine;
        List<CardBehaviour> cards = routine.returnVal;

        foreach (CardBehaviour card in cards)
        {
            CardObject cardObj = card as CardObject;
            if (!cardObj)
                cardObj = ((ShieldObject)card).CardObj;

            callback.Invoke(cardObj.CardInst.InstanceEffectHandler);
        }
    }

    #endregion
}
