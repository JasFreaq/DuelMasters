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
    [SerializeField] private GameObject _glowFrame;
    [SerializeField] private GameObject _tappedOverlay;
    [SerializeField] private CompactCardFrameDatabase _cardFrameDatabase;

    public Canvas Canvas
    {
        get { return _canvas; }
    }
    
    public virtual void SetupCard(Card card)
    {
        _artworkImage.sprite = card.ArtworkImage;
        CompactCardFrameData cardFrameData = _cardFrameDatabase.GetFrame(card.Civilization);
        _frameImage.sprite = cardFrameData.frameImage;
        _nameText.text = card.Name;
        _bgRenderer.material = cardFrameData.frameBGMaterial;
    }

    public void SetGlow(bool enableGlow)
    {
        _glowFrame.SetActive(enableGlow);
    }
}
