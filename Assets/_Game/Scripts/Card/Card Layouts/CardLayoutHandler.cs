using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public abstract class CardLayoutHandler : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _artworkImage;
    [SerializeField] private Image _frameImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private RectTransform _cardTypeTextTransform;
    [SerializeField] protected Transform _rulesPanel;
    [SerializeField] protected Image _highlightFrame;
    [SerializeField] CardFrameDatabase _cardFrameDatabase;
    
    public Canvas Canvas
    {
        get { return _canvas; }
    }

    public virtual void SetupCard(CardData cardData)
    {
        _artworkImage.sprite = cardData.ArtworkImage;
        CardFrameData cardFrameData = _cardFrameDatabase.GetFrame(cardData.Civilization);
        _frameImage.sprite = cardFrameData.frameImage;
        _nameText.text = cardData.Name;
        _costText.text = cardData.Cost.ToString();
        _cardTypeTextTransform.localPosition = new Vector2(_cardTypeTextTransform.localPosition.x, cardFrameData.cardTypePosY);
    }

    protected void SetupRulesArea(CardData cardData, bool isCreature)
    {
        SetupRules(cardData.RulesText, isCreature, _rulesPanel);
        SetupFlavorText(cardData.FlavorText);
    }

    public void SetGlow(bool enableGlow)
    {
        _highlightFrame.gameObject.SetActive(enableGlow);
    }
    
    public void SetHighlightColor(Color color)
    {
        _highlightFrame.color = color;
    }

    #region Static Methods

    private static void SetupRules(string rulesText, bool isCreature, Transform panel)
    {
        string[] rules = rulesText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        Type enumType = typeof(KeywordLayoutType);
        string[] keywords = Enum.GetNames(enumType);

        for (int i = 0, n = rules.Length; i < n; i++)
        {
            string rule = rules[i].ToLower();
            rule = Regex.Replace(rule, @"\s", "");
            
            KeywordLayoutType type = KeywordLayoutType.Placeholder;

            for (int j = 0, m = keywords.Length; j < m; j++)
            {
                string keyword = keywords[i].ToLower();
                KeywordLayoutType tempType = (KeywordLayoutType) Enum.Parse(enumType, keywords[i]);

                if (isCreature || tempType == KeywordLayoutType.ShieldTrigger ||
                    tempType == KeywordLayoutType.Placeholder)
                {
                    if (rule.Contains(keyword))
                    {
                        type = tempType;
                        break;
                    }
                }
            }

            KeywordLayoutHandler keywordLayoutPrefab = KeywordPrefabHolder.Instance.KeywordPrefabDict[type];

            if (type == KeywordLayoutType.Placeholder)
                keywordLayoutPrefab.SetDescText(rules[i]);

            KeywordLayoutHandler keywordLayout = Instantiate(keywordLayoutPrefab, panel);
            keywordLayout.SetDescDisplay();
        }
    }

    public void SetupFlavorText(string flavorText)
    {
        if (!String.IsNullOrWhiteSpace(flavorText))
        {
            FlavorTextLayoutHandler flavorTextLayout =
                Instantiate(GameParamsHolder.Instance.FlavorTextLayoutPrefab, _rulesPanel);
            flavorTextLayout.SetupFlavorText(flavorText);
        }
    }

    #endregion
}
