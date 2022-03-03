using System.Collections.Generic;

namespace DuelMasters.Card.Data.Effects.Functionality.Parameters
{
    [System.Serializable]
    public abstract class EffectFunctionalityParameter
    {
#if UNITY_EDITOR

        public abstract IReadOnlyList<EffectFunctionalityType> FunctionalityTypes { get; }

        public abstract void DrawInspector();

        public abstract bool ShouldAssignCriterion();

#endif
    }
}