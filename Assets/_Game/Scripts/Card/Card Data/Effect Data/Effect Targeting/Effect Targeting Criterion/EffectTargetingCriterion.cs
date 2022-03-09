using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Data
{
    [System.Serializable]
    public class EffectTargetingCriterion
    {
        private NumericParamsHolder _numericParams = new NumericParamsHolder();
        private PlayerTargetType _owningPlayer;
        private CardZoneType _zoneType;
        
        private bool _includeSelf;
        private bool _opponentChooses;
        
#if UNITY_EDITOR
        #region Editor Only Members
        
        public ParameterTargetingType targetingType;

        #endregion
#endif

        public NumericParamsHolder NumericParams
        {
            get { return _numericParams; }

#if UNITY_EDITOR
            set { _numericParams = value; }
#endif
        }

        public PlayerTargetType OwningPlayer
        {
            get { return _owningPlayer; }

#if UNITY_EDITOR
            set { _owningPlayer = value; }
#endif
        }

        public CardZoneType ZoneType
        {
            get { return _zoneType; }

#if UNITY_EDITOR
            set { _zoneType = value; }
#endif
        }

        public bool IncludeSelf
        {
            get { return _includeSelf; }

#if UNITY_EDITOR
            set { _includeSelf = value; }
#endif
        }

        public bool OpponentChooses
        {
            get { return _opponentChooses; }

#if UNITY_EDITOR
            set { _opponentChooses = value; }
#endif
        }
    }
}