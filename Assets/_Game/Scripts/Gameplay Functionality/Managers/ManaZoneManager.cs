using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ManaZoneManager : MonoBehaviour
{
    private bool _isPlayer = true;
    private float _fromTransitionTime;
    private float _toTransitionTime;
    private Transform _intermediateHolder;

    private ManaZoneLayoutHandler _manaZoneLayoutHandler;

    private void Awake()
    {
        _manaZoneLayoutHandler = GetComponent<ManaZoneLayoutHandler>();
    }

    public void Initialize(bool isPlayer, float fromTransitionTime, float toTransitionTime, Transform intermediateTransform)
    {
        _isPlayer = isPlayer;
        _fromTransitionTime = fromTransitionTime;
        _toTransitionTime = toTransitionTime;
        _intermediateHolder = intermediateTransform;
    }

    public IEnumerator MoveFromManaZoneRoutine(Transform cardTransform)
    {
        cardTransform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(_intermediateHolder.rotation.eulerAngles, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToManaZoneRoutine(Transform cardTransform, CardData cardData)
    {
        Transform tempCard = _manaZoneLayoutHandler.AssignTempCard(cardData);
        cardTransform.DOMove(tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(tempCard.rotation.eulerAngles, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(transform.localScale, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        _manaZoneLayoutHandler.AddCard(cardTransform);
    }
    
    public CardManager GetCardAtIndex(int index)
    {
        return _manaZoneLayoutHandler.GetCardAtIndex(index);
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        return _manaZoneLayoutHandler.RemoveCardAtIndex(index);
    }
}
