using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CardFrameDatabase))] [DisallowMultipleComponent]
public abstract class CardObject : MonoBehaviour
{
    [SerializeField] private Image _artworkImage;
    [SerializeField] private Image _frameImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private RectTransform _cardTypeTextTransform;
    [SerializeField] protected Transform _rulesPanel;
    [SerializeField] private Transform _previewHolder;
    [SerializeField] private FlavorTextObject _flavorTextPrefab;
    [SerializeField] private GameObject _canvas;
    [SerializeField] protected GameObject _glowFrame;

    private CardFrameDatabase _cardFrameDatabase;

    private void Awake()
    {
        _cardFrameDatabase = GetComponent<CardFrameDatabase>();
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
            FlavorTextObject flavorText = Instantiate(_flavorTextPrefab, _rulesPanel);
            flavorText.SetupFlavorText(cardData.FlavorText);
        }

        GameObject previewDuplicate = Instantiate(_canvas, _previewHolder);
    }

    protected virtual void SetupRules(string rulesText)
    {

    }
}
