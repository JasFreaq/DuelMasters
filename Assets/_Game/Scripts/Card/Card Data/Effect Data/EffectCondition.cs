using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectCondition
{
    private bool _isAssigned;
    private EffectConditionType _type;
    private EffectTargetingCondition _targetingCondition = new EffectTargetingCondition();
    private bool _mayUse;

    public bool IsAssigned
    {
        get { return _isAssigned; }

#if UNITY_EDITOR
        set { _isAssigned = value; }
#endif
    }

    public EffectConditionType Type
    {
        get { return _type; }

#if UNITY_EDITOR
        set { _type = value; }
#endif
    }

    public EffectTargetingCondition TargetingCondition
    {
        get { return _targetingCondition; }

#if UNITY_EDITOR
        set { _targetingCondition = value; }
#endif
    }

    public bool MayUse
    {
        get { return _mayUse; }

#if UNITY_EDITOR
        set { _mayUse = value; }
#endif
    }

    public override string ToString()
    {
        if (IsAssigned)
        {
            string str = $"{_type} ";
            if (_mayUse)
                str += "may ";
            str += _targetingCondition.ToString();
            return str;
        }

        return "Condition unassigned.";
    }
}
