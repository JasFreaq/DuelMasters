using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Parameters
{
    public class CardTargetingCondParam : EffectTargetingConditionParameter, ICardIntrinsicParam
    {
        #region Helper Data Structure(s)

        [System.Serializable]
        private class CardCondition
        {
            public CardData cardData;
            public ConnectorType connector;
        }

        #endregion

        [SerializeReference] private List<CardCondition> _cardConditions = new List<CardCondition>();

#if UNITY_EDITOR

        public override int CompareValue
        {
            get { return 0; }
        }

#endif

        public override bool IsConditionSatisfied(CardInstance cardInstToCheck)
        {
            bool result = true;
            CardData cardData = cardInstToCheck.CardData;

            for (int i = 0, n = _cardConditions.Count; i < n; i++)
            {
                CardCondition cardCondition = _cardConditions[i];

                bool res0 = cardData.Name == cardCondition.cardData.Name;
                result = result && res0;

                if (n > 1 && i < n - 1)
                {
                    if (cardCondition.connector == ConnectorType.And && !result)
                        break;
                    if (cardCondition.connector == ConnectorType.Or && result)
                        break;
                }
            }

            return result;
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            if (_cardConditions.Count > 0)
            {
                List<CardCondition> removedConditions = new List<CardCondition>();
                GUILayout.Label("Cards:", EditorStatics.EffectTargetingConditionParamLabelStyle);

                for (int i = 0, n = _cardConditions.Count; i < n; i++)
                {
                    CardCondition cardCondition = _cardConditions[i];

                    GUILayout.BeginHorizontal();

                    cardCondition.cardData =
                        (CardData) EditorGUILayout.ObjectField(cardCondition.cardData, typeof(CardData), false);
                    if (GUILayout.Button("Remove Card"))
                    {
                        removedConditions.Add(cardCondition);
                    }

                    GUILayout.EndHorizontal();

                    if (n > 1 && i < n - 1)
                        cardCondition.connector = EditorUtils.DrawFoldout(cardCondition.connector);
                }

                foreach (CardCondition cardCondition in removedConditions)
                {
                    _cardConditions.Remove(cardCondition);
                }
            }

            if (GUILayout.Button("Add Card"))
            {
                CardCondition cardCondition = new CardCondition();
                _cardConditions.Add(cardCondition);
            }
        }

        public override bool IsAssignedValue()
        {
            return _cardConditions.Count > 0;
        }

        public override string GetEditorRepresentationString()
        {
            return $"\nCard is{ToString()}";
        }
        
#endif

        public override string GetGameRepresentationString()
        {
            return ToString();
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0, n = _cardConditions.Count; i < n; i++)
            {
                CardCondition cardCondition = _cardConditions[i];
                str += $" {cardCondition.cardData.Name}";

                if (n > 1 && i < n - 1)
                    str += $" {_cardConditions[i].connector} ";
            }

            return str;
        }
    }
}