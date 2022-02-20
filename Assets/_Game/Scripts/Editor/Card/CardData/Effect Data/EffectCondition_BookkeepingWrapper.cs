using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Editor.Data.InternalBookkeeping
{
    public static class EffectCondition_BookkeepingWrapper
    {
        private static readonly string FILE_NAME = "EffectConditionData";

        private static Dictionary<int, bool> ConnectToSubConditionDict = new Dictionary<int, bool>();

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void Initialize()
        {
            ConnectToSubConditionDict = SaveWrapper.Load<int, bool>(FILE_NAME);
        }

        public static void SetConnectToSubCondition(EffectCondition condition, bool connect)
        {
            ConnectToSubConditionDict[condition.GetHashCode()] = connect;
            SaveWrapper.Save(FILE_NAME, ConnectToSubConditionDict);
        }

        public static bool GetConnectToSubCondition(EffectCondition condition)
        {
            if (ConnectToSubConditionDict.ContainsKey(condition.GetHashCode()))
                return ConnectToSubConditionDict[condition.GetHashCode()];
            
            return false;
        }
    }
}