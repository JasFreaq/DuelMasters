using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public abstract class CardObject : MonoBehaviour
{
    [SerializeField] private CardObject _previewCard;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _artworkImage;
    [SerializeField] private Image _frameImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private RectTransform _cardTypeTextTransform;
    [SerializeField] protected Transform _rulesPanel;
    [SerializeField] private FlavorTextObject _flavorTextPrefab;
    [SerializeField] protected GameObject _glowFrame;
    [SerializeField] CardFrameDatabase _cardFrameDatabase;

    private CardData _cardData;

    public Canvas Canvas
    {
        get { return _canvas; }
    }

    public CardData CardData
    {
        get { return _cardData; }
    }

    public virtual void SetupCard(CardData cardData)
    {
        _cardData = cardData;

        _artworkImage.sprite = cardData.ArtworkImage;
        CardFrameData cardFrameData = _cardFrameDatabase.GetFrame(cardData.Civilization);
        _frameImage.sprite = cardFrameData.frameImage;
        _nameText.text = cardData.Name;
        _costText.text = cardData.Cost.ToString();
        _cardTypeTextTransform.localPosition = new Vector2(_cardTypeTextTransform.localPosition.x, cardFrameData.cardTypePosY);

        SetupRules(cardData.RulesText);

        if (!String.IsNullOrWhiteSpace(cardData.FlavorText)) 
        {
            FlavorTextObject flavorText = Instantiate(_flavorTextPrefab, _rulesPanel);
            flavorText.SetupFlavorText(cardData.FlavorText);
        }

        if (_previewCard)
        {
            _previewCard.SetupCard(cardData);
        }
    }

    protected virtual void SetupRules(string rulesText)
    {

    }
}
