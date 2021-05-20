using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleZoneManager : MonoBehaviour
{
    private float _fromTransitionTime;
    private float _toTransitionTime;
    private Transform _intermediateHolder;

    private BattleZoneLayoutHandler _battleZoneLayoutHandler;

    private void Awake()
    {
        _battleZoneLayoutHandler = GetComponent<BattleZoneLayoutHandler>();
    }

    public void Initialize(float fromTransitionTime, float toTransitionTime, Transform intermediateTransform)
    {
        _fromTransitionTime = fromTransitionTime;
        _toTransitionTime = toTransitionTime;
        _intermediateHolder = intermediateTransform;
    }

    public IEnumerator MoveFromBattleZoneRoutine(Transform cardTransform)
    {
        cardTransform.parent = _intermediateHolder;
        cardTransform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(_intermediateHolder.rotation.eulerAngles, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToBattleZoneRoutine(Transform cardTransform)
    {
        Transform tempCard = _battleZoneLayoutHandler.AssignTempCard();
        cardTransform.DOMove(tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(tempCard.rotation.eulerAngles, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(tempCard.lossyScale, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        _battleZoneLayoutHandler.AddCard(cardTransform);
    }
    
    public CreatureCardManager GetCardAtIndex(int index)
    {
        return _battleZoneLayoutHandler.GetCardAtIndex(index);
    }

    public CreatureCardManager RemoveCardAtIndex(int index)
    {
        return _battleZoneLayoutHandler.RemoveCardAtIndex(index);
    }
}
