using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSelectionOverlay : MonoBehaviour
{
    private static Vector3 OriginalPreviewScale = new Vector3(0.0184625f, 0.018035f, 1f);
    private static string SubmitText = "Submit";

    [SerializeField] private GameObject _holderCanvas;
    [SerializeField] private GameObject _layoutHolder;
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
    [SerializeField] private int _overlaySortingLayerCeiling = 200;
    [SerializeField] private float _previewScaleMultiplier = 50;
    [SerializeField] private List<Vector3> _edgePosAdjustments = new List<Vector3>(3);
    [SerializeField] private float _edgeRotZAdjustment = 15;
    [SerializeField] private Transform _holderTransform;
    [SerializeField] private float _hoverScaleMultiplier = 1.5f;
    [SerializeField] private float _resetBufferTime = 0.5f;

    private List<CardObject> _selectedCards = new List<CardObject>();
    private List<Canvas> _previewCanvases = new List<Canvas>();

    private Action<bool> _onToggleTabAction;

    private int _lowerBound, _upperBound, _adjustedDisplayNo;
    private bool _overlayActive, _selectionMade;
    
    private Vector3 _circleCenter;
    private Vector3 _circleCentralAxis;

    private void Start()
    {
        _holderCanvas.SetActive(false);

        _layoutScroll.onValueChanged.AddListener(ArrangePreviews);

        Vector3 holderPosition = _holderTransform.localPosition;
        _circleCenter = new Vector3(holderPosition.x, holderPosition.y - _circleRadius, holderPosition.z);
        _circleCentralAxis = new Vector3(holderPosition.x, holderPosition.y, holderPosition.z) - _circleCenter;
        _circleCentralAxis.Normalize();
    }

    public Coroutine GenerateLayout(int lower, int upper, List<CardObject> cardList)
    {
        return StartCoroutine(GenerateLayoutRoutine(lower, upper, cardList));
    }

    public void ToggleTab(bool enable)
    {
        _layoutHolder.SetActive(enable);
        _onToggleTabAction.Invoke(!enable);
    }

    public void SubmitSelection()
    {
        _holderCanvas.SetActive(false);
        _selectionMade = true;
    }
    
    public void CancelSelection()
    {
        _selectedCards.Clear();
        SubmitSelection();
    }

    #region Functionality Methods

    private IEnumerator GenerateLayoutRoutine(int lower, int upper, List<CardObject> cardList)
    {
        _lowerBound = lower;
        _upperBound = upper;
        _submitText.text = $"{SubmitText} 0";
        _submitButton.interactable = false;
        _layoutHolder.SetActive(true);

        SetCards(cardList);
        _overlayActive = true;
        
        foreach (Canvas previewCanvas in _previewCanvases)
            previewCanvas.overrideSorting = true;

        ArrangePreviews();
        while (!_selectionMade)
            yield return new WaitForEndOfFrame();

        List<CardObject> selectedCards = new List<CardObject>(_selectedCards);
        foreach (CardObject cardObj in _selectedCards)
            cardObj.PreviewLayoutHandler.SetHighlight(false);

        yield return ResetCardsRoutine(cardList);
        
        _selectedCards.Clear();
        _previewCanvases.Clear();
        _selectionMade = false;
        _overlayActive = false;

        yield return selectedCards;
    }

    private void SetCards(List<CardObject> cardList)
    {
        Vector3 previewScale = OriginalPreviewScale * new Vector2(_previewScaleMultiplier, _previewScaleMultiplier);
        foreach (CardObject cardObj in cardList)
        {
            cardObj.HoverPreviewHandler.PreviewEnabled = true;
            cardObj.HoverPreviewHandler.InPlayerHand = false;
            cardObj.HoverPreviewHandler.InCardSelection = true;
            cardObj.HoverPreviewHandler.SetPreviewParameters(_overlaySortingLayerCeiling, previewScale,
                previewScale * _hoverScaleMultiplier);

            cardObj.PreviewLayoutHandler.EnableCanvasEventTrigger(true);
            cardObj.PreviewLayoutHandler.AddOnClickEvent(SelectCard, cardObj);

            Canvas previewCanvas = cardObj.PreviewLayoutHandler.Canvas;
            Transform previewTransform = previewCanvas.transform;

            previewTransform.SetParent(_holderTransform);
            previewTransform.localRotation = Quaternion.Euler(Vector3.zero);
            previewTransform.localScale = previewScale;

            _previewCanvases.Add(previewCanvas);
        }

        AdjustLayout();

        _holderCanvas.SetActive(true);
    }

    private IEnumerator ResetCardsRoutine(List<CardObject> cardList)
    {
        foreach (CardObject cardObj in cardList)
        {
            cardObj.HoverPreviewHandler.ResetPreviewEnabled();
            cardObj.HoverPreviewHandler.ResetInPlayerHand();
            cardObj.HoverPreviewHandler.InCardSelection = false;

            cardObj.PreviewLayoutHandler.ResetStates();
            cardObj.PreviewLayoutHandler.EnableCanvasEventTrigger(false);
            cardObj.PreviewLayoutHandler.RemoveOnClickEvent();

            Canvas previewCanvas = cardObj.PreviewLayoutHandler.Canvas;
            Transform previewTransform = previewCanvas.transform;

            previewTransform.SetParent(cardObj.PreviewLayoutHandler.transform);
            previewTransform.gameObject.SetActive(false);

            previewCanvas.renderMode = RenderMode.WorldSpace;
            previewCanvas.sortingOrder = 0;
        }

        yield return new WaitForSeconds(_resetBufferTime);

        foreach (CardObject cardObj in cardList)
        {
            Transform previewTransform = cardObj.PreviewLayoutHandler.Canvas.transform;

            previewTransform.gameObject.SetActive(true);
            previewTransform.localPosition = Vector3.zero;
            previewTransform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            previewTransform.localScale = OriginalPreviewScale;
        }
    }

    private void SelectCard(CardObject cardObj)
    {
        int selectionCount = _selectedCards.Count;

        PreviewLayoutHandler previewLayout = cardObj.PreviewLayoutHandler;
        if (previewLayout.IsHighlighted)
        {
            previewLayout.SetHighlight(false);
            _selectedCards.Remove(cardObj);
        }
        else if (previewLayout.IsValid && selectionCount < _upperBound) 
        {
            previewLayout.SetHighlight(true);
            _selectedCards.Add(cardObj);
        }

        selectionCount = _selectedCards.Count;
        if (selectionCount >= _lowerBound && selectionCount <= _upperBound)
        {
            if (!_submitButton.interactable)
                _submitButton.interactable = true;
        }
        else
        {
            if (_submitButton.interactable)
                _submitButton.interactable = false;
        }
        _submitText.text = $"{SubmitText} {selectionCount}";
    }

    #endregion

    #region Layout Methods

    private void AdjustLayout()
    {
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
    }

    private void ArrangePreviews(float scrollValue)
    {
        ArrangePreviews();
    }

    private void ArrangePreviews()
    {
        float cardWidth = (_cardAreaWidth * 2) / _adjustedDisplayNo;
        float extremeOffset = ((float)_adjustedDisplayNo / 2 + 1) * cardWidth;

        Vector3 holderPosition = _holderTransform.localPosition;
        Vector3 startPos = new Vector3(holderPosition.x - cardWidth, holderPosition.y, holderPosition.z);

        int n = _previewCanvases.Count, m = n - 1 - _adjustedDisplayNo;
        float adjustedScrollVal = m * _layoutScroll.value;

        for (int i = 0; i < n; i++)
        {
            Canvas previewCanvas = _previewCanvases[i];
            Transform previewTransform = previewCanvas.transform;
            Vector3 previewPos = Vector3.zero;

            Vector3 posAddendum = Vector3.zero;
            float rotAddendum = 0;

            if (i < adjustedScrollVal)
            {
                previewPos = new Vector3(startPos.x + extremeOffset, startPos.y, startPos.z);
                previewPos.x = -previewPos.x;

                Vector3 edgePosAdjustment = new Vector3(-_edgePosAdjustments[0].x, _edgePosAdjustments[0].y, _edgePosAdjustments[0].z);
                Vector3 edgePosAdjustmentA = new Vector3(-_edgePosAdjustments[1].x, _edgePosAdjustments[1].y, _edgePosAdjustments[1].z);
                Vector3 edgePosAdjustmentB = new Vector3(-_edgePosAdjustments[2].x, _edgePosAdjustments[2].y, _edgePosAdjustments[2].z);

                float diffVal = Mathf.Abs(i - adjustedScrollVal);

                if (diffVal >= 4)
                {
                    if (previewTransform.gameObject.activeInHierarchy)
                        previewTransform.gameObject.SetActive(false);
                }
                else if (diffVal < 4 && diffVal > 1)
                {
                    rotAddendum = _edgeRotZAdjustment;

                    if (diffVal >= 2)
                    {
                        if (diffVal >= 3)
                        {
                            if (previewCanvas.sortingOrder != _overlaySortingLayerFloor - 3)
                                previewCanvas.sortingOrder = _overlaySortingLayerFloor - 3;

                            posAddendum = edgePosAdjustmentB;
                        }
                        else
                        {
                            if (previewCanvas.sortingOrder != _overlaySortingLayerFloor - 2)
                                previewCanvas.sortingOrder = _overlaySortingLayerFloor - 2;

                            posAddendum = Vector3.Lerp(edgePosAdjustmentA, edgePosAdjustmentB, diffVal - 2);
                        }
                    }
                    else
                    {
                        if (previewCanvas.sortingOrder != _overlaySortingLayerFloor - 1)
                            previewCanvas.sortingOrder = _overlaySortingLayerFloor - 1;

                        posAddendum = Vector3.Lerp(edgePosAdjustment, edgePosAdjustmentA, diffVal - 1);
                    }

                    if (!previewTransform.gameObject.activeInHierarchy)
                        previewTransform.gameObject.SetActive(true);
                }
                else if (diffVal <= 1)
                {
                    if (previewCanvas.sortingOrder != _overlaySortingLayerFloor)
                        previewCanvas.sortingOrder = _overlaySortingLayerFloor;

                    rotAddendum = Mathf.Lerp(0, _edgeRotZAdjustment, diffVal);
                    posAddendum = Vector3.Lerp(Vector3.zero, edgePosAdjustment, diffVal);
                }
            }
            else if (i >= adjustedScrollVal && i <= adjustedScrollVal + _adjustedDisplayNo)
            {
                previewCanvas.sortingOrder = _overlaySortingLayerFloor + i;

                float offset = (i - adjustedScrollVal - (float)_adjustedDisplayNo / 2 + 1) * cardWidth;
                previewPos = new Vector3(startPos.x + offset, startPos.y, startPos.z);
            }
            else if (i > adjustedScrollVal + _adjustedDisplayNo)
            {
                previewPos = new Vector3(startPos.x + extremeOffset, startPos.y, startPos.z);

                float diffVal = Mathf.Abs(i - (adjustedScrollVal + _adjustedDisplayNo));
                if (diffVal >= 4)
                {
                    if (previewTransform.gameObject.activeInHierarchy)
                        previewTransform.gameObject.SetActive(false);
                }
                else if (diffVal < 4 && diffVal > 1)
                {
                    rotAddendum = -_edgeRotZAdjustment;

                    if (diffVal >= 2)
                    {
                        if (diffVal >= 3)
                        {
                            if (previewCanvas.sortingOrder != _overlaySortingLayerFloor - 3)
                                previewCanvas.sortingOrder = _overlaySortingLayerFloor - 3;

                            posAddendum = _edgePosAdjustments[2];
                        }
                        else
                        {
                            if (previewCanvas.sortingOrder != _overlaySortingLayerFloor - 2)
                                previewCanvas.sortingOrder = _overlaySortingLayerFloor - 2;

                            posAddendum = Vector3.Lerp(_edgePosAdjustments[1], _edgePosAdjustments[2], diffVal - 2);
                        }
                    }
                    else
                    {
                        if (previewCanvas.sortingOrder != _overlaySortingLayerFloor - 1)
                            previewCanvas.sortingOrder = _overlaySortingLayerFloor - 1;

                        posAddendum = Vector3.Lerp(_edgePosAdjustments[0], _edgePosAdjustments[1], diffVal - 1);
                    }


                    if (!previewTransform.gameObject.activeInHierarchy)
                        previewTransform.gameObject.SetActive(true);
                }
                else if (diffVal <= 1)
                {
                    if (previewCanvas.sortingOrder != _overlaySortingLayerFloor)
                        previewCanvas.sortingOrder = _overlaySortingLayerFloor;

                    rotAddendum = Mathf.Lerp(0, -_edgeRotZAdjustment, diffVal);
                    posAddendum = Vector3.Lerp(Vector3.zero, _edgePosAdjustments[0], diffVal);
                }
            }

            Vector3 relativeVector = previewPos - _circleCenter;
            relativeVector.Normalize();

            Vector3 rotation = new Vector3(0, 0,
                Vector3.SignedAngle(relativeVector, _circleCentralAxis, -_holderTransform.forward));

            previewPos = relativeVector * _circleRadius;
            previewPos.y -= _circleRadius;

            Quaternion cardRot = Quaternion.Euler(rotation + new Vector3(0, 0, rotAddendum));

            previewTransform.localPosition = previewPos + posAddendum;
            previewTransform.localRotation = cardRot;
        }
    }

    #endregion

    public void RegisterOnToggleTab(Action<bool> action)
    {
        _onToggleTabAction += action;
    }
    
    public void DeregisterOnToggleTab(Action<bool> action)
    {
        _onToggleTabAction -= action;
    }
}
