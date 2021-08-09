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
        CardData cardData = cardObj.CardInst.CardData;

        if (currentPlayer.PlayableCards.Contains(cardObj)) 
        {
            cardObj.DragHandler.ResetDragging();

            dataHandler.PayCost(cardData.Civilization, cardData.Cost);
            currentPlayer.ManaZoneManager.ArrangeCards();
            yield return currentPlayer.PlayCardRoutine(cardObj);

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
