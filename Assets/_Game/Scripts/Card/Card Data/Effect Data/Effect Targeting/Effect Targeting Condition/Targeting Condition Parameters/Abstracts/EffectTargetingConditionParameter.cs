using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Parameters
{
    [System.Serializable]
    public abstract class EffectTargetingConditionParameter
#if UNITY_EDITOR
        : IComparable<EffectTargetingConditionParameter>
#endif
    {

#if UNITY_EDITOR

        public abstract int CompareValue { get; }

#endif

        public abstract bool IsConditionSatisfied(CardInstance cardInstToCheck);

#if UNITY_EDITOR

        public abstract void DrawInspector();

        public abstract bool IsAssignedValue();

        public abstract string GetEditorRepresentationString();

        public int CompareTo(EffectTargetingConditionParameter other)
        {
            return CompareValue.CompareTo(other.CompareValue);
        }

#endif

        public abstract string GetGameRepresentationString();
        
        public abstract override string ToString();
    }
}