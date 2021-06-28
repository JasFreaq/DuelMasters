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

    private CardInstanceObject _hoveredCard;
    private ShieldObject _hoveredShield;

    private HoverState _hoverState = HoverState.None;
    
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
        CardInstanceObject tempCardObj = null;
        PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(true);

        if (dataHandler.AllCards.ContainsKey(iD))
            tempCardObj = (CardInstanceObject) dataHandler.AllCards[iD];
        else
        {
            dataHandler = GameDataHandler.Instance.GetDataHandler(false);
            if (dataHandler.AllCards.ContainsKey(iD))
                tempCardObj = (CardInstanceObject) dataHandler.AllCards[iD];
        }

        if (tempCardObj)
        {
            if (_hoveredCard != tempCardObj)
            {
                ExitHover();

                if (_hoverState == HoverState.None)
                {
                    _hoveredCard = tempCardObj;

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
        PlayerManager player = GameManager.Instance.GetManager(true);
        if (player.CurrentlySelected &&
            player.CurrentlySelected.CurrentZone == CardZone.BattleZone)
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
            if (_hoveredCard)
            {
                if (GameDataHandler.Instance.GetDataHandler(true).AllCards.ContainsKey(iD))
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
            PlayerManager player = GameManager.Instance.GetManager(true);
            CardInstanceObject cardObj = player.CurrentlySelected;
            if (cardObj && cardObj.CurrentZone == CardZone.BattleZone 
                && !cardObj.IsTapped)
            {
                CreatureInstanceObject creatureObj = (CreatureInstanceObject) player.CurrentlySelected;
                float attackTime = GameParamsHolder.Instance.AttackTime;
                Vector3 creaturePos = creatureObj.transform.position;
                
                if (target is CreatureInstanceObject)
                {
                    CreatureInstanceObject attackedCreatureObj = (CreatureInstanceObject) target;
                    if (attackedCreatureObj.IsTapped)
                    {
                        creatureObj.transform.DOMove(target.transform.position, attackTime).SetEase(Ease.InCubic);
                        yield return new WaitForSeconds(attackTime);
                        player.DeselectCurrentlySelected();

                        CreatureData creatureData = (CreatureData) creatureObj.CardData;
                        CreatureData attackedCreatureData = (CreatureData) attackedCreatureObj.CardData;

                        if (creatureData.Power > attackedCreatureData.Power)
                            attackedCreatureObj.DestroyCard();
                        else if (creatureData.Power == attackedCreatureData.Power)
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
                    player.DeselectCurrentlySelected();

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
                        creatureObj.ToggleTap();
                    }
                }

                #endregion
            }
        }

        #endregion
    }
}
