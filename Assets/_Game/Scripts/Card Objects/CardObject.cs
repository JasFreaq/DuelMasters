using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FrameDatabase))] [DisallowMultipleComponent]
public abstract class CardObject : MonoBehaviour
{
    [SerializeField] private Image _artworkImage;
    [SerializeField] private Image _frameImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private RectTransform _cardTypeTextTransform;
    [SerializeField] protected Transform _rulesPanel;
    [SerializeField] private FlavorTextObject _flavorTextPrefab;

    private FrameDatabase _frameDatabase;

    private void Awake()
    {
        _frameDatabase = GetComponent<FrameDatabase>();
    }

    public virtual void SetupCard(Card card)
    {
        _artworkImage.sprite = card.ArtworkImage;
        FrameData frameData = _frameDatabase.GetFrame(card.Civilization);
        _frameImage.sprite = frameData.frameImage;
        _nameText.text = card.Name;
        _costText.text = card.Cost.ToString();
        _cardTypeTextTransform.position = new Vector2(_cardTypeTextTransform.position.x, frameData.cardTypePosY);

        SetupRules(card.RulesText);

        if (!String.IsNullOrWhiteSpace(card.FlavorText)) 
        {
            FlavorTextObject flavorText = Instantiate(_flavorTextPrefab, _rulesPanel);
            flavorText.SetupFlavorText(card.FlavorText);
        }
    }

    protected virtual void SetupRules(string rulesText)
    {

    }
}
