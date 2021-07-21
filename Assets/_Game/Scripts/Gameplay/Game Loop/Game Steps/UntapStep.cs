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
        List<CardObject> tappedCards = new List<CardObject>(currentPlayer.DataHandler.TappedCards);
        foreach (CardObject cardObj in tappedCards)
        {
            cardObj.ToggleTapState();
        }

        yield break;
    }
}
