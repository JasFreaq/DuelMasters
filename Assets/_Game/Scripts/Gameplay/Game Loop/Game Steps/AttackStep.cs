using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStep : GameStep
{
    public AttackStep(GameManager manager) : base(manager)
    {
        _stepType = GameStepType.AttackStep;
        _nextStepType = GameStepType.EndStep;
        _updateStep = true;
    }
}
