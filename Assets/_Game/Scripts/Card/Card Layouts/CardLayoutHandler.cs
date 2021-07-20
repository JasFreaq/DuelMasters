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

        SetupRules(cardData.RulesText);
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

    #region Rules Panel Methods

    private void SetupRules(string rulesText)
    {
        string[] rules = rulesText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        Type enumType = typeof(KeywordLayoutType);
        string[] keywords = Enum.GetNames(enumType);

        for (int i = 0, n = rules.Length; i < n; i++)
        {
            SetupRule(rules[i], enumType, keywords);
        }
    }

    private void SetupRule(string rule, Type enumType, string[] keywords)
    {
        string ruleStr = rule.ToLower();
        ruleStr = Regex.Replace(ruleStr, @"\s", "");

        KeywordLayoutType type = KeywordLayoutType.Placeholder;

        for (int j = 0, m = keywords.Length; j < m; j++)
        {
            string keywordStr = keywords[j].ToLower();
            KeywordLayoutType tempType = (KeywordLayoutType)Enum.Parse(enumType, keywords[j]);

            bool isCreature = this is CreatureLayoutHandler;
            if (isCreature || tempType == KeywordLayoutType.ShieldTrigger)
            {
                if (isCreature && Regex.Match(rule, @"\+[0-9]+").Success)
                    ((CreatureLayoutHandler)this).AddPlusToPower();

                if (ruleStr.Contains(keywordStr))
                {
                    type = tempType;
                    break;
                }
            }
        }

        KeywordLayoutHandler keywordLayoutPrefab = KeywordPrefabHolder.Instance.KeywordPrefabDict[type];
        KeywordLayoutHandler keywordLayout = Instantiate(keywordLayoutPrefab, _rulesPanel);

        switch (type)
        {
            case KeywordLayoutType.Placeholder:
                keywordLayout.SetDescText(rule);
                break;

            case KeywordLayoutType.PowerAttacker:
                keywordLayout.SetDescText(Regex.Match(rule, @"[0-9]+").Value);
                break;
        }
        keywordLayout.SetDescDisplay();
    }

    private void SetupFlavorText(string flavorText)
    {
        if (!string.IsNullOrWhiteSpace(flavorText))
        {
            FlavorTextLayoutHandler flavorTextLayout =
                Instantiate(GameParamsHolder.Instance.FlavorTextLayoutPrefab, _rulesPanel);
            flavorTextLayout.SetupFlavorText(flavorText);
        }
    }

    #endregion
}
