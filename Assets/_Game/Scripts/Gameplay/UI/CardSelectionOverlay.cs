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
    [SerializeField] private TextMeshProUGUI _submitNumText;

    [Header("Layout")]
    [SerializeField] private float _circleRadius = 150;
    [SerializeField] private float _cardAreaWidth = 28;
    [SerializeField] private int _maxCardNo = 7;
    [SerializeField] private int _overlaySortingLayerFloor = 100;
    [SerializeField] private float _previewScaleMultiplier = 50;
    [SerializeField] private Transform _holderTransform;
    
    private int _selectionNum = 0, _lowerBound, _upperBound;
    private bool _selectionMade = false;

    private Vector3 _circleCenter;
    private Vector3 _circleCentralAxis;

    private void Start()
    {
        _holderCanvas.SetActive(false);

        Vector3 holderPosition = _holderTransform.localPosition;
        _circleCenter = new Vector3(holderPosition.x, holderPosition.y - _circleRadius, holderPosition.z);
        _circleCentralAxis = new Vector3(holderPosition.x, holderPosition.y, holderPosition.z) - _circleCenter;
        _circleCentralAxis.Normalize();
    }

    public IEnumerator GenerateLayout(int lower, int upper, List<CardObject> cardList)
    {
        _lowerBound = lower;
        _upperBound = upper;

        List<Canvas> previewCanvases = new List<Canvas>();
        foreach (CardObject cardObj in cardList)
        {
            Canvas previewCanvas = cardObj.PreviewCardLayout.Canvas;
            Transform previewTransform = previewCanvas.transform;

            previewTransform.SetParent(_holderTransform);
            previewTransform.localRotation = Quaternion.Euler(Vector3.zero);
            previewTransform.localScale = OriginalPreviewScale * new Vector2(_previewScaleMultiplier, _previewScaleMultiplier);
            
            previewCanvases.Add(previewCanvas);
        }

        _holderCanvas.SetActive(true);

        while (!_selectionMade) 
        {
            ArrangeCards(previewCanvases);
            yield return new WaitForEndOfFrame();
        }
        
        foreach (CardObject cardObj in cardList)
        {
            Canvas previewCanvas = cardObj.PreviewCardLayout.Canvas;
            Transform previewTransform = previewCanvas.transform;

            previewTransform.SetParent(cardObj.PreviewCardLayout.transform);
            previewCanvas.renderMode = RenderMode.WorldSpace;

            previewTransform.localPosition = Vector3.zero;
            previewTransform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            previewTransform.localScale = OriginalPreviewScale;
        }

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

    private void ArrangeCards(List<Canvas> previewCanvases)
    {
        Vector3 holderPosition2 = _holderTransform.localPosition;
        _circleCenter = new Vector3(holderPosition2.x, holderPosition2.y - _circleRadius, holderPosition2.z);
        _circleCentralAxis = new Vector3(holderPosition2.x, holderPosition2.y, holderPosition2.z) - _circleCenter;
        _circleCentralAxis.Normalize();

        float cardWidth = (_cardAreaWidth * 2) / _maxCardNo;
        float startOffset = (_maxCardNo % 2) * cardWidth;
        
        Vector3 holderPosition = _holderTransform.localPosition;
        Vector3 startPos = new Vector3(holderPosition.x - startOffset, holderPosition.y, holderPosition.z);

        for (int i = 0, n = previewCanvases.Count; i < n; i++) 
        {
            Transform cardTransform = previewCanvases[i].transform;

            float offset = (i - n / 2 + 1) * cardWidth;
            Vector3 cardPos = new Vector3(startPos.x + offset, startPos.y, startPos.z);

            Vector3 relativeVector = cardPos - _circleCenter;
            relativeVector.Normalize();

            Vector3 rotation = new Vector3(0, 0,
                Vector3.SignedAngle(relativeVector, _circleCentralAxis, -_holderTransform.forward));

            cardPos = relativeVector * _circleRadius;
            cardPos.y -= _circleRadius;

            Quaternion cardRot = Quaternion.Euler(rotation);
            
            cardTransform.localPosition = cardPos;
            cardTransform.localRotation = cardRot;
        }
    }

    public void MakeSelection()
    {
        _holderCanvas.SetActive(false);
        _selectionMade = true;
    }
}
