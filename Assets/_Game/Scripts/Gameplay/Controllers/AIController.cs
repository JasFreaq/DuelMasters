using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller
{
    private int _numSelection = 0, _lowerBound, _upperBound;
    private bool _numSelectionMade;

    void Start()
    {
        _isPlayer = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(false);
            Dictionary<int, CreatureObject> battlers = dataHandler.CardsInBattle;
            foreach (KeyValuePair<int, CreatureObject> pair in battlers)
            {
                SelectCard(pair.Value);
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(true);
            ShieldObject shield = dataHandler.Shields[0];
            _targetedShield = shield;
            ProcessInput(shield.GetInstanceID());
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(false);
            List<CreatureObject> blockers = dataHandler.BlockersInBattle;

            _targetedCard = blockers[0];
            int iD = blockers[0].transform.GetInstanceID();
            ProcessInput(iD);
        }

        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            SubmitSelection();
        }
    }

    #region Number Selection Methods

    public override IEnumerator GetNumberSelectionRoutine(int lower, int upper)
    {
        _numSelection = _lowerBound = lower;
        _upperBound = upper;

        while (!_numSelectionMade)
        {
            yield return new WaitForEndOfFrame();
        }

        _numSelectionMade = false;
        yield return _numSelection;
    }

    public void DecreaseSelection()
    {
        _numSelection = Mathf.Max(_numSelection - 1, _lowerBound);
    }

    public void IncreaseSelection()
    {
        _numSelection = Mathf.Min(_numSelection + 1, _upperBound);
    }

    public void MakeSelection()
    {
        _numSelectionMade = true;
    }

    #endregion
}
