using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class HandManager : MonoBehaviour
{
    #region Helper Data Structures

    struct TransformData
    {
        public Vector3 position;
        public Vector3 eulerAngles;
    }

    #endregion

    [SerializeField] private PlayerDataHandler _playerData;
    [SerializeField] private float _dragArrangeYLimit = -7.5f;
    
    [Header("Transition")]
    [SerializeField] private Transform _intermediateHolder;
    [SerializeField] private bool _isPlayer = true;
    [SerializeField] private float _fromTransitionTime = 1.5f;
    [SerializeField] private float _toTransitionTime = 1.5f;

    [Header("Preview")]
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetRotation;
    [SerializeField] private Vector3 _previewTargetScale;

    [Header("Layout")]
    [SerializeField] private float _circleRadius = 150;
    [SerializeField] private float _cardAreaWidth = 24;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private int _handSortingLayerFloor = 50;
    [SerializeField] private bool _arrangeLeftToRight = true;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;

    private Vector3 _circleCenter;
    private Vector3 _circleCentralAxis;
    private Vector3 _previewCardPosition;
    private Vector3 _previewCardRotation;

    private CardManager _currentDraggedCard = null;

    private void Start()
    {
        _circleCenter = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z - _circleRadius);
        _circleCentralAxis = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z) - _circleCenter;
        _circleCentralAxis.Normalize();
    }

    private void Update()
    {
        if (_currentDraggedCard) 
            HandleCardDrag();
    }

    #region Functionality Methods

    private void BeginCardDrag(Transform draggedTransform)
    {
        _currentDraggedCard = _playerData.CardsInHand[draggedTransform.GetInstanceID()];

        HoverPreviewHandler previewHandler = _currentDraggedCard.HoverPreviewHandler;
        previewHandler.PreviewEnabled = false;
        previewHandler.EnableHandPreview(false, HandleHandPreview);
    }

    private void HandleCardDrag()
    {
        if (_currentDraggedCard.transform.position.y < _dragArrangeYLimit)
        {
            SetState(true);
            DragRearrange();
        }
        else
        {
            SetState(false);
        }

        void SetState(bool state)
        {
            if (_currentDraggedCard.DragHandler.IsReturningToPosition != state)
                _currentDraggedCard.DragHandler.IsReturningToPosition = state;

            if (_currentDraggedCard.IsGlowSelectColor != state)
                _currentDraggedCard.SetGlowColor(!state);
        }
    }

    private void EndCardDrag()
    {
        HoverPreviewHandler previewHandler = _currentDraggedCard.HoverPreviewHandler;
        previewHandler.PreviewEnabled = true;
        previewHandler.EnableHandPreview(true, HandleHandPreview);
        
        _currentDraggedCard = null;
    }

    private void HandleHandPreview(bool preview, Transform cardTransform)
    {
        if (preview)
        {
            _previewCardPosition = cardTransform.position;
            _previewCardRotation = cardTransform.eulerAngles;

            _playerData.CardsInHand[cardTransform.GetInstanceID()].DragHandler.SetOriginalOrientation(cardTransform.localPosition, cardTransform.localEulerAngles);

            Vector3 previewPosition = _previewTargetPosition;
            previewPosition.x = cardTransform.position.x;
            cardTransform.DOMove(previewPosition, HoverPreviewHandler.TRANSITION_TIME).SetEase(Ease.OutQuint);
            cardTransform.DORotate(_previewTargetRotation, HoverPreviewHandler.TRANSITION_TIME).SetEase(Ease.OutQuint);
            cardTransform.DOScale(_previewTargetScale, HoverPreviewHandler.TRANSITION_TIME).SetEase(Ease.OutQuint);
        }
        else
        {
            cardTransform.DOMove(_previewCardPosition, HoverPreviewHandler.TRANSITION_TIME).SetEase(Ease.OutQuint);
            cardTransform.DORotate(_previewCardRotation, HoverPreviewHandler.TRANSITION_TIME).SetEase(Ease.OutQuint);
            cardTransform.DOScale(Vector3.one, HoverPreviewHandler.TRANSITION_TIME).SetEase(Ease.OutQuint);
        }
    }
    
    private void RemoveCardAtIndex(int index)
    {
        int iD = _holderTransform.GetChild(index).GetInstanceID();

        CardManager card = _playerData.CardsInHand[iD];
        card.DragHandler.DeregisterOnDragBegin(BeginCardDrag);
        card.DragHandler.DeregisterOnDragEnd(EndCardDrag);

        if (_isPlayer)
            card.HoverPreviewHandler.EnableHandPreview(false, HandleHandPreview);

        _playerData.CardsInHand.Remove(iD);
        card.transform.parent = transform;
        ArrangeCards();
    }

    private void AddCard(CardManager card)
    {
        _tempCard.parent = transform;
        card.transform.parent = _holderTransform;

        if (_isPlayer)
                card.HoverPreviewHandler.EnableHandPreview(true, HandleHandPreview);
        else
            card.HoverPreviewHandler.TargetPosition = _previewTargetPosition;
        card.HoverPreviewHandler.TargetScale = _previewTargetScale;

        ArrangeCards();
    }

    #endregion

    #region Transition Methods

    public IEnumerator MoveFromHandRoutine(CardManager card, bool forShield = false)
    {
        RemoveCardAtIndex(card.transform.GetSiblingIndex());
        
        card.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        Vector3 rotation = new Vector3(-90, 0, 0);
        if (forShield && !_isPlayer)
            rotation = new Vector3(90, 0, 0);
        card.transform.DORotate(rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToHandRoutine(CardManager card, bool opponentVisible = false)
    {
        card.DragHandler.RegisterOnDragBegin(BeginCardDrag);
        card.DragHandler.RegisterOnDragEnd(EndCardDrag);

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
    
    #endregion
    
    #region Layout Methods

    private TransformData ArrangeCards(int index = -1)
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

        TransformData indexCardTransform = new TransformData();

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _holderTransform.GetChild(i);

            float offset = _arrangeLeftToRight ? (i - n / 2 + 1) * cardWidth : -(i - n / 2 + 1) * cardWidth;
            Vector3 cardPos = new Vector3(startPos.x + offset, startPos.y, startPos.z);
            
            Vector3 relativeVector = cardPos - _circleCenter;
            relativeVector.Normalize();

            Vector3 cardRot = new Vector3(cardTransform.localEulerAngles.x,
                Vector3.SignedAngle(relativeVector, _circleCentralAxis, _holderTransform.up),
                cardTransform.localEulerAngles.z);

            cardPos = relativeVector * _circleRadius;
            cardPos.z -= _circleRadius;

            if (index == i)
            {
                indexCardTransform.position = cardPos;
                indexCardTransform.eulerAngles = cardRot;
            }
            else
            {
                cardTransform.localPosition = cardPos;
                cardTransform.localEulerAngles = cardRot;
            }

            int iD = _holderTransform.GetChild(i).GetInstanceID();
            if (_playerData.CardsInHand.ContainsKey(iD))
            {
                _playerData.CardsInHand[iD].CardLayout.Canvas.sortingOrder = _handSortingLayerFloor + i;
            }
        }

        return indexCardTransform;
    }

    public void DragRearrange()
    {
        int index = _currentDraggedCard.transform.GetSiblingIndex();
        int siblingIndex = -1;

        if (index > 0)
        {
            Transform sibling = _holderTransform.GetChild(index - 1);
            if (_currentDraggedCard.transform.position.x < sibling.position.x)
            {
                siblingIndex = index - 1;
            }
        }

        if (index < _holderTransform.childCount - 1)
        {
            Transform sibling = _holderTransform.GetChild(index + 1);
            if (_currentDraggedCard.transform.position.x > sibling.position.x)
            {
                siblingIndex = index + 1;
            }
        }

        if (siblingIndex != -1)
        {
            _holderTransform.GetChild(siblingIndex).SetSiblingIndex(index);
            _currentDraggedCard.transform.SetSiblingIndex(siblingIndex);
            TransformData orientation = ArrangeCards(siblingIndex);
            _currentDraggedCard.DragHandler.SetOriginalOrientation(orientation.position, orientation.eulerAngles);
        }
    }

    #endregion
}
