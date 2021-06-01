using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class HandManager : MonoBehaviour
{
    [SerializeField] private PlayerDataHolder _playerData;
    [SerializeField] private float _dragArrangeYLimit = -7.5f;

    private HandLayoutHandler _handLayoutHandler;
    
    private void Awake()
    {
        _handLayoutHandler = GetComponent<HandLayoutHandler>();
    }

    public Coroutine MoveFromHand(CardManager card, bool forShield = false)
    {
        return StartCoroutine(_handLayoutHandler.MoveFromHandRoutine(card, forShield));
    }

    public Coroutine MoveToHand(CardManager card, bool opponentVisible = false)
    {
        card.DragHandler.RegisterOnDrag(HandleCardDrag);
        return StartCoroutine(_handLayoutHandler.MoveToHandRoutine(card, opponentVisible));
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        int iD = _handLayoutHandler.HolderTransform.GetChild(index).GetInstanceID();

        CardManager card = _playerData.CardsInHand[iD];
        card.DragHandler.DeregisterOnDrag(HandleCardDrag);

        _playerData.CardsInHand.Remove(iD);
        return _handLayoutHandler.RemoveCardAtIndex(card);
    }
    
    private void HandleCardDrag(Transform draggedTransform)
    {
        CardManager card = _playerData.CardsInHand[draggedTransform.GetInstanceID()];
        if (draggedTransform.position.y < _dragArrangeYLimit)
        {
            if (!card.DragHandler.ReturnToPosition)
                card.DragHandler.ReturnToPosition = true;

            _handLayoutHandler.DragRearrange(card);
        }
        else
        {
            if (card.DragHandler.ReturnToPosition)
                card.DragHandler.ReturnToPosition = false;
        }
    }
}
