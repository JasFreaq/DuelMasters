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

    public override IEnumerator ProcessGameAction(CardObject cardObj, PlayerManager currentPlayer)
    {
        PlayerDataHandler dataHandler = currentPlayer.DataHandler;

        if (currentPlayer.PlayableCards.Contains(cardObj)) 
        {
            cardObj.DragHandler.ResetDragging();

            dataHandler.PayCost(cardObj.CardData.Civilization, cardObj.CardData.Cost);
            currentPlayer.ManaZoneManager.ArrangeCards();
            yield return currentPlayer.PlayCardFromHandRoutine(cardObj);

            CheckPlayableCards(currentPlayer);
        }
        else
        {
            cardObj.HoverPreviewHandler.ShouldStopPreview = false;
            cardObj.DragHandler.EndDragging();
        }
    }

    private void CheckPlayableCards(PlayerManager currentPlayer)
    {
        int playableCards = currentPlayer.SetPlayableCards();
        if (playableCards == 0)
        {
            _gameManager.EndCurrentStep();
        }
    }
}
