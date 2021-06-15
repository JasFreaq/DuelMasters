using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginStep : GameStep
{
    public BeginStep(GameManager manager) : base(manager)
    {
        _stepType = GameStepType.BeginStep;
        _nextStepType = GameStepType.UntapStep;
    }
}
