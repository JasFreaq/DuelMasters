using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaZoneHandler : MonoBehaviour
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
    [SerializeField] private int _manaZoneSortingLayerFloor;
    
    private Dictionary<int, CompactCardObject> _cardsInManaZone = new Dictionary<int, CompactCardObject>();
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0, n = transform.childCount; i < n; i++)
            {
                CompactCardObject card = transform.GetChild(i).GetComponent<CompactCardObject>();
                if (!_cardsInManaZone.ContainsKey(transform.GetChild(i).GetInstanceID()))
                    _cardsInManaZone.Add(transform.GetChild(i).GetInstanceID(), card);
            }

            ArrangeCards();
        }
    }

    void ArrangeCards()
    {
        RearrangeCardOrder();

        int n = transform.childCount;
        float cardWidth = Mathf.Min(_cardAreaWidth / n, _maxCardWidth);
        float sameCivWidth = Mathf.Min(cardWidth / _maxCardWidth) * _maxSameCivWidth;
        Vector3 pos = transform.position;

        CompactCardObject lastCard = null;

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = transform.GetChild(i);
            CompactCardObject currentCard =  _cardsInManaZone[cardTransform.GetInstanceID()];
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
                pos.x += currentWidth;
            cardTransform.position = pos;
            pos = cardTransform.position;
            lastCard = currentCard;
        }
    }

    void RearrangeCardOrder()
    {
        int n = transform.childCount;
        TransformValuePair[] transformValuePairs = new TransformValuePair[n];
        for (int i = 0; i < n; i++)
        {
            int value = 0;
            CompactCardObject card = _cardsInManaZone[transform.GetChild(i).GetInstanceID()];
            foreach (CardParams.Civilization civilization in card.CardData.Civilization)
            {
                value += (int) civilization;
            }

            transformValuePairs[i] = new TransformValuePair(transform.GetChild(i), value);
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
