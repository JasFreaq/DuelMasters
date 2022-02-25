using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Parameters
{
    public class PowerConditionParam : EffectTargetingConditionParameter, ICardIntrinsicParam
    {
        #region Helper Data Structure(s)

        [System.Serializable]
        private enum ComparisonType
        {
            LessThan,
            GreaterThan,
            EqualTo,
            LessThanOrEqualTo,
            GreaterThanOrEqualTo
        }

        [System.Serializable]
        private class PowerCondition
        {
            public ComparisonType comparator;
            public int power;
            public ConnectorType connector;
        }

        #endregion

        [SerializeReference] private List<PowerCondition> _powerConditions = new List<PowerCondition>();

#if UNITY_EDITOR

        public override int CompareValue
        {
            get { return 4; }
        }

#endif

        public override bool IsConditionSatisfied(CardInstance cardInstToCheck)
        {
            bool result = true;
            CardData cardData = cardInstToCheck.CardData;

            if (_powerConditions.Count > 0)
            {
                if (cardData is CreatureData creatureData)
                {
                    for (int i = 0, n = _powerConditions.Count; i < n; i++)
                    {
                        PowerCondition powerCondition = _powerConditions[i];

                        bool res0 = false;
                        switch (powerCondition.comparator)
                        {
                            case ComparisonType.LessThan:
                                res0 = creatureData.Power < powerCondition.power;
                                break;

                            case ComparisonType.GreaterThan:
                                res0 = creatureData.Power > powerCondition.power;
                                break;

                            case ComparisonType.EqualTo:
                                res0 = creatureData.Power == powerCondition.power;
                                break;

                            case ComparisonType.LessThanOrEqualTo:
                                res0 = creatureData.Power <= powerCondition.power;
                                break;

                            case ComparisonType.GreaterThanOrEqualTo:
                                res0 = creatureData.Power >= powerCondition.power;
                                break;
                        }

                        result = result && res0;

                        if (n > 1 && i < n - 1)
                        {
                            if (powerCondition.connector == ConnectorType.And && !result)
                                break;
                            if (powerCondition.connector == ConnectorType.Or && result)
                                break;
                        }
                    }
                }
                else
                    result = false;
            }

            return result;
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            if (_powerConditions.Count > 0)
            {
                List<PowerCondition> removedConditions = new List<PowerCondition>();
                GUILayout.Label("Powers:", EditorStatics.EffectTargetingConditionParamLabelStyle);

                for (int i = 0, n = _powerConditions.Count; i < n; i++)
                {
                    PowerCondition powerCondition = _powerConditions[i];

                    GUILayout.BeginHorizontal();

                    powerCondition.comparator = EditorUtils.DrawFoldout(powerCondition.comparator);
                    powerCondition.power = EditorGUILayout.IntField(powerCondition.power);
                    if (GUILayout.Button("Remove Power"))
                    {
                        removedConditions.Add(powerCondition);
                    }

                    GUILayout.EndHorizontal();

                    if (n > 1 && i < n - 1)
                        powerCondition.connector = EditorUtils.DrawFoldout(powerCondition.connector);
                }

                foreach (PowerCondition powerCondition in removedConditions)
                {
                    _powerConditions.Remove(powerCondition);
                }
            }

            if (GUILayout.Button("Add Power"))
            {
                PowerCondition powerCondition = new PowerCondition();
                _powerConditions.Add(powerCondition);
            }
        }

        public override bool IsAssignedValue()
        {
            return _powerConditions.Count > 0;
        }

        public override string GetEditorRepresentationString()
        {
            return $"\nPower is{ToString()}";
        }

#endif

        public override string GetGameRepresentationString()
        {
            return $"creature with power {ToString()}";
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0, n = _powerConditions.Count; i < n; i++)
            {
                PowerCondition powerCondition = _powerConditions[i];
                str += $" {powerCondition.comparator}";
                str += $" {powerCondition.power}";

                if (n > 1 && i < n - 1)
                    str += $" {_powerConditions[i].connector} ";
            }

            return str;
        }
    }
}