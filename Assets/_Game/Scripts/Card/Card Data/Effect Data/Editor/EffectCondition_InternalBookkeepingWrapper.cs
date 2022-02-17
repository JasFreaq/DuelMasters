using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectCondition_InternalBookkeepingWrapper
{
    private static Dictionary<int, bool> ConnectToSubConditionDict = new Dictionary<int, bool>();

    public static void SetConnectToSubCondition(EffectCondition condition, bool connect)
    {
        ConnectToSubConditionDict[condition.GetHashCode()] = connect;
    }

    public static bool GetConnectToSubCondition(EffectCondition condition)
    {
        if (ConnectToSubConditionDict.ContainsKey(condition.GetHashCode()))
            return ConnectToSubConditionDict[condition.GetHashCode()];

        return false;
    }
}
