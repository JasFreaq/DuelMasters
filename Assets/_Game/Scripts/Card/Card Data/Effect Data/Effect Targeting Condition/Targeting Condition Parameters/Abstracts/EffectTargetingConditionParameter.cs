using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EffectTargetingConditionParameter
{
    public abstract bool IsConditionSatisfied(CardInstance cardInstToCheck);

#if UNITY_EDITOR

    public abstract void DrawParamInspector();

    public abstract bool IsAssignedValue();

    public abstract string GetEditorRepresentationString();

#endif

    public abstract string GetGameRepresentationString();

    public abstract override string ToString();
}
