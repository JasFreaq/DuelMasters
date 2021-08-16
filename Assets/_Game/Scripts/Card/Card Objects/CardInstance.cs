using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstance
{
    private CardObject _cardObj;
    private CardData _cardData;
    private CardZoneType _currentZone = CardZoneType.Deck;

    private bool _isTapped = false;

    #region Effect Data Members

    private Action _whenPlayed;
    private Action _whenPutIntoBattle;
    private Action _whenDestroyed;
    private Action _whenWouldBeDestroyed;

    #endregion

    public CardInstance(CardData cardData)
    {
        _cardData = cardData;
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
        get
        {
            return CardData.IsTargetingConditionSatisfied(this, CardData.BlockerCondition);
        }
    }

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
                GameManager.Instance.ProcessRegionMovement(_cardObj, functionality.MovementZones);
                break;

            case CardTargetType.TargetOther:
                GameManager.Instance.ProcessRegionMovement(functionality.ChoosingPlayer == PlayerTargetType.TargetPlayer,
                    functionality.TargetPlayer == PlayerTargetType.TargetPlayer, 
                    functionality.MovementZones, functionality.TargetingCondition, mayUse);
                break;
        }
    }

    #endregion
}
