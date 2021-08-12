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

    public void SetupRules(CreatureObject creatureObj, Transform rulesPanel)
    {
        CreatureData creatureData = creatureObj.CardData;
        if (!string.IsNullOrWhiteSpace(creatureData.RulesText)) 
        {
            string[] rules = creatureData.RulesText.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

            Type enumType = typeof(KeywordLayoutType);
            string[] keywords = Enum.GetNames(enumType);
            
            for (int i = 0, n = rules.Length; i < n; i++)
            {
                KeywordLayoutType type = KeywordLayoutType.Placeholder;

                for (int j = 0, m = keywords.Length; j < m; j++)
                {
                    string keywordStr = Regex.Replace(keywords[j], "([a-z])([A-Z])", "$1 $2");
                    keywordStr = keywordStr.Substring(0, 1) + keywordStr.Substring(1).ToLower();
                    KeywordLayoutType tempType = (KeywordLayoutType) Enum.Parse(enumType, keywords[j]);

                    if (Regex.Match(rules[i], @"\+[0-9]+").Success)
                        creatureObj.AddPlusToPower();
                    
                    if (rules[i].StartsWith(keywordStr))
                    {
                        type = tempType;
                        break;
                    }
                }

                KeywordLayoutHandler keywordLayout = Instantiate(_keywordPrefabDict[type], rulesPanel);

                int itr = 0;
                switch (type)
                {
                    case KeywordLayoutType.Placeholder:
                        keywordLayout.SetDescText(rules[i]);
                        break;

                    case KeywordLayoutType.Evolution:
                        string[] ruleStrings1 = new string[creatureData.Race.Length];
                        foreach (CardParams.Race race in creatureData.Race)
                        {
                            ruleStrings1[itr] = CardParams.PluralStringFromRace(race);
                            itr++;
                        }
                        keywordLayout.SetDescText(ruleStrings1);
                        break;

                    case KeywordLayoutType.PowerAttacker:
                        keywordLayout.SetDescText(Regex.Match(rules[i], @"[0-9]+").Value);
                        break;

                    case KeywordLayoutType.VortexEvolution:
                        CardParams.Race[] vortexRaces = creatureData.GetVortexEvolutionRaces();
                        string[] ruleStrings2 = new string[vortexRaces.Length];
                        foreach (CardParams.Race race in vortexRaces)
                        {
                            ruleStrings2[itr] = CardParams.PluralStringFromRace(race);
                            itr++;
                        }
                        keywordLayout.SetDescText(ruleStrings2);
                        break;
                }

                keywordLayout.SetDescDisplay();
            }
        }
    }
    
    public void SetupRules(SpellObject spellObj, Transform rulesPanel)
    {
        CardData cardData = spellObj.CardInst.CardData;
        if (!string.IsNullOrWhiteSpace(cardData.RulesText)) 
        {
            string[] rules = cardData.RulesText.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

            Type enumType = typeof(KeywordLayoutType);
            string[] keywords = Enum.GetNames(enumType);
            
            for (int i = 0, n = rules.Length; i < n; i++)
            {
                KeywordLayoutType type = KeywordLayoutType.Placeholder;

                for (int j = 0, m = keywords.Length; j < m; j++)
                {
                    string keywordStr = Regex.Replace(keywords[j], "([a-z])([A-Z])", "$1 $2");
                    keywordStr = keywordStr.Substring(0, 1) + keywordStr.Substring(1).ToLower();
                    KeywordLayoutType tempType = (KeywordLayoutType) Enum.Parse(enumType, keywords[j]);

                    if (tempType == KeywordLayoutType.ShieldTrigger && rules[i].StartsWith(keywordStr))
                    {
                        type = tempType;
                        break;
                    }
                }

                KeywordLayoutHandler keywordLayout = Instantiate(_keywordPrefabDict[type], rulesPanel);

                switch (type)
                {
                    case KeywordLayoutType.Placeholder:
                        keywordLayout.SetDescText(rules[i]);
                        break;
                }

                keywordLayout.SetDescDisplay();
            }
        }
    }
}
