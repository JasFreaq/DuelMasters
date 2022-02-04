using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public static class EditorStatics 
{
    private static GUIStyle _effectTargetingConditionParamLabelStyle = new GUIStyle { fontStyle = FontStyle.Italic, fontSize = 12 };

    public static GUIStyle EffectTargetingConditionParamLabelStyle { get { return _effectTargetingConditionParamLabelStyle; } }
}
