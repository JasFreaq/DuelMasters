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

    [SerializeField] private int _deselectDelayBufferFrames = 30;

    private Camera _mainCamera = null;
    private GameManager _gameManager = null;

    private bool _canInteract, _canSelect, _canSelectShield;
    private HoverState _hoverState = HoverState.None;

    private List<CardBehaviour> _selectionRange = new List<CardBehaviour>();
    private List<CardObject> _cardSelections = new List<CardObject>();
    private List<ShieldObject> _shieldSelections = new List<ShieldObject>();
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
                    {
                        TargetingLinesHandler.Instance.SetLine(hit.point);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            _selectionMade = true;
    }

    public void EnableFullControl(bool enable)
    {
        _canInteract = enable;
        _canSelect = enable;
    }

    public override void DeselectCurrentlySelected()
    {
        base.DeselectCurrentlySelected();

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
    
    public IEnumerator SelectCardsRoutine(bool selectCard, List<ShieldObject> shieldList)
    {
        Coroutine<List<CardBehaviour>> routine = this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(selectCard, new List<CardBehaviour>(shieldList)));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }
    
    public IEnumerator SelectCardsRoutine(bool selectCard, Dictionary<int, CreatureObject> creatureDict)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();
        foreach (KeyValuePair<int, CreatureObject> pair in creatureDict)
        {
            CardBehaviour card = pair.Value;
            cards.Add(card);
        }

        Coroutine<List<CardBehaviour>> routine = this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(selectCard, cards));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }
    
    public IEnumerator SelectCardsRoutine(bool selectCard, Dictionary<int, CardObject> cardDict)
    {
        List<CardBehaviour> cards = new List<CardBehaviour>();
        foreach (KeyValuePair<int, CardObject> pair in cardDict)
        {
            CardBehaviour card = pair.Value;
            cards.Add(card);
        }

        Coroutine<List<CardBehaviour>> routine = this.StartCoroutine<List<CardBehaviour>>(SelectCardsRoutine(selectCard, cards));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }

    private IEnumerator SelectCardsRoutine(bool selectCard, List<CardBehaviour> cards)
    {
        _selectMultiple = true;
        _selectCard = selectCard;
        _selectionRange = cards;

        if (_cardSelections.Count > 0) 
            _cardSelections.Clear();
        if (_shieldSelections.Count > 0) 
            _shieldSelections.Clear();

        while (!_selectionMade)
            yield return new WaitForEndOfFrame();

        if (_selectCard)
        {
            foreach (CardObject cardObj in _cardSelections)
                cardObj.SetHighlight(false);
        }
        else
        {
            foreach (ShieldObject shieldObj in _shieldSelections)
            {
                shieldObj.KeepHighlighted = false;
                shieldObj.SetHighlight(false);
            }
        }

        _selectMultiple = false;
        _selectionMade = false;
        _selectionRange.Clear();

        if (_selectCard)
            yield return _cardSelections;
        else
            yield return _shieldSelections;
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
                        if (_targetedCard == card)
                        {
                            if (!_cardSelections.Contains(_targetedCard)) 
                            {
                                _targetedCard.SetHighlight(true);
                                _cardSelections.Add(_targetedCard);
                                
                            }
                            else
                            {
                                _targetedCard.SetHighlight(false);
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
                        if (_targetedShield == card)
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
            base.ProcessInput(iD);
    }
}
