using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHandler : MonoBehaviour
{
    [SerializeField] private float _circleRadius = 100;
    [SerializeField] private float _cardAreaWidth = 24;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private int _handSortingLayerFloor;

    private Vector3 _circleCenter;
    private Vector3 _circleCentralAxis;

    private Dictionary<int, CardObject> _cardsInHand = new Dictionary<int, CardObject>();

    private void Start()
    {
        _circleCenter = new Vector3(transform.position.x, transform.position.y - _circleRadius);
        _circleCentralAxis = new Vector3(transform.position.x, transform.position.y) - _circleCenter;
        _circleCentralAxis.Normalize();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0, n = transform.childCount; i < n; i++)
            {
                CardObject card = transform.GetChild(i).GetComponent<CardObject>();
                _cardsInHand.Add(transform.GetChild(i).GetInstanceID(), card);
            }

            ArrangeCards();
        }
    }

    void ArrangeCards()
    {
        int n = transform.childCount;
        float cardWidth = Mathf.Min((_cardAreaWidth * 2) / n, _maxCardWidth);
        float startOffset = (n % 2) * cardWidth;
        if (n % 2 == 0)
            startOffset += cardWidth / 2;
        Vector2 startPos = new Vector2(transform.position.x - startOffset, transform.position.y);

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = transform.GetChild(i);
            _cardsInHand[cardTransform.GetInstanceID()].Canvas.sortingOrder = _handSortingLayerFloor + i;
            Vector3 cardPos = new Vector3(startPos.x + (i - n / 2 + 1) * cardWidth, startPos.y);
            
            Vector3 relativeVector = cardPos - _circleCenter;
            relativeVector.Normalize();

            cardTransform.localEulerAngles = new Vector3(cardTransform.localEulerAngles.x, Vector2.SignedAngle(relativeVector, _circleCentralAxis),
                cardTransform.localEulerAngles.z);

            cardPos = relativeVector * _circleRadius;
            cardPos.y -= _circleRadius;
            cardPos += transform.position;
            cardTransform.position = cardPos;
        }
    }
}
