using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tester : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0)) 
            Test();
    }

    private void Test()
    {
        MovementZones movementZones = new MovementZones
        {
            fromZone = CardZoneType.Deck,
            toZone = CardZoneType.ManaZone,
            deckCardMove = DeckCardMoveType.SearchShuffle,
            countChoice = CountChoiceType.Upto,
            moveCount = 4
        };

        EffectTargetingCondition targetingCondition = new EffectTargetingCondition();
        targetingCondition.AddPowerCondition(new PowerCondition
        {
            comparator = ComparisonType.GreaterThan,
            power = 1000
        });

        GameManager.Instance.ProcessRegionMovement(true, movementZones, targetingCondition);
    }
}
