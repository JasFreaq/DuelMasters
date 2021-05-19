using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class HandManager : MonoBehaviour
{
    private bool _isPlayer = true;
    private float _fromTransitionTime;
    private float _toTransitionTime;
    private Transform _intermediateHolder;

    private HandLayoutHandler _handLayoutHandler;

    private void Awake()
    {
        _handLayoutHandler = GetComponent<HandLayoutHandler>();
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

    public IEnumerator MoveToHandRoutine(Transform cardTransform, bool opponentVisible = false)
    {
        Transform tempCard = _handLayoutHandler.AssignTempCard();
        cardTransform.DOMove(tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);

        Vector3 rotation = tempCard.rotation.eulerAngles;
        if (!_isPlayer && opponentVisible)
        {
            rotation -= new Vector3(0, 0, 180);
        }
        cardTransform.DORotate(rotation, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        if (!_isPlayer && opponentVisible)
        {
            //TODO: Call Visible Icon Code
        }

        _handLayoutHandler.AddCard(cardTransform);
    }
    
    public CardManager GetCardAtIndex(int index)
    {
        return _handLayoutHandler.GetCardAtIndex(index);
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        return _handLayoutHandler.RemoveCardAtIndex(index);
    }
}
