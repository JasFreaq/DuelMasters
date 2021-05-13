using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private FlavorTextLayoutHandler flavorTextLayoutPrefab;
    [SerializeField] protected GameObject _glowFrame;
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

        if (!String.IsNullOrWhiteSpace(cardData.FlavorText)) 
        {
            FlavorTextLayoutHandler flavorTextLayout = Instantiate(flavorTextLayoutPrefab, _rulesPanel);
            flavorTextLayout.SetupFlavorText(cardData.FlavorText);
        }
    }

    protected virtual void SetupRules(string rulesText)
    {

    }
}
