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
    protected bool _selectMultiple, _selectCard = true, _selectionMade;

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
            if (_targetedCard)
            {
                if (_selectCard)
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
            }
            else if (_targetedShield)
            {
                if (!_selectCard)
                {
                    foreach (CardBehaviour card in _selectionRange)
                    {
                        if (_targetedShield == card &&
                            _shieldSelections.Count >= _selectionLowerBound && _shieldSelections.Count <= _selectionUpperBound)
                        {
                            if (!_shieldSelections.Contains(_targetedShield))
                            {
                                _targetedShield.KeepHighlighted = true;
                                _shieldSelections.Add(_targetedShield);
                            }
                            else
                            {
                                _targetedShield.KeepHighlighted = false;
                                _shieldSelections.Remove(_targetedShield);
                            }

                            break;
                        }
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
                    //if (_currentlySelected && _currentlySelected.CardInst.CanAttackCreatures)
                    GameManager.Instance.AttemptAttack(_isPlayer, _targetedCard);
                }
            }
            else if (_targetedShield && GameManager.Instance.CurrentStep == GameStepType.AttackStep)
            {
                //if (_currentlySelected && _currentlySelected.CardInst.CanAttackPlayers)
                GameManager.Instance.AttemptAttack(_isPlayer, _targetedShield);
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
                    new List<CardBehaviour>(blockers), null));
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
                    new List<CardBehaviour>(matchingCreatures), null));
            yield return routine.coroutine;
            selectedCards = routine.returnVal;

            if (selectedCards.Count == 1)
                yield return (CreatureObject) selectedCards[0];
            else
                yield return null;
        }

        yield return null;
    }

    #region Multiple Card Selection Methods

    public void SubmitSelection()
    {
        _selectionMade = true;
    }

    public void CancelSelection()
    {
        _cardSelections.Clear();
        _shieldSelections.Clear();
        SubmitSelection();
    }

    public IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, List<ShieldObject> shieldList)
    {
        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(lower, upper, selectCard,
                new List<CardBehaviour>(shieldList), null));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }

    public IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard,
        Dictionary<int, CreatureObject> creatureDict, EffectTargetingCondition targetingCondition)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();
        foreach (KeyValuePair<int, CreatureObject> pair in creatureDict)
        {
            CardBehaviour card = pair.Value;
            cards.Add(card);
        }

        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(lower, upper, selectCard, cards,
                targetingCondition));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }

    public IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, Dictionary<int, CardObject> cardDict,
        EffectTargetingCondition targetingCondition)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();
        foreach (KeyValuePair<int, CardObject> pair in cardDict)
        {
            CardBehaviour card = pair.Value;
            cards.Add(card);
        }

        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(lower, upper, selectCard, cards,
                targetingCondition));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }

    protected virtual IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, List<CardBehaviour> cards,
        EffectTargetingCondition targetingCondition)
    {
        _selectionLowerBound = lower;
        _selectionUpperBound = upper;

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
        _selectionMade = false;
        _selectionRange.Clear();
        
        yield return selectedCards;
    }
    
    #endregion
}
