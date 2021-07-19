using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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
    
    private Camera _mainCamera = null;

    private CardObject _currentlySelected;
    private CardObject _hoveredCard;
    private ShieldObject _hoveredShield;

    private bool _canInteract = false;
    private HoverState _hoverState = HoverState.None;

    public bool CanInteract
    {
        set { _canInteract = value; }
    }

    void Start()
    {
        _mainCamera = Camera.main;
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

    private void SelectCard(CardObject cardObj)
    {
        if (_currentlySelected != cardObj)
        {
            _currentlySelected = cardObj;

            switch (GameManager.Instance.CurrentStep)
            {
                case GameStepType.ChargeStep:
                    _currentlySelected.SetHighlight(true);
                    break;

                case GameStepType.MainStep:
                    PlayerManager player = GameManager.Instance.GetManager(true);
                    foreach (CardObject cardManager in player.PlayableCards)
                    {
                        if (cardManager != _currentlySelected)
                            cardManager.SetHighlight(false);
                    }
                    break;

                case GameStepType.AttackStep:
                    if (!_currentlySelected.CardInst.IsTapped)
                    {
                        _currentlySelected.SetHighlight(true);
                        TargetingLinesHandler.Instance.EnableLine(_currentlySelected.transform.position);
                    }
                    else
                        _currentlySelected = null;
                    break;
            }
        }
        else if (_currentlySelected)
        {
            DeselectCurrentlySelected();
        }
    }

    public void DeselectCurrentlySelected()
    {
        switch (GameManager.Instance.CurrentStep)
        {
            case GameStepType.ChargeStep:
            case GameStepType.AttackStep:
                _currentlySelected.SetHighlight(false);
                break;

            case GameStepType.MainStep:
                if (_currentlySelected.ProcessAction)
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
        _currentlySelected = null;
    }

    
    private void ProcessCardHover(int iD)
    {
        CardObject tempCardObj = null;
        PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(true);

        if (dataHandler.AllCards.ContainsKey(iD))
            tempCardObj = (CardObject) dataHandler.AllCards[iD];
        else
        {
            dataHandler = GameDataHandler.Instance.GetDataHandler(false);
            if (dataHandler.AllCards.ContainsKey(iD))
                tempCardObj = (CardObject) dataHandler.AllCards[iD];
        }

        if (tempCardObj)
        {
            if (_hoveredCard != tempCardObj)
            {
                ExitHover();

                if (_hoverState == HoverState.None)
                {
                    _hoveredCard = tempCardObj;
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
                if (_hoveredShield != tempShield)
                {
                    ExitHover();

                    if (_hoverState == HoverState.None)
                    {
                        _hoveredShield = tempShield;
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
            if (_hoveredCard)
            {
                if (GameDataHandler.Instance.GetDataHandler(true).AllCards.ContainsKey(iD))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _hoveredCard.ProcessMouseDown();
                        SelectCard(_hoveredCard);
                    }
                }
                else if (GameDataHandler.Instance.GetDataHandler(false).AllCards.ContainsKey(iD))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(AttemptAttack(_hoveredCard));
                    }
                }
            }
            else if (_hoveredShield)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(AttemptAttack(_hoveredShield));
                }
            }
        }

        #region Local Functions

        IEnumerator AttemptAttack(CardBehaviour target)
        {
            if (_currentlySelected && _currentlySelected.InZone(CardZoneType.BattleZone)
                                   && !_currentlySelected.CardInst.IsTapped)
            {
                TargetingLinesHandler.Instance.DisableLines();

                CreatureObject creatureObj = (CreatureObject) _currentlySelected;
                float attackTime = GameParamsHolder.Instance.AttackTime;
                Vector3 creaturePos = creatureObj.transform.position;
                
                if (target is CreatureObject)
                {
                    CreatureObject attackedCreatureObj = (CreatureObject) target;
                    if (attackedCreatureObj.CardInst.IsTapped)
                    {
                        creatureObj.transform.DOMove(target.transform.position, attackTime).SetEase(Ease.InCubic);
                        yield return new WaitForSeconds(attackTime);
                        DeselectCurrentlySelected();

                        int attackingCraturePower = ((CreatureData) creatureObj.CardInst.CardData).Power;
                        int attackedCreaturePower = ((CreatureData) attackedCreatureObj.CardInst.CardData).Power;

                        if (attackingCraturePower > attackedCreaturePower)
                            attackedCreatureObj.DestroyCard();
                        else if (attackingCraturePower == attackedCreaturePower)
                        {
                            creatureObj.DestroyCard();
                            creatureObj = null;
                            attackedCreatureObj.DestroyCard();
                        }
                        else
                        {
                            creatureObj.DestroyCard();
                            creatureObj = null;
                        }

                        yield return ReturnCreature();
                    }
                }
                else if (target is ShieldObject)
                {
                    creatureObj.transform.DOMove(target.transform.position, attackTime).SetEase(Ease.InCubic);
                    yield return new WaitForSeconds(attackTime);
                    DeselectCurrentlySelected();

                    PlayerManager opponent = GameManager.Instance.GetManager(false);
                    StartCoroutine(opponent.BreakShieldRoutine(target.transform.GetSiblingIndex()));

                    yield return ReturnCreature();
                }

                #region Local Functions

                IEnumerator ReturnCreature()
                {
                    if (creatureObj)
                    {
                        creatureObj.transform.DOMove(creaturePos, attackTime).SetEase(Ease.InCubic);
                        yield return new WaitForSeconds(attackTime);
                        creatureObj.ToggleTapState();
                    }
                }

                #endregion
            }
        }

        #endregion
    }
}
