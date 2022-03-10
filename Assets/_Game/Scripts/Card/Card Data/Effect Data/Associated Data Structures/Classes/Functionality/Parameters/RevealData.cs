using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    [System.Serializable]
    public class RevealData
    {
        public CardZoneType lookAtZone;
        public NumericParamsHolder numericParams = new NumericParamsHolder();
        public RevealResultType revealResult;

        public override string ToString()
        {
            string str = "Reveal " + numericParams.CountQuantifier.ToString().ToLower();
            if (numericParams.CountRangeType == CountRangeType.All)
                str += "all cards";
            else
            {
                str += $"{numericParams.Count} card";
                if (numericParams.Count > 1)
                    str += "s";
            }

            str += " and " + revealResult.ToString().ToLower();

            return str;
        }
    }
}