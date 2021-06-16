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
        public Quaternion rotation;
    }

    #endregion

    [SerializeField] private PlayerDataHandler _playerData;
    [SerializeField] private float _dragArrangeYLimit = -7.5f;
    
    [Header("Layout")]
    [SerializeField] private float _circleRadius = 150;
    [SerializeField] private float _cardAreaWidth = 24;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private int _handSortingLayerFloor = 50;
    [SerializeField] private bool _arrangeLeftToRight = true;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;
    
    [Header("Transition")]
    [SerializeField] private bool _isPlayer = true;
    [SerializeField] private float _fromTransitionTime = 1.5f;
    [SerializeField] private float _toTransitionTime = 1.5f;
    [SerializeField] private Transform _intermediateHolder;

    [HideInInspector] public Transform _flippedIntermediateHolder;
    [HideInInspector] public Vector3 _previewTargetPosition = new Vector3(0, -8.25f, -14.5f);
    [HideInInspector] public Vector3 _previewTargetRotation = new Vector3(-105f, 0, 0);
    [HideInInspector] public Vector3 _previewTargetScale = new Vector3(1.5f, 1.5f, 1.5f);

    private Vector3 _circleCenter;
    private Vector3 _circleCentralAxis;

    private Transform _currentPreviewingCard = null;
    private Coroutine _previewResetRoutine = null;

    public bool IsPlayer
    {
        get { return _isPlayer; }
    }
    
    private void Start()
    {
        _circleCenter = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z - _circleRadius);
        _circleCentralAxis = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z) - _circleCenter;
        _circleCentralAxis.Normalize();
    }

    
    #region Functionality Methods
    
    private void HandleCardDrag(Transform cardTransform)
    {
        CardManager card = _playerData.CardsInHand[cardTransform.GetInstanceID()];
        if (card.transform.position.y < _dragArrangeYLimit)
        {
            SetState(false);
            DragRearrange(card);
        }
        else if (GameManager.CurrentStep == GameStepType.ChargeStep ||
                 GameManager.CurrentStep == GameStepType.MainStep) 
        {
            SetState(true);
        }

        void SetState(bool state)
        {
            if (card.ProcessAction != state)
                card.ProcessAction = state;

            if (card.IsGlowSelectColor == state)
                card.SetGlowColor(state);
        }
    }
    
    private void RemoveCardAtIndex(int index)
    {
        int iD = _holderTransform.GetChild(index).GetInstanceID();

        CardManager card = _playerData.CardsInHand[iD];
        card.DragHandler.DeregisterOnDrag(HandleCardDrag);

        if (_isPlayer)
        {
            card.InPlayerHand = false;
            if (card.IsVisible)
                card.VisibleEyeIcon.SetActive(false);
        }

        if (card.IsVisible)
            card.IsVisible = false;
        card.DragHandler.CanDrag = false;

        _playerData.CardsInHand.Remove(iD);
        card.transform.parent = transform;
        
        ArrangeCards();
    }

    private void AddCard(CardManager card)
    {
        _tempCard.parent = transform;
        card.transform.parent = _holderTransform;
        card.DragHandler.RegisterOnDrag(HandleCardDrag);

        if (_isPlayer)
        {
            card.InPlayerHand = true;
            card.HoverPreviewHandler.SetPreviewParameters(_previewTargetPosition, _previewTargetRotation, _previewTargetScale);
        }

        ArrangeCards();
    }

    #endregion

    #region Transition Methods

    public IEnumerator MoveFromHandRoutine(CardManager card, bool forShield = false)
    {
        RemoveCardAtIndex(card.transform.GetSiblingIndex());

        card.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        Quaternion rotation = (forShield && !_isPlayer) ? 
            _flippedIntermediateHolder.rotation : _intermediateHolder.rotation;
        card.transform.DORotateQuaternion(rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToHandRoutine(CardManager card)
    {
        _tempCard.parent = _holderTransform;
        ArrangeCards();
        card.transform.DOMove(_tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);

        Quaternion rotation = _tempCard.rotation;
        if (!_isPlayer && card.IsVisible)
            rotation *= Quaternion.Euler(0, 0, 180);
        card.transform.DORotateQuaternion(rotation, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        if (_isPlayer && card.IsVisible)
            card.VisibleEyeIcon.SetActive(true);

        _playerData.CardsInHand.Add(card.transform.GetInstanceID(), card);
        AddCard(card);
    }
    
    #endregion
    
    #region Layout Methods

    private TransformData ArrangeCards(int index = -1)
    {
        int n = _holderTransform.childCount;
        float cardWidth = Mathf.Min((_cardAreaWidth * 2) / n, _maxCardWidth);
        float arrangeTime = GameParamsHolder.Instance.LayoutsArrangeMoveTime;

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

            Vector3 rotation = new Vector3(_tempCard.localEulerAngles.x,
                Vector3.SignedAngle(relativeVector, _circleCentralAxis, _holderTransform.up),
                _tempCard.localEulerAngles.z);

            cardPos = relativeVector * _circleRadius;
            cardPos.z -= _circleRadius;

            Quaternion cardRot = Quaternion.Euler(rotation);

            if (_playerData.CardsInHand.TryGetValue(cardTransform.GetInstanceID(), out CardManager card))
            {
                card.CardLayout.Canvas.sortingOrder = _handSortingLayerFloor + i;
                if (!_isPlayer && card.IsVisible)
                {
                    cardRot *= Quaternion.Euler(0, 0, 180);
                }
            }
            
            if (index == i)
            {
                indexCardTransform.position = cardPos;
                indexCardTransform.rotation = cardRot;
            }
            else if (card)
            {
                cardTransform.DOLocalMove(cardPos, arrangeTime).SetEase(Ease.OutQuint); 
                cardTransform.DOLocalRotateQuaternion(cardRot, arrangeTime).SetEase(Ease.OutQuint);
            }
            else
            {
                cardTransform.localPosition = cardPos;
                cardTransform.localRotation = cardRot;
            }
        }

        return indexCardTransform;
    }

    private void DragRearrange(CardManager card)
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
            
            TransformData orientation = ArrangeCards(siblingIndex);
            card.DragHandler.SetOriginalOrientation(orientation.position, orientation.rotation);
        }
    }

    #endregion
}
