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

    public void SetDescText(string rulesText)
    {
        switch (_type)
        {
            case KeywordLayoutType.Placeholder:
                _descText.text = $"<indent=50>\u2022 {rulesText}</indent>";
                break;

            case KeywordLayoutType.PowerAttacker:
                _descText.text = $"<indent=50>\u2022 Power Attacker +{rulesText} (While attacking, this creature gets +{rulesText} power.)</indent>";
                _shortDescText.text = $"<space=50>\u2022 Power Attacker +{rulesText}";
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
