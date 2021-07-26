using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public abstract class CompactCardLayoutHandler : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _artworkImage;
    [SerializeField] private Image _frameImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Renderer _bgRenderer;
    [SerializeField] private Image _highlightFrame;
    [SerializeField] private GameObject _tappedOverlay;

    public Canvas Canvas
    {
        get { return _canvas; }
    }

    public GameObject TappedOverlay
    {
        get { return _tappedOverlay; }
    }

    public virtual void SetupCard(CardData cardData, CompactCardFrameData compactFrameData)
    {
        _artworkImage.sprite = cardData.ArtworkImage;
        _frameImage.sprite = compactFrameData.frameImage;
        _nameText.text = cardData.Name;

        _bgRenderer.material = compactFrameData.frameBGMaterial;
    }

    public void SetGlow(bool enableGlow)
    {
        _highlightFrame.gameObject.SetActive(enableGlow);
    }

    public void SetHighlightColor(Color color)
    {
        _highlightFrame.color = color;
    }
}
