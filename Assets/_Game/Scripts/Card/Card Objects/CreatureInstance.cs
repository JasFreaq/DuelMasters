using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureInstance : CardInstance
{
    private int _grantedPower = 0;

    public CreatureInstance(CardData cardData) : base(cardData) { }

    public new CreatureData CardData
    {
        get { return (CreatureData) _cardData; }
    }

    public int Power
    {
        get { return CardData.Power + _grantedPower; }
    }

    public int GrantedPower
    {
        get { return _grantedPower;}
        set { _grantedPower = value; }
    }

    public bool CanAttack
    {
        get { return !_isTapped; }
    }
}
