using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    public abstract class MultipliableFuncParam : EffectFunctionalityParameter
    {
        private bool _shouldMultiplyVal;

        public bool ShouldMultiplyVal
        {
            get { return _shouldMultiplyVal; }
        }

        public bool DrawShouldMultiplyVal()
        {
            bool initial = _shouldMultiplyVal;
            _shouldMultiplyVal = GUILayout.Toggle(_shouldMultiplyVal, "Multiply val");

            return initial != _shouldMultiplyVal;
        }
    }
}