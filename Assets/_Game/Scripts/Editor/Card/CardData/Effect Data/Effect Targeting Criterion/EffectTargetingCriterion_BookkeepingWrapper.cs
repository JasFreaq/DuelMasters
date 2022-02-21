using System.Collections.Generic;
using DuelMasters.Card.Data.Effects.TargetingCondition.Data;

namespace DuelMasters.Editor.Data.InternalBookkeeping
{
    public static class EffectTargetingCriterion_BookkeepingWrapper
    {
        private static readonly string FILE_NAME = "EffectTargetingCriterionData";

        private static Dictionary<int, ParameterTargetingType> DataToTargetingDict =
            new Dictionary<int, ParameterTargetingType>();

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void Initialize()
        {
            DataToTargetingDict = SaveWrapper.Load<int, ParameterTargetingType>(FILE_NAME);
        }

        public static void SetTargetingType(EffectTargetingCriterion criterion, ParameterTargetingType targetingType)
        {
            DataToTargetingDict[criterion.GetHashCode()] = targetingType;

            SaveWrapper.Save(FILE_NAME, DataToTargetingDict);
        }

        public static ParameterTargetingType GetTargetingType(EffectTargetingCriterion criterion)
        {
            if (DataToTargetingDict.ContainsKey(criterion.GetHashCode()))
                return DataToTargetingDict[criterion.GetHashCode()];
            
            return ParameterTargetingType.Check;
        }

        public static void RemoveTargetingType(EffectTargetingCriterion criterion)
        {
            DataToTargetingDict.Remove(criterion.GetHashCode());

            SaveWrapper.Save(FILE_NAME, DataToTargetingDict);
        }
    }
}