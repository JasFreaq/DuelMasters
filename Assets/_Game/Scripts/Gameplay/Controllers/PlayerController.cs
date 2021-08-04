using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    #region Helper Data Structures

    enum HoverState
    {
        None,
        Hover
    };

    #endregion

    [SerializeField] private ActionButtonOverlay _actionOverlay;
    [SerializeField] private int _deselectDelayBufferFrames = 30;

    private Camera _mainCamera = null;
    private GameManager _gameManager = null;

    private bool _canInteract, _canSelect, _canSelectShield;
    private HoverState _hoverState = HoverState.None;

    private List<CardBehaviour> _selectionRange = new List<CardBehaviour>();
    private List<CardObject> _cardSelections = new List<CardObject>();
    private List<ShieldObject> _shieldSelections = new List<ShieldObject>();
    private int _selectionLowerBound, _selectionUpperBound;
    private bool _selectMultiple, _selectCard = true, _selectionMade;

    public bool CanInteract
    {
        set { _canInteract = value; }
    }
    
    public bool CanSelect
    {
        set { _canSelect = value; }
    }

    public bool CanSelectShield
    {
        set { _canSelectShield = value; }
    }

    void Start()
    {
        _mainCamera = Camera.main;
        _gameManager = GameManager.Instance;
        _isPlayer = true;
    }

    void Update()
    {
        if (_canInteract) 
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                int iD = hit.transform.GetInstanceID();
                ProcessCardHover(iD);
                ProcessShieldHover(iD);

                if (_hoverState == HoverState.Hover && _canSelect && Input.GetMouseButtonDown(0))
                    ProcessInput(iD);
                
                if (_gameManager.CurrentStep == GameStepType.AttackStep)
                {
                    if (_currentlySelected && _currentlySelected.InZone(CardZoneType.BattleZone))
                        TargetingLinesHandler.Instance.SetLine(hit.point);
                }
            }
        }
    }

    public void EnableFullControl(bool enable)
    {
        _canInteract = enable;
        _canSelect = enable;
    }

    public override void DeselectCurrentlySelected()
    {
        base.DeselectCurrentlySelected();

        //TargetingLinesHandler.Instance.DisableLines();
        StartCoroutine(DelayedDeselectRoutine());

        #region Local Functions

        IEnumerator DelayedDeselectRoutine()
        {
            int count = _deselectDelayBufferFrames;
            while (count > 0)
            {
                count--;
                yield return new WaitForEndOfFrame();
            }
            _canSelectShield = false;
        }

        #endregion
    }

    private void ProcessCardHover(int iD)
    {
        CardObject tempCardObj = null;
        PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(true);
        
        if (dataHandler.AllCards.ContainsKey(iD))
            tempCardObj = dataHandler.AllCards[iD];
        else
        {
            dataHandler = GameDataHandler.Instance.GetDataHandler(false);
            if (dataHandler.AllCards.ContainsKey(iD))
                tempCardObj = dataHandler.AllCards[iD];
        }

        if (tempCardObj)
        {
            if (_targetedCard != tempCardObj)
            {
                ExitHover();
                
                if (_hoverState == HoverState.None)
                {
                    _targetedCard = tempCardObj;
                    _targetedCard.ProcessMouseEnter();
                }

                _hoverState = HoverState.Hover;
            }
        }
        else
        {
            ExitHover();
        }

        #region Local Functions

        void ExitHover()
        {
            if (_targetedCard) 
            {
                if (_hoverState == HoverState.Hover)
                {
                    //OnMouseExit
                    _targetedCard.ProcessMouseExit();
                    _targetedCard = null;
                }
                
                _hoverState = HoverState.None;
            }
        }

        #endregion
    }

    private void ProcessShieldHover(int iD)
    {
        bool selectToAttack = false;
        if (_currentlySelected && _currentlySelected.InZone(CardZoneType.BattleZone))
            _canSelectShield = selectToAttack = true;//_currentlySelected.CanAttackPlayer
            
        if (_canSelectShield)
        {
            ShieldObject tempShield = null;
            if (_selectMultiple)
            {
                foreach (CardBehaviour card in _selectionRange)
                {
                    if (card.transform.GetInstanceID() == iD)
                    {
                        tempShield = (ShieldObject) card;
                        break;
                    }
                }
            }
            else
            {
                PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(true);
                if (!selectToAttack)
                {
                    foreach (ShieldObject shield in dataHandler.Shields)
                    {
                        if (shield.transform.GetInstanceID() == iD)
                        {
                            tempShield = shield;
                            break;
                        }
                    }
                }

                if (!tempShield)
                {
                    dataHandler = GameDataHandler.Instance.GetDataHandler(false);
                    foreach (ShieldObject shield in dataHandler.Shields)
                    {
                        if (shield.transform.GetInstanceID() == iD)
                        {
                            tempShield = shield;
                            break;
                        }
                    }
                }
            }

            if (tempShield)
            {
                if (_targetedShield != tempShield)
                {
                    ExitHover();

                    if (_hoverState == HoverState.None)
                    {
                        _targetedShield = tempShield;
                        _targetedShield.SetHighlight(true);
                    }

                    _hoverState = HoverState.Hover;
                }
            }
            else
            {
                ExitHover();
            }
        }

        #region Local Functions

        void ExitHover()
        {
            if (_targetedShield) 
            {
                if (_hoverState == HoverState.Hover)
                {
                    _targetedShield.SetHighlight(false);
                    _targetedShield = null;
                }

                _hoverState = HoverState.None;
            }
        }

        #endregion
    }

    #region Multiple Cards Selection Methods
    
    public IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, List<ShieldObject> shieldList)
    {
        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(lower, upper, selectCard,
                new List<CardBehaviour>(shieldList)));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }

    public IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard,
        Dictionary<int, CreatureObject> creatureDict, EffectTargetingCondition targetingCondition = null)
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
        EffectTargetingCondition targetingCondition = null)
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

    private IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, List<CardBehaviour> cards,
        EffectTargetingCondition targetingCondition = null)
    {
        _selectionLowerBound = lower;
        _selectionUpperBound = upper;

        _selectMultiple = true;
        _selectCard = selectCard;
        if (_selectCard)
            cards = CardInstance.CheckValidity(cards, targetingCondition);
        _selectionRange = cards;
        
        _actionOverlay.ActivateCardSelectionButtons(SubmitSelection, CancelSelection);
        _actionOverlay.AdjustCardSelectionButtons(_selectionLowerBound, _selectionUpperBound, 0);

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
            selectedCards = new List<CardBehaviour>(_shieldSelections);
            foreach (ShieldObject shieldObj in _shieldSelections)
            {
                shieldObj.KeepHighlighted = false;
                shieldObj.SetHighlight(false);
            }
            _shieldSelections.Clear();
        }

        _selectMultiple = false;
        _selectionMade = false;
        _selectionRange.Clear();

        _actionOverlay.DeactivateButtons();

        yield return selectedCards;
    }

    private void SubmitSelection()
    {
        _selectionMade = true;
    }

    private void CancelSelection()
    {
        _cardSelections.Clear();
        _shieldSelections.Clear();
        SubmitSelection();
    }

    #endregion

    protected override void ProcessInput(int iD)
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

                            _actionOverlay.AdjustCardSelectionButtons(_selectionLowerBound, _selectionUpperBound,
                                _cardSelections.Count);

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

                            _actionOverlay.AdjustCardSelectionButtons(_selectionLowerBound, _selectionUpperBound,
                                _shieldSelections.Count);

                            break;
                        }
                    }
                }
            }
        }
        else
            base.ProcessInput(iD);
    }
}
