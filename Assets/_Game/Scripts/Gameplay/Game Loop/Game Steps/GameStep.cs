using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStep
{
    protected GameStepType _stepType = 0;
    protected GameStepType _nextStepType = 0;

    protected bool _updateStep = false;

    protected GameManager _gameManager = null;

    public GameStepType StepType
    {
        get { return _stepType; }
    }
    
    public GameStepType NextStepType
    {
        get { return _nextStepType; }
    }

    public bool UpdateStep
    {
        get { return _updateStep; }
    }

    protected GameStep(GameManager manager)
    {
        _gameManager = manager;
    }

    public virtual IEnumerator StartStepRoutine(PlayerManager currentPlayer)
    {
        yield break;
    }

    public virtual IEnumerator ProcessGameAction(CardObject cardObj, PlayerManager currentPlayer)
    {
        yield break;
    }

    public virtual IEnumerator FinishStepRoutine(PlayerManager currentPlayer)
    {
        yield break;
    }
}
