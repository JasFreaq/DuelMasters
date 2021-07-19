using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellLayoutHandler : CardLayoutHandler
{
    public override void SetupCard(CardData cardData)
    {
        base.SetupCard(cardData);
        SetupRulesArea(cardData, false);
    }
}
