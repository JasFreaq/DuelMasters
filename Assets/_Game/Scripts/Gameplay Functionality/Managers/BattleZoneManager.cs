using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleZoneManager : MonoBehaviour
{
    [SerializeField] private PlayerDataHolder _playerData;

    [Header("Transition")]
    [SerializeField] private Transform _intermediateHolder;
    [SerializeField] private float _fromTransitionTime = 1f;
    [SerializeField] private float _toTransitionTime = 1f;

    [Header("Layout")]
    [SerializeField] private float _cardAreaWidth = 28;
    [SerializeField] private float _maxCardWidth = 10.25f;
    [SerializeField] private int _battleZoneSortingLayerFloor = 0;
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetScale;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;

    #region Functionality Methods

    public void AddCard(Transform cardTransform)
    {
        _tempCard.parent = transform;
        _tempCard.localScale = Vector3.one;
        cardTransform.parent = _holderTransform;

        CreatureCardManager card = cardTransform.GetComponent<CreatureCardManager>();
        card.HoverPreviewHandler.TargetPosition = _previewTargetPosition;
        card.HoverPreviewHandler.TargetScale = _previewTargetScale;
        _playerData.CardsInBattle.Add(cardTransform.GetInstanceID(), card);

        ArrangeCards();
    }

    public CreatureCardManager GetCardAtIndex(int index)
    {
        return _playerData.CardsInBattle[_holderTransform.GetChild(index).GetInstanceID()];
    }

    public CreatureCardManager RemoveCardAtIndex(int index)
    {
        CreatureCardManager card = GetCardAtIndex(index);
        _playerData.CardsInBattle.Remove(_holderTransform.GetChild(index).GetInstanceID());
        card.transform.parent = transform;
        ArrangeCards();
        return card;
    }

    #endregion

    #region Transition Methods

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
        _tempCard.parent = _holderTransform;
        ArrangeCards();

        cardTransform.DOMove(_tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(_tempCard.rotation.eulerAngles, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(_tempCard.lossyScale, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        AddCard(cardTransform);
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
