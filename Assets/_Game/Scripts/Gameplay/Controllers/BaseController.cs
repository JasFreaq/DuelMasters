using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected CardObject _currentlySelected;
    protected CardObject _targetedCard;
    protected ShieldObject _targetedShield;

    public CardObject CurrentlySelected
    {
        get { return _currentlySelected; }
    }

    public void SelectCard(CardObject cardObj)
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

    protected void ProcessInput(int iD)
    {
        if (_targetedCard)
        {
            if (GameDataHandler.Instance.GetDataHandler(true).AllCards.ContainsKey(iD))
            {
                _targetedCard.ProcessMouseDown();
                SelectCard(_targetedCard);
            }
            else if (GameDataHandler.Instance.GetDataHandler(false).AllCards.ContainsKey(iD))
            {
                GameManager.Instance.AttemptAttack(this, _targetedCard);
            }
        }
        else if (_targetedShield)
        {
            GameManager.Instance.AttemptAttack(this, _targetedShield);
        }
    }
    
}
