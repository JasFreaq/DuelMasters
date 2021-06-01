using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class HandLayoutHandler : MonoBehaviour
{
    [SerializeField] private PlayerDataHolder _playerData;
    [SerializeField] private float _circleRadius = 100;
    [SerializeField] private float _cardAreaWidth = 24;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private int _handSortingLayerFloor = 50;
    [SerializeField] private bool _arrangeLeftToRight = true;
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetScale;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;
    [SerializeField] private bool _isPlayer = true;
    [SerializeField] private float _fromTransitionTime = 2f;
    [SerializeField] private float _toTransitionTime = 2f;
    [SerializeField] private Transform _intermediateHolder;

    private Vector3 _circleCenter;
    private Vector3 _circleCentralAxis;
    
    public Transform HolderTransform
    {
        get { return _holderTransform; }
    }

    private void Start()
    {
        _circleCenter = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z - _circleRadius);
        _circleCentralAxis = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z) - _circleCenter;
        _circleCentralAxis.Normalize();
    }

    public IEnumerator MoveFromHandRoutine(CardManager card, bool forShield)
    {
        card.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        Vector3 rotation = new Vector3(-90, 0, 0);
        if (forShield && !_isPlayer)
            rotation = new Vector3(90, 0, 0);
        card.transform.DORotate(rotation, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToHandRoutine(CardManager card, bool opponentVisible)
    {
        _tempCard.parent = _holderTransform;
        ArrangeCards();
        card.transform.DOMove(_tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);

        Vector3 rotation = _tempCard.rotation.eulerAngles;
        if (!_isPlayer && opponentVisible)
        {
            rotation -= new Vector3(0, 0, 180);
        }
        card.transform.DORotate(rotation, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        if (!_isPlayer && opponentVisible)
        {
            //TODO: Call Visible Icon Code
        }

        _playerData.CardsInHand.Add(card.transform.GetInstanceID(), card);
        AddCard(card);
    }

    public CardManager RemoveCardAtIndex(CardManager card)
    {
        card.transform.parent = transform;
        ArrangeCards();
        return card;
    }

    private void AddCard(CardManager card)
    {
        _tempCard.parent = transform;
        card.transform.parent = _holderTransform;

        card.HoverPreviewHandler.TargetPosition = _previewTargetPosition;
        card.HoverPreviewHandler.TargetScale = _previewTargetScale;
        
        ArrangeCards();
    }
    
    private Vector3 ArrangeCards(int index = -1)
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

        Vector3 indexCardPos = Vector3.zero;

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _holderTransform.GetChild(i);
            
            float offset = _arrangeLeftToRight ? (i - n / 2 + 1) * cardWidth : -(i - n / 2 + 1) * cardWidth;
            Vector3 cardPos = new Vector3(startPos.x + offset, startPos.y, startPos.z);

            int iD = _holderTransform.GetChild(i).GetInstanceID();
            if (_playerData.CardsInHand.ContainsKey(iD))
                _playerData.CardsInHand[iD].CardLayout.Canvas.sortingOrder = _handSortingLayerFloor + i;

            Vector3 relativeVector = cardPos - _circleCenter;
            relativeVector.Normalize();

            cardTransform.localEulerAngles = new Vector3(cardTransform.localEulerAngles.x, Vector3.SignedAngle(relativeVector, _circleCentralAxis,_holderTransform.up),
                cardTransform.localEulerAngles.z);

            cardPos = relativeVector * _circleRadius;
            cardPos.z -= _circleRadius;

            if (index == i)
                indexCardPos = transform.TransformPoint(cardPos);
            else
                cardTransform.localPosition = cardPos;
        }
        
        return indexCardPos;
    }
    
    public void DragRearrange(CardManager card)
    {
        int index = card.transform.GetSiblingIndex();
        int siblingIndex = -1;

        if (index > 0)
        {
            Transform sibling = _holderTransform.GetChild(index - 1);
            if (card.transform.position.x < sibling.position.x)
            {
                siblingIndex = index - 1;
            }
        }

        if (index < _holderTransform.childCount - 1)
        {
            Transform sibling = _holderTransform.GetChild(index + 1);
            if (card.transform.position.x > sibling.position.x)
            {
                siblingIndex = index + 1;
            }
        }

        if (siblingIndex != -1)
        {
            _holderTransform.GetChild(siblingIndex).SetSiblingIndex(index);
            card.transform.SetSiblingIndex(siblingIndex);
            card.DragHandler.OriginalPosition = ArrangeCards(siblingIndex);
        }
    }
}
