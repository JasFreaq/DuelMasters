using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ManaZoneManager : MonoBehaviour
{
    #region Helper Data Structures
    [System.Serializable]
    struct ManaTransform
    {
        public ManaTransform(Transform transform, bool isTapped, int civValue, string cardName)
        {
            this.transform = transform;
            this.isTapped = isTapped;
            this.civValue = civValue;
            this.cardName = cardName;
        }

        public Transform transform;
        public bool isTapped;
        public int civValue;
        public string cardName;
    }
    
    #endregion

    [SerializeField] private PlayerDataHandler _playerData;

    [Header("Transition")]
    [SerializeField] private Transform _intermediateHolder;
    [SerializeField] private float _fromTransitionTime = 1f;
    [SerializeField] private float _toTransitionTime = 1f;

    [Header("Preview")]
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetScale;

    [Header("Layout")]
    [SerializeField] private float _cardAreaWidth = 50;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private float _tapUntapDist = 12;
    [SerializeField] private float _maxSameCivTapWidth = 4;
    [SerializeField] private float _maxSameCivUntapWidth = 6;
    [SerializeField] private bool _arrangeLeftToRight = true;
    [SerializeField] private int _manaZoneSortingLayerFloor = 25;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;

    private ManaTransform _tempManaCard;

    private void Start()
    {
        _tempManaCard = new ManaTransform(_tempCard, false, 0, "");
    }

    #region Functionality Methods

    private void AddCard(CardManager card)
    {
        _tempCard.parent = transform;
        card.transform.parent = _holderTransform;

        card.HoverPreviewHandler.TargetPosition = _previewTargetPosition;
        card.HoverPreviewHandler.TargetScale = _previewTargetScale;
        _playerData.CardsInMana.Add(card.transform.GetInstanceID(), card);
    }

    private CardManager GetCardAtIndex(int index)
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

    #region Transition Methods

    public IEnumerator MoveFromManaZoneRoutine(CardManager card)
    {
        card.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DORotate(_intermediateHolder.rotation.eulerAngles, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public IEnumerator MoveToManaZoneRoutine(CardManager card)
    {
        _tempCard.parent = _holderTransform;
        _tempManaCard.isTapped = card.CardData.Civilization.Length > 1;
        if (_tempManaCard.isTapped)
        {
            _tempCard.localEulerAngles = new Vector3(_tempCard.localEulerAngles.x,
                CardManager.TAP_ANGLE, _tempCard.localEulerAngles.z);
        }
        else
        {
            _tempCard.localEulerAngles = new Vector3(_tempCard.localEulerAngles.x,
                0, _tempCard.localEulerAngles.z);
        }

        _tempManaCard.civValue = CardParams.GetCivValue(card.CardData.Civilization);
        _tempManaCard.cardName = card.CardData.Name;
        ArrangeCards();

        card.transform.DOMove(_tempCard.position, _toTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DORotate(_tempCard.rotation.eulerAngles, _toTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DOScale(transform.localScale, _toTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toTransitionTime);

        AddCard(card);
    }
    
    #endregion

    #region Layout Methods

    public void ArrangeCards()
    {
        RearrangeCardOrder();

        int n = _holderTransform.childCount;
        float cardWidth = Mathf.Min(_cardAreaWidth / n, _maxCardWidth);
        float ratio = Mathf.Min(cardWidth / _maxCardWidth);
        Vector3 pos = _holderTransform.localPosition;

        ManaTransform lastManaCard = new ManaTransform(null, false, 0, "");

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _holderTransform.GetChild(i);
            float currentWidth = cardWidth;

            ManaTransform currentManaCard = new ManaTransform(null, false, 0, "");
            if (_playerData.CardsInMana.TryGetValue(cardTransform.GetInstanceID(), out CardManager currentCard)) 
            {
                currentCard.ManaLayout.Canvas.sortingOrder = _manaZoneSortingLayerFloor + i;

                currentManaCard.isTapped = currentCard.IsTapped;
                currentManaCard.civValue = CardParams.GetCivValue(currentCard.CardData.Civilization);
            }
            else
            {
                currentManaCard = _tempManaCard;
            }

            if (lastManaCard.transform != null)
            {
                if (lastManaCard.isTapped)
                {
                    if (currentManaCard.isTapped)
                    {
                        currentWidth = ratio * _maxSameCivTapWidth;
                    }
                    else
                    {
                        currentWidth = ratio * _tapUntapDist;
                    }
                }
                else if (currentManaCard.civValue == lastManaCard.civValue) 
                {
                    currentWidth = ratio * _maxSameCivUntapWidth;
                }
            }

            if (i > 0)
                pos.x += _arrangeLeftToRight ? currentWidth : -currentWidth;
            cardTransform.localPosition = pos;
            pos = cardTransform.localPosition;

            lastManaCard = currentManaCard;
            lastManaCard.transform = currentCard ? currentCard.transform : _tempManaCard.transform;
        }
    }

    private void RearrangeCardOrder()
    {
        int n = _holderTransform.childCount;
        ManaTransform[] manaTransforms = new ManaTransform[n];
        for (int i = 0; i < n; i++)
        {
            if (_playerData.CardsInMana.TryGetValue(_holderTransform.GetChild(i).GetInstanceID(), out CardManager card))
            {
                manaTransforms[i] = new ManaTransform(_holderTransform.GetChild(i), card.IsTapped,
                    CardParams.GetCivValue(card.CardData.Civilization), card.CardData.Name);
            }
            else
            {
                manaTransforms[i] = _tempManaCard;
            }
        }

        Array.Sort(manaTransforms, delegate (ManaTransform a, ManaTransform b)
        {
            if (a.isTapped && !b.isTapped)
                return -1;

            if (!a.isTapped && b.isTapped)
                return 1;

            if (a.civValue == b.civValue)
                    return a.cardName.CompareTo(b.cardName);

            return a.civValue - b.civValue;
        });
        
        for (int i = 0; i < n; i++)
        {
            manaTransforms[i].transform.SetSiblingIndex(i);
        }
    }
    
    #endregion
}
