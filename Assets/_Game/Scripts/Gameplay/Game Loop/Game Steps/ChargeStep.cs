using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeStep : GameStep
{
    public ChargeStep(GameManager manager) : base(manager)
    {
        _stepType = GameStepType.ChargeStep;
        _nextStepType = GameStepType.MainStep;
        _updateStep = true;
    }

    public override IEnumerator ProcessGameAction(CardManager card, PlayerManager currentPlayer)
    {
        PlayerDataHandler dataHandler = currentPlayer.DataHandler;

        yield return currentPlayer.ChargeManaRoutine(card);

        if (card.CardData.Civilization.Length > 1)
        {
            card.ToggleTap();
            dataHandler.TappedCards.Add(card);
        }

        _gameManager.EndCurrentStep();
    }
}
