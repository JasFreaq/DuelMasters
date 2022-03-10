using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    [System.Serializable]
    public class DiscardData
    {
        public DiscardType discardType;
        public NumericParamsHolder numericParams = new NumericParamsHolder();

        public override string ToString()
        {
            string str = "discards " + numericParams.CountQuantifier.ToString().ToLower();
            if (numericParams.CountRangeType == CountRangeType.All)
                str += "all cards";
            else
            {
                str += $"{numericParams.Count} card";
                if (numericParams.Count > 1)
                    str += "s ";

                switch (discardType)
                {
                    case DiscardType.Random:
                        str += "at random";
                        break;
                    case DiscardType.PlayerChoose:
                        str += "chosen by player";
                        break;
                    case DiscardType.OpponentChoose:
                        str += "chosen by opponent";
                        break;
                }
            }

            return str;
        }
    }
}