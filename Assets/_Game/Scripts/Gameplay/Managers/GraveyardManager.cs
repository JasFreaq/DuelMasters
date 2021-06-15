using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardManager : MonoBehaviour
{
    [SerializeField] private PlayerDataHandler _playerData;

    [Header("Preview")]
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetScale;

    [Header("Layout")] 
    [SerializeField] private bool _isPlayer = true;
    [SerializeField] private float _cardWidth = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _cardScale = 0.5f;
    
    float _lastYPos = 0;

    private void Start()
    {
        if (!_isPlayer)
            _cardWidth = -_cardWidth;
    }

    public void AddCard(CardManager card)
    {
        card.transform.parent = transform;
        card.transform.localPosition = new Vector3(0, _lastYPos += _cardWidth * _cardScale, 0);
        card.transform.localScale = Vector3.one * _cardScale;

        if (_playerData.CardsInGrave.Count > 0)
        {
            _playerData.CardsInGrave[_playerData.CardsInGrave.Count - 1].HoverPreviewHandler.PreviewEnabled = false;
        }

        card.HoverPreviewHandler.SetPreviewParameters(_previewTargetPosition, _previewTargetScale);
        card.HoverPreviewHandler.PreviewEnabled = true;
        _playerData.CardsInGrave.Add(card);
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        if (index == _playerData.CardsInGrave.Count - 1)
        {
            _playerData.CardsInGrave[index].HoverPreviewHandler.PreviewEnabled = false;
            if (_playerData.CardsInGrave.Count > 1)
            {
                _playerData.CardsInGrave[index - 1].HoverPreviewHandler.PreviewEnabled = true;
            }
        }

        CardManager card = _playerData.CardsInGrave[index];
        _playerData.CardsInGrave.RemoveAt(index);
        ArrangeCards();
        return card;
    }

    private void ArrangeCards()
    {
        float lastYPos = 0;

        foreach (CardManager card in _playerData.CardsInGrave)
        {
            card.transform.localPosition = new Vector3(0, lastYPos -= _cardWidth * _cardScale, 0);
            card.transform.localScale = Vector3.one * _cardScale;
        }

        _lastYPos = lastYPos;
    }
}
