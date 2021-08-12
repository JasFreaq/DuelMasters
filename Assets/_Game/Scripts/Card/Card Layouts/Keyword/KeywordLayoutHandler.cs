using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeywordLayoutHandler : MonoBehaviour
{
    private static string BulletPointString = "<space=50>\u2022 ";

    [SerializeField] private KeywordLayoutType _type;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private TextMeshProUGUI _shortDescText;
    [SerializeField] private bool _includesSubDesc;

    [HideInInspector] public Transform subDescHolder;

    public KeywordLayoutType Type
    {
        get { return _type; }
    }

    public bool IncludesSubDesc
    {
        get { return _includesSubDesc; }
    }

    public void SetDescText(string ruleStr)
    {
        switch (_type)
        {
            case KeywordLayoutType.Placeholder:
                _descText.text = $"<indent=50>\u2022 {ruleStr}</indent>";
                break;

            case KeywordLayoutType.PowerAttacker:
                _descText.text = $"<indent=50>\u2022 Power Attacker +{ruleStr} (While attacking, this creature gets +{ruleStr} power.)</indent>";
                _shortDescText.text = $"<space=50>\u2022 Power Attacker +{ruleStr}";
                break;
        }
    }
    
    public void SetDescText(string[] ruleStrings)
    {
        switch (_type)
        {
            case KeywordLayoutType.Evolution:
                string ruleStr1 = ruleStrings[0];
                if (ruleStrings.Length > 1)
                {
                    int n1 = ruleStrings.Length;
                    for (int i = 1; i < n1 - 1; i++)
                    {
                        ruleStr1 += $", {ruleStrings[i]}";
                    }

                    ruleStr1 += $"or {ruleStrings[n1 - 1]}";
                }
                _descText.text = $"<indent=50>\u2022 Evolution - Put on one of your {ruleStr1}";
                _shortDescText.text = $"<indent=50>\u2022 Evolution - {ruleStr1}";
                break;

            case KeywordLayoutType.VortexEvolution:
                string ruleStr2 = ruleStrings[0], shortRuleStr2;
                int n2 = ruleStrings.Length;
                for (int i = 1; i < n2 - 1; i++)
                {
                    ruleStr2 += $", {ruleStrings[i]}";
                }

                shortRuleStr2 = ruleStr2;
                ruleStr2 += $"and one of your {ruleStrings[n2 - 1]}";
                shortRuleStr2 += $"and {ruleStrings[n2 - 1]}";
                
                _descText.text = $"<space=50>\u2022 Vortex Evolution - Put on one of your {ruleStr2}";
                _shortDescText.text = $"<indent=50>\u2022 Vortex Evolution - {shortRuleStr2}";
                break;
        }
    }

    public void SetDescDisplay(bool showShortDesc = false)
    {
        if (_type != KeywordLayoutType.Placeholder) 
        {
            _descText.gameObject.SetActive(!showShortDesc);
            _shortDescText.gameObject.SetActive(showShortDesc);
        }
    }
}
