using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance
{
    private CardObject _cardObj;
    private CardData _cardData;
    private CardZoneType _currentZone = CardZoneType.Deck;

    private bool _isTapped;
    private readonly bool _isBlocker;
    
    #region Effect Data Members

    private bool _isMultipleBreaker;
    private MultipleBreakerType _multipleBreakerType;

    private bool _isPowerAttacker;
    private int _powerBoost;

    private Action _whenPlayed;
    private Action _whenPutIntoBattle;
    private Action _whenDestroyed;
    private Action _whenWouldBeDestroyed;

    #endregion
    
    public CardInstance(CardData cardData)
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

    public CardData CardData
    {
        get { return _cardData; }
    }

    public CardZoneType CurrentZone
    {
        get { return _currentZone;}
    }

    public bool IsTapped
    {
        get { return _isTapped; }
    }
    
    public bool IsBlocker
    {
        get { return _isBlocker; }
    }

    #region Effect Data Properties

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

    #endregion

    public void SetCurrentZone(CardZoneType currentZone)
    {
        _currentZone = currentZone;
    }

    public void ToggleTapState()
    {
        _isTapped = !_isTapped;
    }

    public bool CanAttack()
    {
        return !_isTapped;
    }

    #region Effect Methods/Callbacks

    public void SetupRuleEffects()
    {
        foreach (EffectData effectData in _cardData.ruleEffects)
        {
            Action function = SetupEffectFunctionality(effectData.EffectFunctionality, effectData.MayUseFunction);
            if (effectData.EffectCondition)
                SetupEffectCondition(effectData.EffectCondition.Type, function);
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

    private void SetupEffectCondition(EffectConditionType conditionType, Action function)
    {
        switch (conditionType)
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
                CardEffectsManager.Instance.ProcessRegionMovement(functionality.ChoosingPlayer == PlayerTargetType.TargetPlayer,
                    functionality.TargetPlayer == PlayerTargetType.TargetPlayer, 
                    functionality.MovementZones, functionality.TargetingCondition, mayUse);
                break;
        }
    }

    #endregion
}
