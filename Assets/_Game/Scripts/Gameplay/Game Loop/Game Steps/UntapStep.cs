using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UntapStep : GameStep
{
    public UntapStep(GameManager manager) : base(manager)
    {
        _stepType = GameStepType.UntapStep;
        _nextStepType = GameStepType.StartOfTurnStep;
    }

    public override IEnumerator StartStepRoutine(PlayerManager currentPlayer)
    {
        foreach (CardInstanceObject card in currentPlayer.DataHandler.TappedCards)
        {
            card.ToggleTap();
        }

        currentPlayer.DataHandler.TappedCards.Clear();

        yield break;
    }
}
