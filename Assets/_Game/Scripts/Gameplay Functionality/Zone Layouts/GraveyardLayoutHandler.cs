using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardLayoutHandler : MonoBehaviour
{
    [SerializeField] private float _cardWidth = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _cardScale = 0.5f;
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetScale;

    private List<CardManager> _cards = new List<CardManager>();
    
    float _lastYPos = 0;

    public void AddCard(Transform cardTransform)
    {
        CardManager card = cardTransform.GetComponent<CardManager>();
        cardTransform.parent = transform;
        cardTransform.localPosition = new Vector3(0, _lastYPos -= _cardWidth * _cardScale, 0);
        cardTransform.localScale = Vector3.one * _cardScale;

        if (_cards.Count > 0)
        {
            _cards[_cards.Count - 1].HoverPreview.PreviewEnabled = false;
        }
        card.HoverPreview.TargetPosition = _previewTargetPosition;
        card.HoverPreview.TargetScale = _previewTargetScale;
        card.HoverPreview.PreviewEnabled = true;
        _cards.Add(card);
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        if (index == _cards.Count - 1) 
        {
            _cards[index].HoverPreview.PreviewEnabled = false;
            if (_cards.Count > 1)
            {
                _cards[index - 1].HoverPreview.PreviewEnabled = true;
            }
        }

        CardManager card = _cards[index];
        _cards.RemoveAt(index);
        ArrangeCards();
        return card;
    }

    private void ArrangeCards()
    {
        float lastYPos = 0;

        foreach (CardManager card in _cards)
        {
            card.transform.localPosition = new Vector3(0, lastYPos -= _cardWidth * _cardScale, 0);
            card.transform.localScale = Vector3.one * _cardScale;
        }

        _lastYPos = lastYPos;
    }
}
