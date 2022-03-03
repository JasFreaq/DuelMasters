using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Parameters
{
    public abstract class SingleEnumTargetingCondParam : EffectTargetingConditionParameter
    {
#if UNITY_EDITOR

        [SerializeReference] protected bool _assignedParameter;

        public override bool IsAssignedValue()
        {
            return _assignedParameter;
        }

#endif
    }
}