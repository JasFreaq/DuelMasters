using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Parameters
{
    [System.Serializable]
    public abstract class EffectTargetingConditionParameter
    {
        public abstract bool IsConditionSatisfied(CardInstance cardInstToCheck);

#if UNITY_EDITOR

        public abstract void DrawInspector();

        public abstract bool IsAssignedValue();

        public abstract string GetEditorRepresentationString();

#endif

        public abstract string GetGameRepresentationString();

        public abstract override string ToString();
    }
}