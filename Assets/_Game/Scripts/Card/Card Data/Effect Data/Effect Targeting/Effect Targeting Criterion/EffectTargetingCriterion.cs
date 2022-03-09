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
        private CountRangeType _countRangeType;
        private CountQuantifierType _countQuantifier;
        private int _count;
        private PlayerTargetType _owningPlayer;
        private CardZoneType _zoneType;
        
        private bool _includeSelf;
        private bool _opponentChooses;
        
#if UNITY_EDITOR
        #region Editor Only Members
        
        public ParameterTargetingType targetingType;

        #endregion
#endif

        public CountRangeType CountRangeType
        {
            get { return _countRangeType; }

#if UNITY_EDITOR
            set { _countRangeType = value; }
#endif
        }

        public CountQuantifierType CountQuantifier
        {
            get { return _countQuantifier; }

#if UNITY_EDITOR
            set { _countQuantifier = value; }
#endif
        }

        public int Count
        {
            get { return _count; }

#if UNITY_EDITOR
            set { _count = value; }
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