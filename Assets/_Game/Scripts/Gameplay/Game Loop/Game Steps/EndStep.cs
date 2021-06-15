using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndStep : GameStep
{
    public EndStep(GameManager manager) : base(manager)
    {
        _stepType = GameStepType.EndStep;
        _nextStepType = GameStepType.BeginStep;
    }

    public override IEnumerator FinishStepRoutine(PlayerManager currentPlayer)
    {
        if (_gameManager.FirstTurn)
            _gameManager.EndFirstTurn();
        _gameManager.TogglePlayerTurn();
        yield break;
    }
}
