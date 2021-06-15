using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStep : GameStep
{
    public MainStep(GameManager manager) : base(manager)
    {
        _stepType = GameStepType.MainStep;
        _nextStepType = GameStepType.AttackStep;
        _updateStep = true;
    }

    public override IEnumerator StartStepRoutine(PlayerManager currentPlayer)
    {
        CheckPlayableCards(currentPlayer);
        yield break;
    }

    public override IEnumerator ProcessGameAction(CardManager card, PlayerManager currentPlayer)
    {
        PlayerDataHandler dataHandler = currentPlayer.DataHandler;
        Card cardData = card.CardData;

        if (dataHandler.CanPayCost(cardData.Civilization, cardData.Cost))
        {
            dataHandler.PayCost(cardData.Civilization, cardData.Cost);
            currentPlayer.ManaZoneManager.ArrangeCards();
            yield return currentPlayer.PlayCardRoutine(card);
            CheckPlayableCards(currentPlayer);
        }
        else
        {
            card.DragHandler.ReturnToPosition();
        }
    }

    private void CheckPlayableCards(PlayerManager currentPlayer)
    {
        int playableCards = _gameManager.HighlightPlayableCards(currentPlayer.DataHandler);
        if (playableCards == 0)
        {
            _gameManager.EndCurrentStep();
        }
    }
}
