using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller
{
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
                break;
            }

            ProcessInput(iD);
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            PlayerDataHandler dataHandler = GameDataHandler.Instance.GetDataHandler(false);
            List<CreatureObject> blockers = dataHandler.BlockersInBattle;
            int iD = blockers[0].transform.GetInstanceID();

            ProcessInput(iD);
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            SubmitSelection();
        }
    }
}
