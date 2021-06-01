using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ManaZoneManager : MonoBehaviour
{
    #region Helper Data Structures

    struct TransformValuePair
    {
        public TransformValuePair(Transform transform, int civValue, string cardName)
        {
            this.transform = transform;
            this.civValue = civValue;
            this.cardName = cardName;
        }

        public Transform transform;
        public int civValue;
        public string cardName;
    }

    #endregion

    [SerializeField] private PlayerDataHolder _playerData;

    [Header("Transition")]
    [SerializeField] private Transform _intermediateHolder;
    [SerializeField] private float _fromTransitionTime = 1f;
    [SerializeField] private float _toTransitionTime = 1f;

    [Header("Layout")]
    [SerializeField] private float _cardAreaWidth = 50;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private float _maxSameCivWidth = 6;
    [SerializeField] private bool _arrangeLeftToRight = true;
    [SerializeField] private int _manaZoneSortingLayerFloor = 25;
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetScale;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;

    private void Start()
    {
        CardManager card = _tempCard.gameObject.AddComponent<CardManager>();
        ManaCardLayoutHandler manaCardLayout = _tempCard.gameObject.AddComponent<ManaCardLayoutHandler>();
        card.ManaLayout = manaCardLayout;
        _playerData.CardsInMana.Add(_tempCard.GetInstanceID(), card);
    }

    #region Transition Methods

    public IEnumerator MoveFromManaZoneRoutine(Transform cardTransform)
    {
        cardTransform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(_intermediateHolder.rotation.eulerAngles, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToManaZoneRoutine(Transform cardTransform, Card card)
    {
        _tempCard.parent = _holderTransform;
        _playerData.CardsInMana[_tempCard.GetInstanceID()].SetupCard(card, true);
        ArrangeCards();

        cardTransform.DOMove(_tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(_tempCard.rotation.eulerAngles, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(transform.localScale, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        AddCard(cardTransform);
    }

    public void AddCard(Transform cardTransform)
    {
        _tempCard.parent = transform;
        cardTransform.parent = _holderTransform;

        CardManager card = cardTransform.GetComponent<CardManager>();
        card.HoverPreviewHandler.TargetPosition = _previewTargetPosition;
        card.HoverPreviewHandler.TargetScale = _previewTargetScale;
        _playerData.CardsInMana.Add(cardTransform.GetInstanceID(), card);

        ArrangeCards();
    }

    public CardManager GetCardAtIndex(int index)
    {
        return _playerData.CardsInMana[_holderTransform.GetChild(index).GetInstanceID()];
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        CardManager card = GetCardAtIndex(index);
        _playerData.CardsInMana.Remove(_holderTransform.GetChild(index).GetInstanceID());
        card.transform.parent = transform;
        ArrangeCards();
        return card;
    }

    #endregion

    #region Layout Methods

    void ArrangeCards()
    {
        RearrangeCardOrder();

        int n = _holderTransform.childCount;
        float cardWidth = Mathf.Min(_cardAreaWidth / n, _maxCardWidth);
        float sameCivWidth = Mathf.Min(cardWidth / _maxCardWidth) * _maxSameCivWidth;
        Vector3 pos = _holderTransform.localPosition;

        CardManager lastCard = null;

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _holderTransform.GetChild(i);
            CardManager currentCard = _playerData.CardsInMana[cardTransform.GetInstanceID()];
            if (currentCard.ManaLayout.Canvas)
                currentCard.ManaLayout.Canvas.sortingOrder = _manaZoneSortingLayerFloor + i;

            float currentWidth = cardWidth;
            if (lastCard != null)
            {
                if (CardParams.IsCivilizationEqual(currentCard.Card.Civilization, lastCard.Card.Civilization))
                {
                    currentWidth = sameCivWidth;
                }
            }

            if (i > 0)
                pos.x += _arrangeLeftToRight ? currentWidth : -currentWidth;
            cardTransform.localPosition = pos;
            pos = cardTransform.localPosition;
            lastCard = currentCard;
        }
    }

    void RearrangeCardOrder()
    {
        int n = _holderTransform.childCount;
        TransformValuePair[] transformValuePairs = new TransformValuePair[n];
        for (int i = 0; i < n; i++)
        {
            int value = 0;
            CardManager card = _playerData.CardsInMana[_holderTransform.GetChild(i).GetInstanceID()];
            foreach (CardParams.Civilization civilization in card.Card.Civilization)
            {
                value += (int)civilization;
            }

            transformValuePairs[i] = new TransformValuePair(_holderTransform.GetChild(i), value, card.Card.Name);
        }

        Array.Sort(transformValuePairs, delegate (TransformValuePair a, TransformValuePair b)
        {
            return a.civValue - b.civValue;
        });

        for (int i = 0; i < n; i++)
        {
            transformValuePairs[i].transform.SetSiblingIndex(i);
        }
    }

    #endregion
}
