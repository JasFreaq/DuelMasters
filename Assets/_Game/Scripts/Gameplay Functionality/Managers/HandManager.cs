using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class HandManager : MonoBehaviour
{
    [SerializeField] private float _dragArrangeYLimit = -7.5f;
    [SerializeField] private int _handSortingLayerFloor = 50;

    private bool _isPlayer = true;
    private float _fromTransitionTime;
    private float _toTransitionTime;
    private Transform _intermediateHolder;

    private HandLayoutHandler _handLayoutHandler;
    private Dictionary<int, CardManager> _cardsInHand = new Dictionary<int, CardManager>();

    private void Awake()
    {
        _handLayoutHandler = GetComponent<HandLayoutHandler>();
    }

    private void OnEnable()
    {
        _handLayoutHandler.RegisterOnArrange(UpdateOnArrange);
    }

    private void OnDisable()
    {
        _handLayoutHandler.DeregisterOnArrange(UpdateOnArrange);
    }

    public void Initialize(bool isPlayer, float fromTransitionTime, float toTransitionTime, Transform intermediateTransform)
    {
        _isPlayer = isPlayer;
        _fromTransitionTime = fromTransitionTime;
        _toTransitionTime = toTransitionTime;
        _intermediateHolder = intermediateTransform;
    }

    public IEnumerator MoveFromHandRoutine(Transform cardTransform, bool forShield = false)
    {
        cardTransform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        Vector3 rotation = new Vector3(-90, 0, 0);
        if (forShield && !_isPlayer)
            rotation = new Vector3(90, 0, 0);
        cardTransform.DORotate(rotation, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToHandRoutine(CardManager card, bool opponentVisible = false)
    {
        Transform tempCard = _handLayoutHandler.AssignTempCard();
        card.transform.DOMove(tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);

        Vector3 rotation = tempCard.rotation.eulerAngles;
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

        card.DragHandler.RegisterOnDrag(HandleCardDrag);
        _cardsInHand.Add(card.transform.GetInstanceID(), card);
        _handLayoutHandler.AddCard(card);
    }
    
    public CardManager GetCardAtIndex(int index)
    {
        return _cardsInHand[_handLayoutHandler.HolderTransform.GetChild(index).GetInstanceID()];
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        CardManager card = GetCardAtIndex(index);
        card.DragHandler.DeregisterOnDrag(HandleCardDrag);
        _cardsInHand.Remove(_handLayoutHandler.HolderTransform.GetChild(index).GetInstanceID());
        return _handLayoutHandler.RemoveCardAtIndex(card);
    }

    private void UpdateOnArrange()
    {
        Transform holder = _handLayoutHandler.HolderTransform;
        int n = holder.childCount;

        for (int i = 0; i < n; i++)
        {
            int iD = holder.GetChild(i).GetInstanceID();
            if (_cardsInHand.ContainsKey(iD))
                _cardsInHand[iD].CardLayout.Canvas.sortingOrder = _handSortingLayerFloor + i;
        }
    }

    private void HandleCardDrag(Transform draggedTransform)
    {
        CardManager card = _cardsInHand[draggedTransform.GetInstanceID()];
        if (draggedTransform.position.y < _dragArrangeYLimit)
        {
            card.SetGlow(false);
            _handLayoutHandler.DragRearrange(card);
        }
        else
        {
            card.SetGlow(true);
        }
    }
}
