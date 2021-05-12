using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaZoneLayoutHandler : MonoBehaviour
{
    struct TransformValuePair
    {
        public TransformValuePair(Transform transform, int value)
        {
            this.transform = transform;
            this.value = value;
        }

        public Transform transform;
        public int value;
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

    private Dictionary<int, CompactCardLayoutHandler> _cardsInManaZone = new Dictionary<int, CompactCardLayoutHandler>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0, n = _holderTransform.childCount; i < n; i++)
            {
                if (!_cardsInManaZone.ContainsKey(_holderTransform.GetChild(i).GetInstanceID()))
                {
                    AddCard(_holderTransform.GetChild(i));
                }
            }

            ArrangeCards();
        }
    }

    void AddCard(Transform cardTransform)
    {
        CompactCardLayoutHandler card = cardTransform.GetComponent<CompactCardLayoutHandler>();
        card.HoverPreview.TargetPosition = _previewTargetPosition;
        card.HoverPreview.TargetScale = _previewTargetScale;
        _cardsInManaZone.Add(cardTransform.GetInstanceID(), card);
    }

    void ArrangeCards()
    {
        RearrangeCardOrder();

        int n = _holderTransform.childCount;
        float cardWidth = Mathf.Min(_cardAreaWidth / n, _maxCardWidth);
        float sameCivWidth = Mathf.Min(cardWidth / _maxCardWidth) * _maxSameCivWidth;
        Vector3 pos = _holderTransform.localPosition;

        CompactCardLayoutHandler lastCard = null;

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _holderTransform.GetChild(i);
            CompactCardLayoutHandler currentCard =  _cardsInManaZone[cardTransform.GetInstanceID()];
            currentCard.Canvas.sortingOrder = _manaZoneSortingLayerFloor + i;

            float currentWidth = cardWidth;
            if (lastCard != null)
            {
                if (CardParams.IsCivilizationEqual(currentCard.CardData.Civilization, lastCard.CardData.Civilization))
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
            CompactCardLayoutHandler card = _cardsInManaZone[_holderTransform.GetChild(i).GetInstanceID()];
            foreach (CardParams.Civilization civilization in card.CardData.Civilization)
            {
                value += (int) civilization;
            }

            transformValuePairs[i] = new TransformValuePair(_holderTransform.GetChild(i), value);
        }

        Array.Sort(transformValuePairs, delegate(TransformValuePair a, TransformValuePair b)
        {
            return a.value - b.value;
        });

        for (int i = 0; i < n; i++)
        {
            transformValuePairs[i].transform.SetSiblingIndex(i);
        }
    }
}
