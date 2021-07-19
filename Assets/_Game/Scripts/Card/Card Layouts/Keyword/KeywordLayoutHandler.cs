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
    [SerializeField] private GameObject _subDescHolder;

    public KeywordLayoutType Type
    {
        get { return _type; }
    }
    
    public void SetDescText(string rulesText)
    {
        if (_type == KeywordLayoutType.Placeholder)
            _descText.text = BulletPointString + rulesText;
    }

    public void SetDescDisplay(bool showShortDesc = false)
    {
        if (_type != KeywordLayoutType.Placeholder) 
        {
            _descText.gameObject.SetActive(!showShortDesc);
            _shortDescText.gameObject.SetActive(!showShortDesc);
        }
    }
}
