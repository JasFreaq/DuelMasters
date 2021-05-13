using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CompactCardFrameDatabase))] [DisallowMultipleComponent]
public abstract class CompactCardLayoutHandler : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _artworkImage;
    [SerializeField] private Image _frameImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Renderer _bgRenderer;
    [SerializeField] private GameObject _glowFrame;
    [SerializeField] private GameObject _tappedOverlay;

    private CompactCardFrameDatabase _cardFrameDatabase;
    private CardData _cardData;

    private HoverPreview _hoverPreview;

    public Canvas Canvas
    {
        get { return _canvas; }
    }

    public CardData CardData
    {
        get { return _cardData; }
        set { _cardData = value; }
    }

    public HoverPreview HoverPreview
    {
        get { return _hoverPreview; }
    }

    private void Awake()
    {
        _cardFrameDatabase = GetComponent<CompactCardFrameDatabase>();
        _hoverPreview = GetComponent<HoverPreview>();
    }

    public virtual void SetupCard(CardData cardData)
    {
        _cardData = cardData;

        _artworkImage.sprite = cardData.ArtworkImage;
        CompactCardFrameData cardFrameData = _cardFrameDatabase.GetFrame(cardData.Civilization);
        _frameImage.sprite = cardFrameData.frameImage;
        _nameText.text = cardData.Name;
        _bgRenderer.material = cardFrameData.frameBGMaterial;
    }
}
