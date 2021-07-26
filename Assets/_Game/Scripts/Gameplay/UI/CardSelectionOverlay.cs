using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionOverlay : MonoBehaviour
{
    private static Vector3 OriginalPreviewScale = new Vector3(0.0184625f, 0.018035f, 1f);

    [SerializeField] private GameObject _holderCanvas;
    [SerializeField] private TextMeshProUGUI _chooseText;
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private Scrollbar _layoutScroll;
    [SerializeField] private Button _submitButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private TextMeshProUGUI _submitText;

    [Header("Layout")]
    [SerializeField] private float _circleRadius = 150;
    [SerializeField] private float _cardAreaWidth = 28;
    [SerializeField] private int _displayCardsNo = 7;
    [SerializeField] private int _overlaySortingLayerFloor = 100;
    [SerializeField] private float _previewScaleMultiplier = 50;
    [SerializeField] private Vector3 _edgePosAdjustment = new Vector3(50, 50, 0);
    [SerializeField] private Vector2 _edgePosDeviationRange = new Vector2(10, 10);
    [SerializeField] private float _edgeRotZAdjustment = 15;
    [SerializeField] private Transform _holderTransform;

    private List<Canvas> _previewCanvases = new List<Canvas>();

    private int _selectionNum = 0, _lowerBound, _upperBound, _adjustedDisplayNo;
    private bool _selectionMade = false;

    private List<Vector2> _edgeRandomDeviations = new List<Vector2>();

    private Vector3 _circleCenter;
    private Vector3 _circleCentralAxis;

    private void Start()
    {
        _holderCanvas.SetActive(false);

        _layoutScroll.onValueChanged.AddListener(ArrangeCards);

        Vector3 holderPosition = _holderTransform.localPosition;
        _circleCenter = new Vector3(holderPosition.x, holderPosition.y - _circleRadius, holderPosition.z);
        _circleCentralAxis = new Vector3(holderPosition.x, holderPosition.y, holderPosition.z) - _circleCenter;
        _circleCentralAxis.Normalize();
    }

    public IEnumerator GenerateLayout(int lower, int upper, List<CardObject> cardList)
    {
        _lowerBound = lower;
        _upperBound = upper;

        _previewCanvases = new List<Canvas>();
        foreach (CardObject cardObj in cardList)
        {
            Canvas previewCanvas = cardObj.PreviewHandler.Canvas;
            Transform previewTransform = previewCanvas.transform;

            previewTransform.SetParent(_holderTransform);
            previewTransform.localRotation = Quaternion.Euler(Vector3.zero);
            previewTransform.localScale = OriginalPreviewScale * new Vector2(_previewScaleMultiplier, _previewScaleMultiplier);
            
            _previewCanvases.Add(previewCanvas);
        }

        if (_previewCanvases.Count <= _displayCardsNo)
        {
            _adjustedDisplayNo = _previewCanvases.Count;
            _layoutScroll.value = 0.5f;
            _layoutScroll.gameObject.SetActive(false);
        }
        else
        {
            _adjustedDisplayNo = _displayCardsNo - 1;
            _layoutScroll.value = 1;
            _layoutScroll.gameObject.SetActive(true);
        }

        _edgeRandomDeviations.Add(CalculateDeviation());
        _edgeRandomDeviations.Add(_edgeRandomDeviations[0] + CalculateDeviation());

        _holderCanvas.SetActive(true);

        ArrangeCards(_layoutScroll.value);
        while (!_selectionMade)
        {
            yield return new WaitForEndOfFrame();
        }
        
        foreach (CardObject cardObj in cardList)
        {
            Canvas previewCanvas = cardObj.PreviewHandler.Canvas;
            Transform previewTransform = previewCanvas.transform;

            previewTransform.SetParent(cardObj.PreviewHandler.transform);
            previewCanvas.renderMode = RenderMode.WorldSpace;

            previewTransform.localPosition = Vector3.zero;
            previewTransform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            previewTransform.localScale = OriginalPreviewScale;
        }

        _previewCanvases.Clear();
        _edgeRandomDeviations.Clear();
        _selectionMade = false;

        //yield return 
    }

    public IEnumerator GenerateLayout(int lower, int upper, Dictionary<int, CardObject> cardDict)
    {
        List<CardObject> cardList = new List<CardObject>();
        foreach (KeyValuePair<int, CardObject> pair in cardDict)
        {
            CardObject cardObj = pair.Value;
            cardList.Add(cardObj);
        }

        yield return GenerateLayout(lower, upper, cardList);
    }

    private void ArrangeCards(float scrollValue)
    {
        ArrangeCards();
    }

    private void ArrangeCards()
    {
        float cardWidth = (_cardAreaWidth * 2) / _adjustedDisplayNo;
        float extremeOffset = ((float) _adjustedDisplayNo / 2 + 1) * cardWidth;

        Vector3 holderPosition = _holderTransform.localPosition;
        Vector3 startPos = new Vector3(holderPosition.x - cardWidth, holderPosition.y, holderPosition.z);

        int n = _previewCanvases.Count, m = n - 1 - _adjustedDisplayNo;
        float adjustedScrollVal = m * _layoutScroll.value;

        for (int i = 0; i < n; i++)
        {
            Transform cardTransform = _previewCanvases[i].transform;
            Vector3 cardPos = Vector3.zero;

            Vector3 posAddendum = Vector3.zero;
            float rotAddendum = 0;

            if (i < adjustedScrollVal)
            {
                cardPos = new Vector3(startPos.x + extremeOffset, startPos.y, startPos.z);
                cardPos.x = -cardPos.x;

                Vector3 edgePosAdjustment = new Vector3(-_edgePosAdjustment.x, _edgePosAdjustment.y, _edgePosAdjustment.z);
                Vector3 edgePosAdjustmentA = new Vector3(edgePosAdjustment.x - _edgeRandomDeviations[0].x,
                    edgePosAdjustment.y + _edgeRandomDeviations[0].y, edgePosAdjustment.z);
                Vector3 edgePosAdjustmentB = new Vector3(edgePosAdjustmentA.x - _edgeRandomDeviations[1].x,
                    edgePosAdjustmentA.y + _edgeRandomDeviations[1].y, edgePosAdjustmentA.z);

                float diffVal = Mathf.Abs(i - adjustedScrollVal);

                if (diffVal >= 4)
                {
                    if (cardTransform.gameObject.activeInHierarchy)
                        cardTransform.gameObject.SetActive(false);
                }
                else if (diffVal < 4 && diffVal > 1)
                {
                    rotAddendum = _edgeRotZAdjustment;
                    
                    if (diffVal >= 2)
                    {
                        if (diffVal >= 3)
                            posAddendum = edgePosAdjustmentB;
                        else
                            posAddendum = Vector3.Lerp(edgePosAdjustmentA, edgePosAdjustmentB, diffVal - 2);
                    }
                    else
                        posAddendum = Vector3.Lerp(edgePosAdjustment, edgePosAdjustmentA, diffVal - 1);
                    

                    if (!cardTransform.gameObject.activeInHierarchy)
                        cardTransform.gameObject.SetActive(true);
                }
                else if (diffVal <= 1)
                {
                    rotAddendum = Mathf.Lerp(0, _edgeRotZAdjustment, diffVal);
                    posAddendum = Vector3.Lerp(Vector3.zero, edgePosAdjustment, diffVal);
                }
            }
            else if (i >= adjustedScrollVal && i <= adjustedScrollVal + _adjustedDisplayNo)
            {
                float offset = (i - adjustedScrollVal - (float) _adjustedDisplayNo / 2 + 1) * cardWidth;
                cardPos = new Vector3(startPos.x + offset, startPos.y, startPos.z);
            }
            else if (i > adjustedScrollVal + _adjustedDisplayNo)
            {
                cardPos = new Vector3(startPos.x + extremeOffset, startPos.y, startPos.z);

                float diffVal = Mathf.Abs(i - (adjustedScrollVal + _adjustedDisplayNo));
                if (diffVal > 2)
                {
                    if (cardTransform.gameObject.activeInHierarchy)
                        cardTransform.gameObject.SetActive(false);
                }
                else if (diffVal <= 2 && diffVal > 1)
                {
                    rotAddendum = -_edgeRotZAdjustment;
                    posAddendum = _edgePosAdjustment;

                    if (!cardTransform.gameObject.activeInHierarchy)
                        cardTransform.gameObject.SetActive(true);
                }
                else if (diffVal < 1) 
                {
                    rotAddendum = Mathf.Lerp(0, -_edgeRotZAdjustment, diffVal);
                    posAddendum = Vector3.Lerp(Vector3.zero, _edgePosAdjustment, diffVal);
                }
            }
            
            Vector3 relativeVector = cardPos - _circleCenter;
            relativeVector.Normalize();

            Vector3 rotation = new Vector3(0, 0,
                Vector3.SignedAngle(relativeVector, _circleCentralAxis, -_holderTransform.forward));

            cardPos = relativeVector * _circleRadius;
            cardPos.y -= _circleRadius;

            Quaternion cardRot = Quaternion.Euler(rotation + new Vector3(0, 0, rotAddendum));

            cardTransform.localPosition = cardPos + posAddendum;
            cardTransform.localRotation = cardRot;
        }
    }

    private Vector2 CalculateDeviation()
    {
        Vector2 vector2 = new Vector2();
        vector2.x = Random.Range(_edgePosDeviationRange.x / 2, _edgePosDeviationRange.x);
        vector2.y = Random.Range(-_edgePosDeviationRange.y, _edgePosDeviationRange.y);

        return vector2;
    }

    public void MakeSelection()
    {
        _holderCanvas.SetActive(false);
        _selectionMade = true;
    }
}
