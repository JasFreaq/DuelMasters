using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOfTurnStep : GameStep
{
    public StartOfTurnStep(GameManager manager) : base(manager)
    {
        _stepType = GameStepType.StartOfTurnStep;
        _nextStepType = GameStepType.DrawStep;
    }
}
