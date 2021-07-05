using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectData : ScriptableObject
{
    public EffectCondition effectCondition = new EffectCondition();
    public EffectFunctionality effectFunctionality = new EffectFunctionality();

#if UNITY_EDITOR
    public bool isBeingEdited = false;
#endif

    public override string ToString()
    {
        return $"{effectCondition}\n{effectFunctionality}";
    }
}
