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
        public EffectTargetingData TargetingData;
        public EffectTargetingCondition targetingCondition;

        public BoolActionHolder boolAction;
    }
    
    #endregion

    #region Data Members

    private CardInstance _cardInst;

    private readonly bool _hasShieldTrigger;
    private readonly bool _isBlocker;
    
    private bool _isSlayer;
    private bool _attacksEachTurnIfAble;
    private bool _attackAsThoughTapped;

    private bool _isMultipleBreaker;
    private MultipleBreakerType _multipleBreakerType;

    private bool _canAttackUntapped, _cantAttack, _cantAttackCreatures, _cantAttackPlayer;
    private bool _cantBeAttacked, _cantBeBlocked;

    private bool _isPowerAttacker, _multiplyPowerAttackValue;
    private int _powerAttackBoost;
    private EffectTargetingData _multiplyPowerAttackData;
    private EffectTargetingCondition _multiplyPowerAttackCondition;
    
    private bool _multiplyGrantedPowerValue;
    private EffectTargetingData _multiplyGrantedPowerData;
    private EffectTargetingCondition _multiplyGrantedPowerCondition;

    private Action _whenPlayed;
    private Action _whenPutIntoBattle;
    private Action _afterBattle;
    private Action _whenDestroyed;
    private Action _whenWouldBeDestroyed;

    private BoolActionHolder _whenAttacking = new BoolActionHolder();
    private List<WhileConditionHolder> _whileConditions = new List<WhileConditionHolder>();
    
    #endregion

    public CardInstanceEffectHandler(CardInstance cardInst)
    {
        _cardInst = cardInst;
        foreach (EffectData effectData in _cardInst.CardData.ruleEffects)
        {
            EffectFunctionality functionality = effectData.EffectFunctionality;
            switch (functionality.Type)
            {
                case EffectFunctionalityType.GrantFunction:
                case EffectFunctionalityType.DisableFunction:
                    break;

                case EffectFunctionalityType.RegionMovement:
                    break;
                    
                case EffectFunctionalityType.Keyword:
                    switch (functionality.Keyword)
                    {
                        case KeywordType.ShieldTrigger:
                            _hasShieldTrigger = true;
                            break;

                        case KeywordType.Blocker:
                            _isBlocker = true;
                            break;

                        case KeywordType.Slayer:
                            _isSlayer = true;
                            break;

                        case KeywordType.AttacksEachTurnIfAble:
                            _attacksEachTurnIfAble = true;
                            break;

                        case KeywordType.AttackThisTurnAsThoughTapped:
                            _attackAsThoughTapped = true;
                            break;
                    }
                    break;

                case EffectFunctionalityType.MultipleBreaker:
                    _isMultipleBreaker = true;
                    _multipleBreakerType = functionality.MultipleBreaker;
                    break;

                case EffectFunctionalityType.AttackTarget:
                    switch (functionality.AttackType)
                    {
                        case AttackType.CanAttackUntapped:
                            _canAttackUntapped = true;
                            break;

                        case AttackType.CantAttack:
                            _cantAttack = true;
                            break;

                        case AttackType.CantAttackCreatures:
                            _cantAttackCreatures = true;
                            break;

                        case AttackType.CantAttackPlayer:
                            _cantAttackPlayer = true;
                            break;
                    }
                    break;

                case EffectFunctionalityType.TargetBehaviour:
                    switch (functionality.TargetBehaviour)
                    {
                        case TargetBehaviourType.CantBeAttacked:
                            _cantBeAttacked = true;
                            break;

                        case TargetBehaviourType.CantBeBlocked:
                            _cantBeBlocked = true;
                            break;
                    }
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
                        _multiplyPowerAttackData = functionality.TargetingData;
                        _multiplyPowerAttackCondition = functionality.TargetingCondition;
                    }
                    break;
                
                case EffectFunctionalityType.GrantPower:
                    _multiplyGrantedPowerValue = functionality.ShouldMultiplyVal;
                    if (_multiplyGrantedPowerValue)
                    {
                        _multiplyGrantedPowerData = functionality.TargetingData;
                        _multiplyGrantedPowerCondition = functionality.TargetingCondition;
                    }
                    break;

                case EffectFunctionalityType.CostAdjustment:
                    break;
            }
        }
    }

    #region Properties
    
    public bool HasShieldTrigger
    {
        get { return _hasShieldTrigger; }
    }
    
    public bool IsBlocker
    {
        get { return _isBlocker; }
    }

    public bool IsSlayer
    {
        get { return _isSlayer; }
    }

    public bool IsMultipleBreaker
    {
        get { return _isMultipleBreaker; }
    }
    
    public bool CanAttackUntapped
    {
        get { return _canAttackUntapped; }
    }
    
    public bool CantAttack
    {
        get { return _cantAttack; }
    }
    
    public bool CantAttackCreatures
    {
        get { return _cantAttackCreatures; }
    }

    public bool CantAttackPlayer
    {
        get { return _cantAttackPlayer; }
    }
    
    public bool CantBeAttacked
    {
        get { return _cantBeAttacked; }
    }
    
    public bool CantBeBlocked
    {
        get { return _cantBeBlocked; }
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
                       CardData.GetNumValidCards(_multiplyPowerAttackData, _multiplyPowerAttackCondition);

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
        foreach (EffectData effectData in _cardInst.CardData.ruleEffects)
        {
            switch (effectData.EffectFunctionality.Type)
            {
                case EffectFunctionalityType.GrantFunction:
                case EffectFunctionalityType.GrantPower:
                    Action<bool> function1 = SetupEffectFunctionality(effectData.EffectFunctionality);
                    SetupEffectCondition(effectData.EffectCondition, function1);
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

            case EffectConditionType.AfterBattle:
                _afterBattle += function;
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
                    TargetingData = condition.TargetingData,
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

    public bool TriggerAfterBattle()
    {
        if (_afterBattle != null)
        {
            _afterBattle.Invoke();
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
            bool meetsCondition = ProcessTargeting(whileCondition.TargetingData, whileCondition.targetingCondition);

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
                Debug.LogError($"{instanceEffect._cardInst.CardData.Name} has RegionMovement as functionality type with the wrong target type.");
                break;
                
            case CardTargetType.TargetOther:
                CardEffectsManager.Instance.ProcessRegionMovement(functionality, mayUse);
                break;
            
            case CardTargetType.TargetSelf:
                CardEffectsManager.Instance.ProcessRegionMovement(instanceEffect._cardInst.CardObj,
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
                Debug.LogError($"{instanceEffect._cardInst.CardData.Name} has Destroy as functionality type with the wrong target type.");
                break;
                
            case CardTargetType.TargetOther:
                instanceEffect._cardInst.CardObj.StartCoroutine(ProcessCardSelectionRoutine(functionality, 
                    instance => { instance._cardInst.CardObj.SendToGraveyard(); }));
                break;
            
            case CardTargetType.TargetSelf:
                instanceEffect._cardInst.CardObj.SendToGraveyard();
                break;
        }
    }

    private static void ProcessGrantFunctionFunctionality(CardInstanceEffectHandler instanceEffect,
        EffectFunctionality functionality, bool activate, bool untilEndOfTurn)
    {
        switch (functionality.TargetCard)
        {
            case CardTargetType.AutoTarget:
                List<CardBehaviour> cards = GameDataHandler.Instance.GetZoneCards(functionality.TargetingData.OwningPlayer == PlayerTargetType.Player, 
                    functionality.TargetingData.ZoneType);
                foreach (CardBehaviour card in cards)
                {
                    CardObject cardObj = card as CardObject;
                    if (!cardObj)
                        cardObj = ((ShieldObject)card).CardObj;

                    ProcessFunctionGrant(cardObj.CardInst.InstanceEffectHandler);
                }
                break;

            case CardTargetType.TargetOther:
                instanceEffect._cardInst.CardObj.StartCoroutine(ProcessCardSelectionRoutine(functionality, ProcessFunctionGrant));
                break;

            case CardTargetType.TargetSelf:
                ProcessFunctionGrant(instanceEffect);
                break;
        }

        #region Local Functions

        void ProcessFunctionGrant(CardInstanceEffectHandler instance)
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
                List<CardBehaviour> cards = GameDataHandler.Instance.GetZoneCards(functionality.TargetingData.OwningPlayer == PlayerTargetType.Player, 
                    functionality.TargetingData.ZoneType);
                foreach (CardBehaviour card in cards)
                {
                    CardObject cardObj = card as CardObject;
                    if (!cardObj)
                        cardObj = ((ShieldObject) card).CardObj;

                    ProcessFunctionDisable(cardObj.CardInst.InstanceEffectHandler);
                }
                break;

            case CardTargetType.TargetOther:
                instanceEffect._cardInst.CardObj.StartCoroutine(ProcessCardSelectionRoutine(functionality, ProcessFunctionDisable));
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
        int grantedPowerBoost = functionality.PowerBoost;

        switch (functionality.TargetCard)
        {
            case CardTargetType.AutoTarget:
                List<CardBehaviour> cards = GameDataHandler.Instance.GetZoneCards(functionality.TargetingData.OwningPlayer == PlayerTargetType.Player, 
                    functionality.TargetingData.ZoneType);
                foreach (CardBehaviour card in cards)
                {
                    CardObject cardObj = card as CardObject;
                    if (!cardObj)
                        cardObj = ((ShieldObject) card).CardObj;

                    ProcessPowerGrant(cardObj.CardInst.InstanceEffectHandler);
                }
                break;

            case CardTargetType.TargetOther:
                instanceEffect._cardInst.CardObj.StartCoroutine(ProcessCardSelectionRoutine(functionality, ProcessPowerGrant));
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
                if (instance._multiplyGrantedPowerValue)
                {
                    int num = CardData.GetNumValidCards(instance._multiplyGrantedPowerData,
                        instance._multiplyGrantedPowerCondition);
                    grantedPowerBoost *= num;
                }

                CreatureObject creatureObj = (CreatureObject) instance._cardInst.CardObj;
                creatureObj.CardInst.GrantedPower += grantedPowerBoost;
                creatureObj.UpdatePower(creatureObj.CardInst.Power);

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
            CreatureObject creatureObj = (CreatureObject) instance._cardInst.CardObj;
            creatureObj.CardInst.GrantedPower -= grantedPowerBoost;
            creatureObj.ResetPower(creatureObj.CardInst.Power);
        }

        #endregion
    }

    private static void ActivateEffectFunctionality(CardInstanceEffectHandler instanceEffect, EffectFunctionality functionality)
    {
        switch (functionality.Type)
        {
            case EffectFunctionalityType.Keyword:
                switch (functionality.Keyword)
                {
                    case KeywordType.Slayer:
                        instanceEffect._isSlayer = true;
                        break;

                    case KeywordType.AttacksEachTurnIfAble:
                        instanceEffect._attacksEachTurnIfAble = true;
                        break;

                    case KeywordType.AttackThisTurnAsThoughTapped:
                        instanceEffect._attackAsThoughTapped = true;
                        break;
                }
                break;

            case EffectFunctionalityType.MultipleBreaker:
                instanceEffect._isMultipleBreaker = true;
                instanceEffect._multipleBreakerType = functionality.MultipleBreaker;
                break;

            case EffectFunctionalityType.AttackTarget:
                switch (functionality.AttackType)
                {
                    case AttackType.CanAttackUntapped:
                        instanceEffect._canAttackUntapped = true;
                        break;

                    case AttackType.CantAttack:
                        instanceEffect._cantAttack = true;
                        break;

                    case AttackType.CantAttackCreatures:
                        instanceEffect._cantAttackCreatures = true;
                        break;

                    case AttackType.CantAttackPlayer:
                        instanceEffect._cantAttackPlayer = true;
                        break;
                }
                break;

            case EffectFunctionalityType.TargetBehaviour:
                switch (functionality.TargetBehaviour)
                {
                    case TargetBehaviourType.CantBeAttacked:
                        instanceEffect._cantBeAttacked = true;
                        break;

                    case TargetBehaviourType.CantBeBlocked:
                        instanceEffect._cantBeBlocked = true;
                        break;
                }
                break;

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
            case EffectFunctionalityType.Keyword:
                switch (functionality.Keyword)
                {
                    case KeywordType.Slayer:
                        instanceEffect._isSlayer = false;
                        break;

                    case KeywordType.AttacksEachTurnIfAble:
                        instanceEffect._attacksEachTurnIfAble = false;
                        break;

                    case KeywordType.AttackThisTurnAsThoughTapped:
                        instanceEffect._attackAsThoughTapped = false;
                        break;
                }
                break;

            case EffectFunctionalityType.MultipleBreaker:
                instanceEffect._isMultipleBreaker = false;
                break;

            case EffectFunctionalityType.AttackTarget:
                switch (functionality.AttackType)
                {
                    case AttackType.CanAttackUntapped:
                        instanceEffect._canAttackUntapped = false;
                        break;

                    case AttackType.CantAttack:
                        instanceEffect._cantAttack = false;
                        break;

                    case AttackType.CantAttackCreatures:
                        instanceEffect._cantAttackCreatures = false;
                        break;

                    case AttackType.CantAttackPlayer:
                        instanceEffect._cantAttackPlayer = false;
                        break;
                }
                break;

            case EffectFunctionalityType.TargetBehaviour:
                switch (functionality.TargetBehaviour)
                {
                    case TargetBehaviourType.CantBeAttacked:
                        instanceEffect._cantBeAttacked = false;
                        break;

                    case TargetBehaviourType.CantBeBlocked:
                        instanceEffect._cantBeBlocked = false;
                        break;
                }
                break;

            case EffectFunctionalityType.PowerAttacker:
                instanceEffect._isPowerAttacker = false;
                instanceEffect._powerAttackBoost = 0;
                break;
        }
    }

    private static bool ProcessTargeting(EffectTargetingData targetingData,
        EffectTargetingCondition targetingCondition)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();
        switch (targetingData.OwningPlayer)
        {
            case PlayerTargetType.Player:
                cards = GameDataHandler.Instance.GetZoneCards(true, targetingData.ZoneType);
                break;

            case PlayerTargetType.Opponent:
                cards = GameDataHandler.Instance.GetZoneCards(false, targetingData.ZoneType);
                break;

            case PlayerTargetType.Both:
                cards = GameDataHandler.Instance.GetZoneCards(true, targetingData.ZoneType, true);
                break;
        }

        int count = 0, targetCount = 0;
        if (targetingData.CountRangeType == CountRangeType.All)
            targetCount = cards.Count;
        else if (targetingData.CountRangeType == CountRangeType.Number)
            targetCount = targetingData.Count;

        foreach (CardBehaviour card in cards)
        {
            CardObject cardObj = card as CardObject;
            if (!cardObj)
                cardObj = ((ShieldObject) card).CardObj;

            if (CardData.IsTargetingConditionSatisfied(cardObj.CardInst, targetingCondition))
                count++;
        }

        if (targetingData.CountRangeType == CountRangeType.Number)
        {
            switch (targetingData.CountQuantifier)
            {
                case CountQuantifierType.Exactly:
                    return count == targetCount;

                case CountQuantifierType.AtLeast:
                    return count >= targetCount;

                case CountQuantifierType.Upto:
                    return count <= targetCount;
            }
        }

        return count == targetCount;
    }

    private static IEnumerator ProcessCardSelectionRoutine(EffectFunctionality functionality, Action<CardInstanceEffectHandler> callback)
    {
        PlayerManager choosingPlayer = GameManager.Instance.GetManager(functionality.ChoosingPlayer == PlayerTargetType.Player);
        choosingPlayer.IsSelecting = true;

        Coroutine<List<CardBehaviour>> routine =
            CardEffectsManager.Instance.StartCoroutine<List<CardBehaviour>>(CardEffectsManager.Instance.ProcessCardSelectionRoutine(
                functionality.ChoosingPlayer == PlayerTargetType.Player,
                functionality.TargetPlayer == PlayerTargetType.Player,
                new CardEffectsManager.CardSelectionData(functionality.DestroyParam, functionality.TargetPlayer), 
                functionality.TargetingCondition, false));
        yield return routine.coroutine;
        List<CardBehaviour> cards = routine.returnVal;

        choosingPlayer.IsSelecting = false;
        while (!choosingPlayer.FinishedCasting)
            yield return new WaitForEndOfFrame();

        foreach (CardBehaviour card in cards)
        {
            CardObject cardObj = card as CardObject;
            if (!cardObj)
                cardObj = ((ShieldObject)card).CardObj;

            callback.Invoke(cardObj.CardInst.InstanceEffectHandler);
        }
    }
    
    //private static IEnumerator ProcessCardSelectionRoutine(EffectTargetingParameter targetingParameter, Action<CardInstanceEffectHandler> callback)
    //{
    //    bool playerChooses = true;
    //    bool affectPlayer = targetingParameter.OwningPlayer == PlayerTargetType.Player;
    //    if (!affectPlayer && targetingParameter.OpponentChooses)
    //        playerChooses = false;

    //    PlayerManager choosingPlayer = GameManager.Instance.GetManager(playerChooses);
    //    choosingPlayer.IsSelecting = true;

    //    Coroutine<List<CardBehaviour>> routine =
    //        CardEffectsManager.Instance.StartCoroutine<List<CardBehaviour>>(CardEffectsManager.Instance.ProcessCardSelectionRoutine(
    //            playerChooses, affectPlayer,
    //            new CardEffectsManager.CardSelectionData(functionality.DestroyParam, functionality.TargetPlayer), 
    //            functionality.TargetingCondition, false));
    //    yield return routine.coroutine;
    //    List<CardBehaviour> cards = routine.returnVal;

    //    choosingPlayer.IsSelecting = false;
    //    while (!choosingPlayer.FinishedCasting)
    //        yield return new WaitForEndOfFrame();

    //    foreach (CardBehaviour card in cards)
    //    {
    //        CardObject cardObj = card as CardObject;
    //        if (!cardObj)
    //            cardObj = ((ShieldObject)card).CardObj;

    //        callback.Invoke(cardObj.CardInst.InstanceEffectHandler);
    //    }
    //}

    #endregion
}
