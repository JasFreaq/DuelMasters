using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    [System.Serializable]
    public class DestroyData
    {
        public CardZoneType destroyZone;
        public NumericParamsHolder numericParams = new NumericParamsHolder();

        public override string ToString()
        {
            string str = "Destroy " + numericParams.CountQuantifier.ToString().ToLower();
            if (numericParams.CountRangeType == CountRangeType.All)
                str += "all cards";
            else
            {
                str += $"{numericParams.Count} card";
                if (numericParams.Count > 1)
                    str += "s ";
            }

            return str;
        }
    }
}