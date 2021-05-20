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
    
    public virtual void SetupCard(Card card)
    {
        _artworkImage.sprite = card.ArtworkImage;
        CardFrameData cardFrameData = _cardFrameDatabase.GetFrame(card.Civilization);
        _frameImage.sprite = cardFrameData.frameImage;
        _nameText.text = card.Name;
        _costText.text = card.Cost.ToString();
        _cardTypeTextTransform.localPosition = new Vector2(_cardTypeTextTransform.localPosition.x, cardFrameData.cardTypePosY);

        SetupRules(card.RulesText);

        if (!String.IsNullOrWhiteSpace(card.FlavorText)) 
        {
            FlavorTextLayoutHandler flavorTextLayout = Instantiate(flavorTextLayoutPrefab, _rulesPanel);
            flavorTextLayout.SetupFlavorText(card.FlavorText);
        }
    }

    protected virtual void SetupRules(string rulesText)
    {

    }
}
