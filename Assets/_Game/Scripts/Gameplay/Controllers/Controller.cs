using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected CardObject _currentlySelected;
    protected CardObject _targetedCard;
    protected ShieldObject _targetedShield;
    protected bool _isPlayer;
    
    protected List<CardBehaviour> _selectionRange;
    protected List<CardObject> _cardSelections = new List<CardObject>();
    protected List<ShieldObject> _shieldSelections = new List<ShieldObject>();
    protected int _selectionLowerBound, _selectionUpperBound;
    protected bool _selectMultiple, _selectCard = true;
    private bool _reachedSelectionRange, _selectionMade;
    
    private bool _choosingEffectActivation, _activateEffect;

    public CardObject CurrentlySelected
    {
        get { return _currentlySelected; }
    }

    public bool IsPlayer
    {
        get { return _isPlayer; }
    }
    
    public virtual void SelectCard(CardObject cardObj)
    {
        if (_currentlySelected != cardObj)
        {
            if (_currentlySelected) 
                DeselectCurrentlySelected();
            _currentlySelected = cardObj;

            switch (GameManager.Instance.CurrentStep)
            {
                case GameStepType.AttackStep:
                    if (!_currentlySelected.CardInst.CanAttack())
                        _currentlySelected = null;

                    break;
            }
        }
        else if (_currentlySelected)
        {
            DeselectCurrentlySelected();
        }
    }
    
    public virtual void DeselectCurrentlySelected()
    {
        _currentlySelected = null;
    }
    
    protected virtual void ProcessInput(int iD)
    {
        if (_selectMultiple)
        {
            if (_targetedCard && _selectCard)
            {
                foreach (CardBehaviour card in _selectionRange)
                {
                    if (_targetedCard == card && _targetedCard.IsValid)
                    {
                        if (!_cardSelections.Contains(_targetedCard) && _cardSelections.Count < _selectionUpperBound)
                        {
                            _targetedCard.SetHighlightColor(false);
                            _cardSelections.Add(_targetedCard);
                        }
                        else
                        {
                            _targetedCard.SetHighlightColor(true);
                            _cardSelections.Remove(_targetedCard);
                        }

                        break;
                    }
                }
                
            }
            else if (_targetedShield && !_selectCard)
            {
                foreach (CardBehaviour card in _selectionRange)
                {
                    if (_targetedShield == card)
                    {
                        if (!_reachedSelectionRange ||
                            _shieldSelections.Count >= _selectionLowerBound && _shieldSelections.Count <= _selectionUpperBound)
                        {
                            if (!_shieldSelections.Contains(_targetedShield))
                            {
                                if (_shieldSelections.Count < _selectionUpperBound) 
                                {
                                    _targetedShield.KeepHighlighted = true;
                                    _shieldSelections.Add(_targetedShield);
                                }
                            }
                            else
                            {
                                _targetedShield.KeepHighlighted = false;
                                _shieldSelections.Remove(_targetedShield);
                            }

                            if (!_reachedSelectionRange && _shieldSelections.Count >= _selectionLowerBound)
                                _reachedSelectionRange = true;
                            else if (_reachedSelectionRange && _shieldSelections.Count < _selectionLowerBound)
                                _reachedSelectionRange = false;
                        }

                        break;
                    }
                }
            }
        }
        else
        {
            if (_targetedCard)
            {
                if (GameDataHandler.Instance.GetDataHandler(_isPlayer).AllCards.ContainsKey(iD))
                {
                    _targetedCard.ProcessMouseDown();
                    SelectCard(_targetedCard);
                }
                else if (GameDataHandler.Instance.GetDataHandler(!_isPlayer).AllCards.ContainsKey(iD)
                         && GameManager.Instance.CurrentStep == GameStepType.AttackStep)
                {
                    BattleManager.Instance.AttemptAttack(_isPlayer, _targetedCard);
                }
            }
            else if (_targetedShield && GameManager.Instance.CurrentStep == GameStepType.AttackStep)
            {
                if (_currentlySelected.CardInst.InstanceEffectHandler.IsMultipleBreaker &&
                    GameDataHandler.Instance.GetDataHandler(!_isPlayer).Shields.Count > 1) 
                {
                    int shieldsToBreak = 0;
                    switch (_currentlySelected.CardInst.InstanceEffectHandler.MultipleBreakerType)
                    {
                        case MultipleBreakerType.DoubleBreaker:
                            shieldsToBreak = 2;
                            break;

                        case MultipleBreakerType.TripleBreaker:
                            shieldsToBreak = 3;
                            break;

                        case MultipleBreakerType.CrewBreaker:
                            break;
                    }

                    StartCoroutine(ProcessMultipleShieldBreakRoutine(shieldsToBreak));
                    ProcessInput(_targetedShield.GetInstanceID());
                }
                else
                    BattleManager.Instance.AttemptAttack(_isPlayer, _targetedShield);
            }
        }
    }

    public IEnumerator ProcessBlockingRoutine()
    {
        List<CreatureObject> blockers = GameDataHandler.Instance.GetDataHandler(_isPlayer).BlockersInBattle;
        if (blockers.Count > 0) 
        {
            List<CardBehaviour> selectedCards;
            Coroutine<List<CardBehaviour>> routine =
                this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(1, 1, true,
                    new List<CardBehaviour>(blockers), null, true));
            yield return routine.coroutine;
            selectedCards = routine.returnVal;

            if (selectedCards.Count == 1)
                yield return (CreatureObject) selectedCards[0];
            else
                yield return null;
        }

        yield return null;
    }
    
    public IEnumerator ProcessEvolvingRoutine(CardParams.Race[] races)
    {
        List<CreatureObject> matchingCreatures = new List<CreatureObject>();

        EffectTargetingCondition targetingCondition = new EffectTargetingCondition();
        foreach (CardParams.Race race in races)
        {
            targetingCondition.AddRaceCondition(new RaceCondition
            {
                connector = ConnectorType.Or,
                race = race
            });
        }
        
        Dictionary<int, CreatureObject> battleCreatures = GameDataHandler.Instance.GetDataHandler(_isPlayer).CardsInBattle;
        foreach (KeyValuePair<int, CreatureObject> pair in battleCreatures)
        {
            CreatureObject creatureObj = pair.Value;
            if (CardData.IsTargetingConditionSatisfied(creatureObj.CardInst, targetingCondition)) 
            {
                matchingCreatures.Add(creatureObj);
            }
        }

        if (matchingCreatures.Count > 0) 
        {
            List<CardBehaviour> selectedCards;
            Coroutine<List<CardBehaviour>> routine =
                this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(1, 1, true,
                    new List<CardBehaviour>(matchingCreatures), null, true));
            yield return routine.coroutine;
            selectedCards = routine.returnVal;

            if (selectedCards.Count == 1)
                yield return (CreatureObject) selectedCards[0];
            else
                yield return null;
        }

        yield return null;
    }

    private IEnumerator ProcessMultipleShieldBreakRoutine(int shieldsToBreak)
    {
        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(shieldsToBreak, shieldsToBreak, false,
                GameDataHandler.Instance.GetDataHandler(!_isPlayer).Shields, false));
        yield return routine.coroutine;
        
        List<CardBehaviour> targetedShields = routine.returnVal;
        for (int i = 0, n = targetedShields.Count; i < n; i++)
        {
            if (i == 0)
                yield return BattleManager.Instance.AttemptAttack(_isPlayer, targetedShields[i], true, true);
            else if (i > 0 && i < n - 1)
                yield return BattleManager.Instance.AttemptAttack(_isPlayer, targetedShields[i], false, true);
            else if (i == n - 1)
                yield return BattleManager.Instance.AttemptAttack(_isPlayer, targetedShields[i], false);
        }
    }

    #region Number Selection Methods

    public virtual IEnumerator GetNumberSelectionRoutine(int lower, int upper)
    {
        yield break;
    }

    #endregion

    #region May Activate Effect Methods

    public virtual IEnumerator ChooseEffectActivationRoutine()
    {
        _choosingEffectActivation = true;
        while (_choosingEffectActivation) 
            yield return new WaitForEndOfFrame();

        yield return _activateEffect;
    }

    public void ActivateEffectActivation()
    {
        _activateEffect = true;
        _choosingEffectActivation = false;
    }

    public void CancelEffectActivation()
    {
        _activateEffect = false;
        _choosingEffectActivation = false;
    }

    #endregion

    #region Multiple Card Selection Methods

    protected void SubmitSelection()
    {
        _selectionMade = true;
    }

    protected void CancelSelection()
    {
        _cardSelections.Clear();
        _shieldSelections.Clear();
        SubmitSelection();
    }

    public IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, List<CardObject> cardList, 
        EffectTargetingCondition targetingCondition, bool mayUse)
    {
        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(lower, upper, selectCard,
                new List<CardBehaviour>(cardList), targetingCondition, mayUse));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }
    
    public IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, List<ShieldObject> shieldList, bool mayUse)
    {
        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(lower, upper, selectCard,
                new List<CardBehaviour>(shieldList), null, mayUse));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }

    public IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard,
        Dictionary<int, CreatureObject> creatureDict, EffectTargetingCondition targetingCondition, bool mayUse)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();
        foreach (CardBehaviour card in creatureDict.Values)
            cards.Add(card);
        
        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(lower, upper, selectCard, cards,
                targetingCondition, mayUse));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }

    public IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, Dictionary<int, CreatureObject> creatureDict1,
        Dictionary<int, CreatureObject> creatureDict2, EffectTargetingCondition targetingCondition, bool mayUse)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();
        foreach (CardBehaviour card in creatureDict1.Values)
            cards.Add(card);
        foreach (CardBehaviour card in creatureDict2.Values)
            cards.Add(card);

        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(lower, upper, selectCard, cards,
                targetingCondition, mayUse));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }
    
    protected virtual IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, List<CardBehaviour> cards,
        EffectTargetingCondition targetingCondition, bool mayUse)
    {
        _selectionLowerBound = Mathf.Min(lower, cards.Count);
        _selectionUpperBound = Mathf.Min(upper, cards.Count);

        _selectMultiple = true;
        _selectCard = selectCard;
        if (_selectCard)
        {
            foreach (CardBehaviour card in cards)
            {
                CardObject cardObj = (CardObject) card;
                cardObj.SetValidity(targetingCondition);
                if (cardObj.IsValid && _isPlayer)
                    cardObj.SetHighlight(true);
            }
        }
        _selectionRange = cards;
        
        while (!_selectionMade)
            yield return new WaitForEndOfFrame();

        List<CardBehaviour> selectedCards;
        if (_selectCard)
        {
            foreach (CardBehaviour card in _selectionRange)
            {
                CardObject cardObj = (CardObject) card;
                cardObj.SetHighlightColor(true);
                cardObj.SetHighlight(false);
            }

            selectedCards = new List<CardBehaviour>(_cardSelections);
            _cardSelections.Clear();
        }
        else
        {
            foreach (ShieldObject shieldObj in _shieldSelections)
            {
                shieldObj.KeepHighlighted = false;
                shieldObj.SetHighlight(false);
            }
            
            selectedCards = new List<CardBehaviour>(_shieldSelections);
            _shieldSelections.Clear();
        }

        _selectMultiple = false;
        _reachedSelectionRange = false;
        _selectionMade = false;
        _selectionRange.Clear();
        
        yield return selectedCards;
    }
    
    #endregion
}
