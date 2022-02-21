using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.DuelMasters;
using UnityEngine;

namespace DuelMasters.Card.Data.Effects.TargetingCondition.Parameters
{
    public class KeywordConditionParam : EffectTargetingConditionParameter, ICardIntrinsicParam
    {
        #region Helper Data Structure(s)

        [System.Serializable]
        private class KeywordCondition
        {
            public bool non = false;
            public KeywordType keyword;
            public ConnectorType connector;
        }

        #endregion

        [SerializeReference] private List<KeywordCondition> _keywordConditions = new List<KeywordCondition>();

        public override bool IsConditionSatisfied(CardInstance cardInstToCheck)
        {
            bool result = true;
            CardData cardData = cardInstToCheck.CardData;

            if (_keywordConditions.Count > 0)
            {
                for (int i = 0, n = _keywordConditions.Count; i < n; i++)
                {
                    KeywordCondition keywordCondition = _keywordConditions[i];

                    bool res0 = false;
                    foreach (KeywordType keyword in cardData.Keywords)
                    {
                        if (keyword == keywordCondition.keyword)
                        {
                            res0 = true;
                            break;
                        }
                    }
                    if (keywordCondition.non)
                        res0 = !res0;

                    result = result && res0;

                    if (n > 1 && i < n - 1)
                    {
                        if (keywordCondition.connector == ConnectorType.And && !result)
                            break;
                        if (keywordCondition.connector == ConnectorType.Or && result)
                            break;
                    }
                }
            }

            return result;
        }

#if UNITY_EDITOR

        public override void DrawInspector()
        {
            if (_keywordConditions.Count > 0)
            {
                List<KeywordCondition> removedConditions = new List<KeywordCondition>();
                GUILayout.Label("Keywords:", EditorStatics.EffectTargetingConditionParamLabelStyle);

                for (int i = 0, n = _keywordConditions.Count; i < n; i++)
                {
                    KeywordCondition keywordCondition = _keywordConditions[i];

                    GUILayout.BeginHorizontal();

                    keywordCondition.non = GUILayout.Toggle(keywordCondition.non, "Non");
                    keywordCondition.keyword = EditorUtils.DrawFoldout(keywordCondition.keyword);
                    if (GUILayout.Button("Remove Keyword"))
                    {
                        removedConditions.Add(keywordCondition);
                    }

                    GUILayout.EndHorizontal();

                    if (n > 1 && i < n - 1)
                        keywordCondition.connector = EditorUtils.DrawFoldout(keywordCondition.connector);
                }

                foreach (KeywordCondition keywordCondition in removedConditions)
                {
                    _keywordConditions.Remove(keywordCondition);
                }
            }

            if (GUILayout.Button("Add Keyword"))
            {
                KeywordCondition keywordCondition = new KeywordCondition();
                _keywordConditions.Add(keywordCondition);
            }
        }

        public override bool IsAssignedValue()
        {
            return _keywordConditions.Count > 0;
        }

        public override string GetEditorRepresentationString()
        {
            return $"\nKeyword is{ToString()}";
        }

#endif

        public override string GetGameRepresentationString()
        {
            return ToString();
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0, n = _keywordConditions.Count; i < n; i++)
            {
                KeywordCondition keywordCondition = _keywordConditions[i];
                if (keywordCondition.non)
                    str += $" non-{keywordCondition.keyword}";
                else
                    str += $" {keywordCondition.keyword}";

                if (n > 1 && i < n - 1)
                    str += $" {_keywordConditions[i].connector} ";
            }

            return str;
        }
    }
}