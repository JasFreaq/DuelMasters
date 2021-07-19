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

    public override IEnumerator ProcessGameAction(CardObject cardObj, PlayerManager currentPlayer)
    {
        PlayerDataHandler dataHandler = currentPlayer.DataHandler;

        cardObj.DragHandler.ResetDragging();
        yield return currentPlayer.ChargeManaRoutine(cardObj);

        if (cardObj.CardInst.CardData.Civilization.Length > 1)
        {
            cardObj.ToggleTapState();
            currentPlayer.ManaZoneManager.ArrangeCards();
        }

        _gameManager.EndCurrentStep();
    }
}
