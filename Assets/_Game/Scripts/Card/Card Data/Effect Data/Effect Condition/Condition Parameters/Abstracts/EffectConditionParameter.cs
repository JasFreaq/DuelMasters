namespace DuelMasters.Card.Data.Effects.Condition.Parameters
{
    [System.Serializable]
    public abstract class EffectConditionParameter
    {
#if UNITY_EDITOR

        public abstract EffectConditionType ConditionType { get; }

        public abstract void DrawInspector();

#endif
    }
}