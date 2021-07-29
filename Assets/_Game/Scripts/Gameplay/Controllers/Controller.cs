using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected CardObject _currentlySelected;
    protected CardObject _targetedCard;
    protected ShieldObject _targetedShield;

    protected bool _isPlayer;

    public CardObject CurrentlySelected
    {
        get { return _currentlySelected; }
    }

    public bool IsPlayer
    {
        get { return _isPlayer; }
    }
    
    public void SelectCard(CardObject cardObj)
    {
        if (_currentlySelected != cardObj)
        {
            if (_currentlySelected) 
                DeselectCurrentlySelected();

            _currentlySelected = cardObj;

            switch (GameManager.Instance.CurrentStep)
            {
                case GameStepType.ChargeStep:
                    _currentlySelected.SetHighlight(true);
                    break;

                case GameStepType.MainStep:
                    PlayerManager player = GameManager.Instance.GetManager(true);
                    foreach (CardObject cardObj0 in player.PlayableCards)
                    {
                        if (cardObj0 != _currentlySelected)
                            cardObj0.SetHighlight(false);
                    }

                    break;

                case GameStepType.AttackStep:
                    if (_currentlySelected.CardInst.CanAttack())
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
    
    public virtual void DeselectCurrentlySelected()
    {
        switch (GameManager.Instance.CurrentStep)
        {
            case GameStepType.ChargeStep:
            case GameStepType.AttackStep:
                _currentlySelected.SetHighlight(false);
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
        _currentlySelected = null;
    }
    
    protected virtual void ProcessInput(int iD)
    {
        if (_targetedCard)
        {
            if (GameDataHandler.Instance.GetDataHandler(_isPlayer).AllCards.ContainsKey(iD))
            {
                _targetedCard.ProcessMouseDown();
                SelectCard(_targetedCard);
            }
            else if (GameDataHandler.Instance.GetDataHandler(!_isPlayer).AllCards.ContainsKey(iD))
            {
                //if (_currentlySelected && _currentlySelected.CardInst.CanAttackCreatures)
                GameManager.Instance.AttemptAttack(_isPlayer, _targetedCard);
            }
        }
        else if (_targetedShield)
        {
            //if (_currentlySelected && _currentlySelected.CardInst.CanAttackPlayers)
            GameManager.Instance.AttemptAttack(_isPlayer, _targetedShield);
        }
    }
    
}
