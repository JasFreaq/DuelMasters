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
        List<CardInstanceObject> tappedCards = currentPlayer.DataHandler.TappedCards;
        foreach (CardInstanceObject cardObj in tappedCards)
        {
            cardObj.ToggleTap();
        }

        yield break;
    }
}
