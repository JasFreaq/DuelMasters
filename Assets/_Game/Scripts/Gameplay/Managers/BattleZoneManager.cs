using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleZoneManager : MonoBehaviour
{
    [SerializeField] private PlayerDataHandler _playerData;

    [Header("Layout")]
    [SerializeField] private float _cardAreaWidth = 28;
    [SerializeField] private float _maxCardWidth = 10.25f;
    [SerializeField] private int _battleZoneSortingLayerFloor = 0;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;
    
    [Header("Transition")]
    [SerializeField] private float _fromTransitionTime = 1f;
    [SerializeField] private float _toTransitionTime = 1f;
    [SerializeField] private Transform _intermediateHolder;

    #region Functionality Methods

    public void AddCard(CreatureCardManager card)
    {
        _tempCard.parent = transform;
        _tempCard.localScale = Vector3.one;
        card.transform.parent = _holderTransform;

        _playerData.CardsInBattle.Add(card.transform.GetInstanceID(), card);
        card.CurrentZone = CardZone.BattleZone;
    }
    
    private void RemoveCardAtIndex(int index)
    {
        int iD = _holderTransform.GetChild(index).GetInstanceID();
        CreatureCardManager card = _playerData.CardsInBattle[iD];
        _playerData.CardsInBattle.Remove(iD);
        card.transform.parent = transform;
        ArrangeCards();
    }

    #endregion

    #region Transition Methods

    public IEnumerator MoveFromBattleZoneRoutine(CardManager card)
    {
        RemoveCardAtIndex(card.transform.GetSiblingIndex());

        card.transform.parent = _intermediateHolder;
        card.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DORotateQuaternion(_intermediateHolder.rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToBattleZoneRoutine(CreatureCardManager card)
    {
        _tempCard.parent = _holderTransform;
        ArrangeCards();

        card.transform.DOMove(_tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DORotateQuaternion(_tempCard.rotation, _toTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DOScale(_tempCard.lossyScale, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        AddCard(card);
    }
    
    #endregion

    #region Layout Methods

    private void ArrangeCards()
    {
        int n = _holderTransform.childCount;
        float cardWidth = Mathf.Min((_cardAreaWidth * 2) / n, _maxCardWidth);
        float sizeRatio = cardWidth / _maxCardWidth;

        float startOffset = (n % 2) * cardWidth;
        if (n % 2 == 0)
            startOffset += cardWidth / 2;
        Vector3 startPos = new Vector3(_holderTransform.localPosition.x - startOffset, _holderTransform.localPosition.y, _holderTransform.localPosition.z);

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _holderTransform.GetChild(i);
            int iD = cardTransform.GetInstanceID();
            if (_playerData.CardsInBattle.ContainsKey(iD))
                _playerData.CardsInBattle[iD].BattleLayout.Canvas.sortingOrder = _battleZoneSortingLayerFloor + i;
            Vector3 cardPos = new Vector3(startPos.x + (i - n / 2 + 1) * cardWidth, startPos.y, startPos.z);

            cardTransform.localPosition = cardPos;
            cardTransform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
        }
    }

    #endregion
}
