using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class KeywordPrefabHandler : MonoBehaviour
{
    [SerializeField] private List<KeywordLayoutHandler> _keywordLayoutPrefabs = new List<KeywordLayoutHandler>();

    private Dictionary<KeywordLayoutType, KeywordLayoutHandler> _keywordPrefabDict = new Dictionary<KeywordLayoutType, KeywordLayoutHandler>();

    #region Static Data Members

    private static KeywordPrefabHandler _Instance = null;

    public static KeywordPrefabHandler Instance
    {
        get
        {
            if (!_Instance)
                _Instance = FindObjectOfType<KeywordPrefabHandler>();
            return _Instance;
        }
    }

    #endregion
    
    private void Awake()
    {
        int count = FindObjectsOfType<KeywordPrefabHandler>().Length;
        if (count > 1)
            Destroy(gameObject);
        else
            _Instance = this;

        foreach (KeywordLayoutHandler keyword in _keywordLayoutPrefabs)
        {
            _keywordPrefabDict[keyword.Type] = keyword;
        }
    }

    public void SetupRules(CardData cardData, CardLayoutHandler layoutHandler, Transform rulesPanel)
    {
        if (!string.IsNullOrWhiteSpace(cardData.RulesText)) 
        {
            string[] rules = cardData.RulesText.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

            Type enumType = typeof(KeywordLayoutType);
            string[] keywords = Enum.GetNames(enumType);

            for (int i = 0, n = rules.Length; i < n; i++)
            {
                string ruleStr = rules[i].ToLower();
                ruleStr = Regex.Replace(ruleStr, @"\s", "");

                KeywordLayoutType type = KeywordLayoutType.Placeholder;

                for (int j = 0, m = keywords.Length; j < m; j++)
                {
                    string keywordStr = Regex.Replace(keywords[j], "([a-z])([A-Z])", "$1 $2");
                    keywordStr = keywordStr.Substring(0, 1) + keywordStr.Substring(1).ToLower();
                    KeywordLayoutType tempType = (KeywordLayoutType) Enum.Parse(enumType, keywords[j]);

                    bool isCreature = cardData is CreatureData;
                    if (isCreature || tempType == KeywordLayoutType.ShieldTrigger)
                    {
                        if (isCreature && Regex.Match(rules[i], @"\+[0-9]+").Success)
                            ((CreatureLayoutHandler) layoutHandler).AddPlusToPower();

                        //if (ruleStr.Contains(keywordStr))
                        //{
                        //    type = tempType;
                        //    break;
                        //}

                        if (rules[i].StartsWith(keywordStr))
                        {
                            type = tempType;
                            break;
                        }
                    }
                }

                KeywordLayoutHandler keywordLayoutPrefab = _keywordPrefabDict[type];
                KeywordLayoutHandler keywordLayout = Instantiate(keywordLayoutPrefab, rulesPanel);

                switch (type)
                {
                    case KeywordLayoutType.Placeholder:
                        keywordLayout.SetDescText(rules[i]);
                        break;

                    case KeywordLayoutType.PowerAttacker:
                        keywordLayout.SetDescText(Regex.Match(rules[i], @"[0-9]+").Value);
                        break;
                }

                keywordLayout.SetDescDisplay();
            }
        }
    }
}
