using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    [System.Serializable]
    public class MovementZones
    {
        public CardZoneType fromZone, toZone;
        public bool showSearchedCard;

        public NumericParamsHolder numericParams = new NumericParamsHolder();
    }
}