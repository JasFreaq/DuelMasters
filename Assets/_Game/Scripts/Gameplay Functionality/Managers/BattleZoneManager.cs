using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleZoneManager : MonoBehaviour
{
    private BattleZoneLayoutHandler _battleZoneLayoutHandler;

    private void Awake()
    {
        _battleZoneLayoutHandler = GetComponent<BattleZoneLayoutHandler>();
    }

    public Transform AssignTempCard()
    {
        return _battleZoneLayoutHandler.AssignTempCard();
    }

    public void AddCard(Transform cardTransform)
    {
        _battleZoneLayoutHandler.AddCard(cardTransform);
    }

    public CreatureCardManager GetCardAtIndex(int index)
    {
        return _battleZoneLayoutHandler.GetCardAtIndex(index);
    }

    public CreatureCardManager RemoveCardAtIndex(int index)
    {
        return _battleZoneLayoutHandler.RemoveCardAtIndex(index);
    }
}
