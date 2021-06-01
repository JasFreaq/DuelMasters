using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ManaZoneLayoutHandler : MonoBehaviour
{
    struct TransformValuePair
    {
        public TransformValuePair(Transform transform, int civValue)
        {
            this.transform = transform;
            this.civValue = civValue;
        }

        public Transform transform;
        public int civValue;
    }

    [SerializeField] private float _cardAreaWidth = 16;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private float _maxSameCivWidth = 2;
    [SerializeField] private bool _arrangeLeftToRight = true;
    [SerializeField] private int _manaZoneSortingLayerFloor;
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetScale;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;

    private Dictionary<int, CardManager> _cardsInManaZone = new Dictionary<int, CardManager>();

    private void Start()
    {
        CardManager card = _tempCard.gameObject.AddComponent<CardManager>();
        ManaCardLayoutHandler manaCardLayout = _tempCard.gameObject.AddComponent<ManaCardLayoutHandler>();
        card.ManaLayout = manaCardLayout;
        _cardsInManaZone.Add(_tempCard.GetInstanceID(), card);
    }

    public void AddCard(Transform cardTransform)
    {
        _tempCard.parent = transform;
        cardTransform.parent = _holderTransform;

        CardManager card = cardTransform.GetComponent<CardManager>();
        card.HoverPreviewHandler.TargetPosition = _previewTargetPosition;
        card.HoverPreviewHandler.TargetScale = _previewTargetScale;
        _cardsInManaZone.Add(cardTransform.GetInstanceID(), card);

        ArrangeCards();
    }

    public Transform AssignTempCard(Card card)
    {
        _tempCard.parent = _holderTransform;
        _cardsInManaZone[_tempCard.GetInstanceID()].SetupCard(card, true);
        ArrangeCards();
        return _tempCard;
    }

    public CardManager GetCardAtIndex(int index)
    {
        return _cardsInManaZone[_holderTransform.GetChild(index).GetInstanceID()];
    }

    public CardManager RemoveCardAtIndex(int index)
    {
        CardManager card = GetCardAtIndex(index);
        _cardsInManaZone.Remove(_holderTransform.GetChild(index).GetInstanceID());
        card.transform.parent = transform;
        ArrangeCards();
        return card;
    }

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
            CardManager currentCard =  _cardsInManaZone[cardTransform.GetInstanceID()];
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
            CardManager card = _cardsInManaZone[_holderTransform.GetChild(i).GetInstanceID()];
            foreach (CardParams.Civilization civilization in card.Card.Civilization)
            {
                value += (int) civilization;
            }

            transformValuePairs[i] = new TransformValuePair(_holderTransform.GetChild(i), value);
        }

        Array.Sort(transformValuePairs, delegate(TransformValuePair a, TransformValuePair b)
        {
            return a.civValue - b.civValue;
        });

        for (int i = 0; i < n; i++)
        {
            transformValuePairs[i].transform.SetSiblingIndex(i);
        }
    }
}
