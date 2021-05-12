using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLayoutHandler : MonoBehaviour
{
    [SerializeField] private float _circleRadius = 100;
    [SerializeField] private float _cardAreaWidth = 24;
    [SerializeField] private float _maxCardWidth = 8;
    [SerializeField] private int _handSortingLayerFloor;
    [SerializeField] private Vector3 _previewTargetPosition;
    [SerializeField] private Vector3 _previewTargetScale;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private Transform _tempCard;

    private Vector3 _circleCenter;
    private Vector3 _circleCentralAxis;

    private Dictionary<int, CardLayoutHandler> _cardsInHand = new Dictionary<int, CardLayoutHandler>();

    private void Start()
    {
        _circleCenter = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z - _circleRadius);
        _circleCentralAxis = new Vector3(_holderTransform.localPosition.x, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z) - _circleCenter;
        _circleCentralAxis.Normalize();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0, n = _holderTransform.childCount; i < n; i++)
            {
                if (!_cardsInHand.ContainsKey(_holderTransform.GetChild(i).GetInstanceID()))
                {
                    AddCard(_holderTransform.GetChild(i));
                }
            }

            ArrangeCards();
        }
    }

    void AddCard(Transform cardTransform)
    {
        CardLayoutHandler card = cardTransform.GetComponent<CardLayoutHandler>();
        card.HoverPreview.TargetPosition = _previewTargetPosition;
        card.HoverPreview.TargetScale = _previewTargetScale;
        _cardsInHand.Add(cardTransform.GetInstanceID(), card);
    }

    void ArrangeCards()
    {
        int n = _holderTransform.childCount;
        float cardWidth = Mathf.Min((_cardAreaWidth * 2) / n, _maxCardWidth);
        float startOffset = (n % 2) * cardWidth;
        if (n % 2 == 0)
            startOffset += cardWidth / 2;
        Vector3 startPos = new Vector3(_holderTransform.localPosition.x - startOffset, _holderTransform.localPosition.y,
            _holderTransform.localPosition.z);

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _holderTransform.GetChild(i);
            _cardsInHand[cardTransform.GetInstanceID()].Canvas.sortingOrder = _handSortingLayerFloor + i;
            Vector3 cardPos = new Vector3(startPos.x + (i - n / 2 + 1) * cardWidth, startPos.y, startPos.z);
            
            Vector3 relativeVector = cardPos - _circleCenter;
            relativeVector.Normalize();

            cardTransform.localEulerAngles = new Vector3(cardTransform.localEulerAngles.x, Vector3.SignedAngle(relativeVector, _circleCentralAxis,_holderTransform.up),
                cardTransform.localEulerAngles.z);

            cardPos = relativeVector * _circleRadius;
            cardPos.z -= _circleRadius;
            cardTransform.localPosition = cardPos;
        }
    }
}
