using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ManaZoneManager : MonoBehaviour
{
    #region Helper Data Structures

    struct ManaTransform
    {
        public ManaTransform(Transform transform, int civValue, string cardName)
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

    private ManaTransform _tempManaCard;

    private void Start()
    {
        _tempManaCard = new ManaTransform(_tempCard, 0, "");
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
        _tempManaCard.civValue = GetCivValue(card.Civilization);
        _tempManaCard.cardName = card.Name;
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

        ManaTransform lastCard = new ManaTransform(null, 0, "");

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _holderTransform.GetChild(i);
            float currentWidth = cardWidth;

            int currentCivValue;
            if (_playerData.CardsInMana.TryGetValue(cardTransform.GetInstanceID(), out CardManager currentCard)) 
            {
                currentCard.ManaLayout.Canvas.sortingOrder = _manaZoneSortingLayerFloor + i;
                currentCivValue = GetCivValue(currentCard.Card.Civilization);
            }
            else
            {
                currentCivValue = _tempManaCard.civValue;
            }

            if (lastCard.transform != null)
            {
                if (currentCivValue == lastCard.civValue) 
                {
                    currentWidth = sameCivWidth;
                }
            }

            if (i > 0)
                pos.x += _arrangeLeftToRight ? currentWidth : -currentWidth;
            cardTransform.localPosition = pos;
            pos = cardTransform.localPosition;

            lastCard.transform = currentCard ? currentCard.transform : _tempManaCard.transform;
            lastCard.civValue = currentCivValue;
        }
    }

    void RearrangeCardOrder()
    {
        int n = _holderTransform.childCount;
        ManaTransform[] manaTransforms = new ManaTransform[n];
        for (int i = 0; i < n; i++)
        {
            if (_playerData.CardsInMana.TryGetValue(_holderTransform.GetChild(i).GetInstanceID(), out CardManager card))
            {
                manaTransforms[i] = new ManaTransform(_holderTransform.GetChild(i),
                    GetCivValue(card.Card.Civilization),
                    card.Card.Name);
            }
            else
            {
                manaTransforms[i] = _tempManaCard;
            }
        }

        Array.Sort(manaTransforms, delegate (ManaTransform a, ManaTransform b)
        {
            return a.civValue - b.civValue;
        });

        for (int i = 0; i < n; i++)
        {
            manaTransforms[i].transform.SetSiblingIndex(i);
        }
    }

    int GetCivValue(CardParams.Civilization[] civilizations)
    {
        int value = 0;
        foreach (CardParams.Civilization civilization in civilizations)
        {
            value += (int) civilization + value * 5;
        }

        return value;
    }

    #endregion
}
