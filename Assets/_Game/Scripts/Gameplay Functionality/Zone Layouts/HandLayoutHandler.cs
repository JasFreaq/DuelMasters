using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HandLayoutHandler : MonoBehaviour
{
    [SerializeField] private float _circleRadius = 100;
    [SerializeField] private float _cardAreaWidth = 24;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private bool _arrangeLeftToRight = true;
    [SerializeField] private int _handSortingLayerFloor;
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetScale;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;

    private Vector3 _circleCenter;
    private Vector3 _circleCentralAxis;

    private Dictionary<int, CardManager> _cardsInHand = new Dictionary<int, CardManager>();

    private void Start()
    {
        _circleCenter = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z - _circleRadius);
        _circleCentralAxis = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z) - _circleCenter;
        _circleCentralAxis.Normalize();
    }

    public CardManager GetCardAtIndex(int index)
    {
        return _cardsInHand[_holderTransform.GetChild(index).GetInstanceID()];
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        CardManager card = GetCardAtIndex(index);
        card.DragHandler.DeregisterOnDrag(HandleCardDrag);
        _cardsInHand.Remove(_holderTransform.GetChild(index).GetInstanceID());
        card.transform.parent = transform;
        ArrangeCards();
        return card;
    }

    public void AddCard(Transform cardTransform)
    {
        _tempCard.parent = transform;
        cardTransform.parent = _holderTransform;

        CardManager card = cardTransform.GetComponent<CardManager>();
        card.HoverPreviewHandler.TargetPosition = _previewTargetPosition;
        card.HoverPreviewHandler.TargetScale = _previewTargetScale;
        card.DragHandler.RegisterOnDrag(HandleCardDrag);
        _cardsInHand.Add(cardTransform.GetInstanceID(), card);

        ArrangeCards();
    }

    public Transform AssignTempCard()
    {
        _tempCard.parent = _holderTransform;
        ArrangeCards();
        return _tempCard;
    }

    private void ArrangeCards()
    {
        int n = _holderTransform.childCount;
        float cardWidth = Mathf.Min((_cardAreaWidth * 2) / n, _maxCardWidth);
        float startOffset = (n % 2) * cardWidth;
        if (n % 2 == 0)
            startOffset += cardWidth / 2;
        if (!_arrangeLeftToRight)
            startOffset = -startOffset;
        Vector3 startPos = new Vector3(_holderTransform.localPosition.x - startOffset, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z);

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _holderTransform.GetChild(i);
            if (_cardsInHand.ContainsKey(_holderTransform.GetChild(i).GetInstanceID()))
                _cardsInHand[cardTransform.GetInstanceID()].CardLayout.Canvas.sortingOrder = _handSortingLayerFloor + i;
            
            float offset = _arrangeLeftToRight ? (i - n / 2 + 1) * cardWidth : -(i - n / 2 + 1) * cardWidth;
            Vector3 cardPos = new Vector3(startPos.x + offset, startPos.y, startPos.z);
            
            Vector3 relativeVector = cardPos - _circleCenter;
            relativeVector.Normalize();

            cardTransform.localEulerAngles = new Vector3(cardTransform.localEulerAngles.x, Vector3.SignedAngle(relativeVector, _circleCentralAxis,_holderTransform.up),
                cardTransform.localEulerAngles.z);

            cardPos = relativeVector * _circleRadius;
            cardPos.z -= _circleRadius;
            cardTransform.localPosition = cardPos;
        }
    }

    private void HandleCardDrag(Transform draggedTransform)
    {
        
    }
}
