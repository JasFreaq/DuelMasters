using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller
{
    void Start()
    {
        _isPlayer = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            int iD = 0;
            PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(false);
            Dictionary<int, CreatureObject> battlers = dataHandler.CardsInBattle;
            foreach (KeyValuePair<int, CreatureObject> pair in battlers)
            {
                iD = pair.Key;
                _targetedCard = pair.Value;
                break;
            }

            ProcessInput(iD);
            _targetedCard = null;
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(false);
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
}
