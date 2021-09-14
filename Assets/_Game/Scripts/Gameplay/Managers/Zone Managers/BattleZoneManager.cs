using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class BattleZoneManager : MonoBehaviour
{
    private const int _LayoutRows = 2;

    [SerializeField] private bool _isPlayer = true;
    
    [Header("Layout")]
    [SerializeField] private float _cardAreaWidth = 20f;
    [SerializeField] private float _maxCardWidth = 10.25f;
    [SerializeField] private float _rowVerticalPosAdjustment = 1f;
    [SerializeField] private float _secondRowPos = 9.5f;
    [SerializeField] [Range(0f, 1f)] private float _sizeAdjustmentFactor = 0.5f;
    [SerializeField] private int _maxCardsInRow = 5;
    [SerializeField] private int _battleZoneSortingLayerFloor = 0;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;
    
    [Header("Transition")]
    [SerializeField] private float _fromTransitionTime = 1f;
    [SerializeField] private float _toTransitionTime = 1f;
    [SerializeField] private int _evolutionTransitionFramesBuffer = 5;
    [SerializeField] private Transform _intermediateHolder;
    
    private PlayerDataHandler _playerData;

    private void Start()
    {
        _playerData = GameDataHandler.Instance.GetDataHandler(_isPlayer);
    }
    
    #region Functionality Methods

    public void AddCard(CreatureObject cardObj)
    {
        _tempCard.parent = transform;
        _tempCard.localScale = Vector3.one;
        cardObj.transform.parent = _holderTransform;

        _playerData.CardsInBattle.Add(cardObj.transform.GetInstanceID(), cardObj);
        if (cardObj.CardInst.InstanceEffectHandler.IsBlocker)
            _playerData.BlockersInBattle.Add(cardObj);

        cardObj.CardInst.SetCurrentZone(CardZoneType.BattleZone);
    }
    
    private void RemoveCardAtIndex(int index)
    {
        int iD = _holderTransform.GetChild(index).GetInstanceID();
        CreatureObject creatureObj = _playerData.CardsInBattle[iD];

        _playerData.CardsInBattle.Remove(iD);
        if (creatureObj.CardInst.InstanceEffectHandler.IsBlocker)
            _playerData.BlockersInBattle.Remove(creatureObj);

        creatureObj.transform.parent = transform;
    }

    #endregion

    #region Transition Methods

    public IEnumerator MoveFromBattleZoneRoutine(CardObject cardObj)
    {
        RemoveCardAtIndex(cardObj.transform.GetSiblingIndex());
        ArrangeCards();

        cardObj.transform.parent = _intermediateHolder;
        cardObj.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardObj.transform.DORotateQuaternion(_intermediateHolder.rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardObj.transform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToBattleZoneRoutine(CreatureObject creatureObj)
    {
        _tempCard.parent = _holderTransform;
        ArrangeCards();

        creatureObj.transform.DOMove(_tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);
        creatureObj.transform.DORotateQuaternion(_tempCard.rotation, _toTransitionTime).SetEase(Ease.OutQuint);
        creatureObj.transform.DOScale(_tempCard.lossyScale, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        AddCard(creatureObj);
    }
    
    public IEnumerator MoveToBattleZoneRoutine(CreatureObject evolvingCreatureObj, CreatureObject creatureObj)
    {
        int originalIndex = creatureObj.transform.GetSiblingIndex();

        evolvingCreatureObj.transform.DOMove(creatureObj.transform.position, _toTransitionTime).SetEase(Ease.OutQuint);
        evolvingCreatureObj.transform.DORotateQuaternion(creatureObj.transform.rotation, _toTransitionTime).SetEase(Ease.OutQuint);
        evolvingCreatureObj.transform.DOScale(creatureObj.transform.lossyScale, _toTransitionTime).SetEase(Ease.OutQuint);

        float transitionBufferTime = _evolutionTransitionFramesBuffer * Time.deltaTime;
        yield return new WaitForSeconds(_toTransitionTime - transitionBufferTime);
        creatureObj.gameObject.SetActive(false);
        yield return new WaitForSeconds(transitionBufferTime);

        RemoveCardAtIndex(originalIndex);
        evolvingCreatureObj.CreaturesUnderEvolution.Add(creatureObj);
        creatureObj.transform.SetParent(evolvingCreatureObj.transform);

        AddCard(evolvingCreatureObj);
        evolvingCreatureObj.transform.SetSiblingIndex(originalIndex);
    }
    
    #endregion

    #region Layout Methods

    public void ArrangeCards()
    {
        int n = _holderTransform.childCount;
        float arrangeTime = GameParamsHolder.Instance.LayoutsArrangeMoveTime;

        for (int i = 0; i < n; i++)
        {
            int maxCardsInRow = _maxCardsInRow;
            if (n > _maxCardsInRow)
            {
                maxCardsInRow = n / _LayoutRows;
                if (maxCardsInRow <= (i / maxCardsInRow + 1) * maxCardsInRow)
                    maxCardsInRow += n % _LayoutRows;
            }
            int cardsInRow, columnFactor = (i / maxCardsInRow + 1) * maxCardsInRow;
            if (n >= columnFactor)
                cardsInRow = maxCardsInRow;
            else
                cardsInRow = maxCardsInRow + n - columnFactor;

            float cardWidth = Mathf.Min((_cardAreaWidth * 2) / cardsInRow, _maxCardWidth);
            float sizeRatio = cardWidth / _maxCardWidth;

            Vector2 startOffset = new Vector2((cardsInRow % 2) * cardWidth, 0);
            if (cardsInRow % 2 == 0)
                startOffset.x += cardWidth / 2;
            if (n > _maxCardsInRow)
            {
                sizeRatio *= _sizeAdjustmentFactor;
                startOffset.y = _rowVerticalPosAdjustment * sizeRatio;
            }
            Vector3 startPos = new Vector3(_holderTransform.localPosition.x - startOffset.x,
                _holderTransform.localPosition.y, _holderTransform.localPosition.z - startOffset.y);

            Transform cardTransform = _holderTransform.GetChild(i);
            float heightAdjuster = i / maxCardsInRow == 0 ? 0 : _secondRowPos * sizeRatio;
            Vector3 cardPos = new Vector3(startPos.x + (i % maxCardsInRow - cardsInRow / 2 + 1) * cardWidth,
                startPos.y, startPos.z + heightAdjuster);
            Vector3 cardScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
            
            int iD = cardTransform.GetInstanceID();
            if (_playerData.CardsInBattle.ContainsKey(iD))
            {
                _playerData.CardsInBattle[iD].BattleLayoutCanvas.sortingOrder = _battleZoneSortingLayerFloor + i;
                cardTransform.DOLocalMove(cardPos, arrangeTime).SetEase(Ease.OutQuint);
                cardTransform.DOScale(cardScale, arrangeTime).SetEase(Ease.OutQuint);
            }
            else
            {
                cardTransform.localPosition = cardPos;
                cardTransform.localScale = cardScale;
            }
        }
    }

    #endregion
}
