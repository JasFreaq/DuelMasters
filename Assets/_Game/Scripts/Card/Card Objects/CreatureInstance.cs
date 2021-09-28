using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureInstance : CardInstance
{
    public CreatureInstance(CardData cardData) : base(cardData) { }

    public new CreatureData CardData
    {
        get { return (CreatureData) _cardData; }
    }

    public bool CanAttack()
    {
        return !_isTapped;
    }
}
