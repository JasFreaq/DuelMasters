using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Helper Data Structures

    enum HoverState
    {
        None,
        Hover
    };

    #endregion
    
    private PlayerManager _manager;
    private Camera _mainCamera = null;

    private CardManager _hoveredCard;
    private Shield _hoveredShield;

    private HoverState _hoverState = HoverState.None;

    private void Awake()
    {
        _manager = GetComponent<PlayerManager>();
    }

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int iD = hit.transform.GetInstanceID();
            ProcessCardHover(iD);
            ProcessShieldHover(iD);
            ProcessInput(iD);
        }
    }

    private void ProcessCardHover(int iD)
    {
        CardManager tempCard = null;
        if (GameManager.PlayerDataHandler.AllCards.ContainsKey(iD))
            tempCard = GameManager.PlayerDataHandler.AllCards[iD];
        else if (GameManager.OpponentDataHandler.AllCards.ContainsKey(iD))
            tempCard = GameManager.OpponentDataHandler.AllCards[iD];

        if (tempCard)
        {
            if (_hoveredCard != tempCard)
            {
                ExitHover();

                if (_hoverState == HoverState.None)
                {
                    _hoveredCard = tempCard;

                    //OnMouseEnter
                    _hoveredCard.ProcessMouseEnter();
                }

                _hoverState = HoverState.Hover;
            }
        }
        else if (_hoveredCard)
        {
            ExitHover();
        }

        #region Local Functions

        void ExitHover()
        {
            if (_hoverState == HoverState.Hover)
            {
                //OnMouseExit
                _hoveredCard.ProcessMouseExit();
                _hoveredCard = null;
            }

            _hoverState = HoverState.None;
        }

        #endregion
    }

    private void ProcessShieldHover(int iD)
    {
        if (_manager.CurrentlySelected &&
                 _manager.CurrentlySelected.CurrentZone == CardZone.BattleZone)
        {
            Shield tempShield = null;
            foreach (Shield shield in GameManager.OpponentDataHandler.Shields)
            {
                if (shield.transform.GetInstanceID() == iD)
                {
                    tempShield = shield;
                    break;
                }
            }

            if (tempShield)
            {
                if (_hoveredShield != tempShield)
                {
                    ExitHover();

                    if (_hoverState == HoverState.None)
                    {
                        _hoveredShield = tempShield;

                        //OnMouseEnter
                        _hoveredShield.SetHighlight(true);
                    }

                    _hoverState = HoverState.Hover;
                }
            }
            else if (_hoveredShield)
            {
                ExitHover();
            }
        }

        #region Local Functions

        void ExitHover()
        {
            if (_hoverState == HoverState.Hover)
            {
                _hoveredShield.SetHighlight(false);
                _hoveredShield = null;
            }

            _hoverState = HoverState.None;
        }

        #endregion
    }

    private void ProcessInput(int iD)
    {
        if (_hoverState == HoverState.Hover)
        {
            if (_hoveredCard && GameManager.PlayerDataHandler.AllCards.ContainsKey(iD))
            {
                //OnMouseOver

                if (Input.GetMouseButtonDown(0))
                {
                    //OnMouseDown
                    _hoveredCard.ProcessMouseDown();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    //OnMouseOver
                }
            }
        }
    }
}
