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
    
    private Camera _mainCamera = null;

    private bool _canInteract = false, _canSelect = false;
    private HoverState _hoverState = HoverState.None;

    public bool CanInteract
    {
        set { _canInteract = value; }
    }
    
    public bool CanSelect
    {
        set { _canSelect = value; }
    }

    void Start()
    {
        _mainCamera = Camera.main;
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

                if (GameManager.Instance.CurrentStep == GameStepType.AttackStep)
                {
                    if (_currentlySelected && _currentlySelected.InZone(CardZoneType.BattleZone))
                    {
                        TargetingLinesHandler.Instance.SetLine(hit.point);
                    }
                }
            }
        }
    }

    public void EnableFullControl(bool enable)
    {
        _canInteract = enable;
        _canSelect = enable;
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
        else if (_targetedCard)
        {
            ExitHover();
        }

        #region Local Functions

        void ExitHover()
        {
            if (_hoverState == HoverState.Hover)
            {
                //OnMouseExit
                _targetedCard.ProcessMouseExit();
                _targetedCard = null;
            }

            _hoverState = HoverState.None;
        }

        #endregion
    }

    private void ProcessShieldHover(int iD)
    {
        if (_currentlySelected && _currentlySelected.InZone(CardZoneType.BattleZone))
        {
            ShieldObject tempShield = null;
            foreach (ShieldObject shield in GameDataHandler.Instance.GetDataHandler(false).Shields)
            {
                if (shield.transform.GetInstanceID() == iD)
                {
                    tempShield = shield;
                    break;
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
            else if (_targetedShield)
            {
                ExitHover();
            }
        }

        #region Local Functions

        void ExitHover()
        {
            if (_hoverState == HoverState.Hover)
            {
                _targetedShield.SetHighlight(false);
                _targetedShield = null;
            }

            _hoverState = HoverState.None;
        }

        #endregion
    }
}
