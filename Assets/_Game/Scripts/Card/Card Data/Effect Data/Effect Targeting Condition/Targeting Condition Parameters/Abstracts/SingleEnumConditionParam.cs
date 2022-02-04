using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleEnumConditionParam : EffectTargetingConditionParameter
{
    [SerializeReference] protected bool _assignedParameter;

    public override bool IsAssignedValue()
    {
        return _assignedParameter;
    }
}
