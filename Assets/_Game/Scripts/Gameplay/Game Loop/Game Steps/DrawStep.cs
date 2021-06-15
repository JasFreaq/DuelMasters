using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawStep : GameStep
{
    public DrawStep(GameManager manager) : base(manager)
    {
        _stepType = GameStepType.DrawStep;
        _nextStepType = GameStepType.ChargeStep;
    }
    
    public override IEnumerator StartStepRoutine(PlayerManager currentPlayer)
    {
        if (!_gameManager.FirstTurn)
            yield return currentPlayer.DrawCardRoutine();
    }
}
