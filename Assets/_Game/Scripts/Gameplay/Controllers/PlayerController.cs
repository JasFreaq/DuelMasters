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
    [SerializeField] private int _deselectDelayBufferFrames = 24;

    private Camera _mainCamera = null;
    private GameManager _gameManager = null;

    private bool _canInteract, _canSelect, _canSelectShield;
    private HoverState _hoverState = HoverState.None;
    
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

    public override void SelectCard(CardObject cardObj)
    {
        if (_currentlySelected != cardObj)
        {
            switch (GameManager.Instance.CurrentStep)
            {
                case GameStepType.ChargeStep:
                    cardObj.SetHighlight(true);
                    break;

                case GameStepType.MainStep:
                    PlayerManager player = GameManager.Instance.GetManager(_isPlayer);
                    foreach (CardObject cardObj0 in player.PlayableCards)
                    {
                        if (cardObj0 != cardObj)
                            cardObj0.SetHighlight(false);
                    }

                    break;

                case GameStepType.AttackStep:
                    if (cardObj.CardInst.CanAttack())
                    {
                        cardObj.SetHighlight(true);
                        if (cardObj.InZone(CardZoneType.BattleZone))
                            TargetingLinesHandler.Instance.EnableLine(cardObj.transform.position);
                    }

                    break;
            }
        }

        base.SelectCard(cardObj);
    }

    public void EnableFullControl(bool enable)
    {
        _canInteract = enable;
        _canSelect = enable;
    }

    public override void DeselectCurrentlySelected()
    {
        switch (GameManager.Instance.CurrentStep)
        {
            case GameStepType.ChargeStep:
            case GameStepType.AttackStep:
                _currentlySelected.SetHighlight(false);
                _currentlySelected.SetHighlightColor(true);
                break;

            case GameStepType.MainStep:
                if (_currentlySelected.ProcessDragRelease)
                    _currentlySelected.SetHighlight(false);
                else
                {
                    PlayerManager player = GameManager.Instance.GetManager(true);
                    foreach (CardObject cardManager in player.PlayableCards)
                    {
                        if (cardManager != _currentlySelected)
                            cardManager.SetHighlight(true);
                    }
                }

                break;
        }

        TargetingLinesHandler.Instance.DisableLines();
        
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
    
    protected override void ProcessInput(int iD)
    {
        base.ProcessInput(iD);

        if (_selectMultiple)
        {
            if (_targetedCard)
            {
                if (_selectCard)
                {
                    _actionOverlay.AdjustCardSelectionButtons(_selectionLowerBound, _selectionUpperBound,
                        _cardSelections.Count);
                }
            }
            else if (_targetedShield)
            {
                if (!_selectCard)
                {
                    _actionOverlay.AdjustCardSelectionButtons(_selectionLowerBound, _selectionUpperBound,
                        _shieldSelections.Count);
                }
            }
        }
    }

    #region Multiple Cards Selection Methods

    protected override IEnumerator SelectCardsRoutine(int lower, int upper, bool selectCard, List<CardBehaviour> cards,
        EffectTargetingCondition targetingCondition = null)
    {
        _actionOverlay.ActivateCardSelectionButtons(SubmitSelection, CancelSelection);
        _actionOverlay.AdjustCardSelectionButtons(lower, upper, 0);

        Coroutine<List<CardBehaviour>> routine =
            this.StartCoroutine<List<CardBehaviour>>(base.SelectCardsRoutine(lower, upper, selectCard, cards,
                targetingCondition));
        yield return routine.coroutine;

        _actionOverlay.DeactivateButtons();

        yield return routine.returnVal;
    }
    
    #endregion
}
