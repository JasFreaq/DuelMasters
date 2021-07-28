using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HoverPreviewHandler: MonoBehaviour
{
    private static Camera _MainCamera = null;
    
    private PreviewLayoutHandler _previewLayoutHandler;
    
    private Vector3 _handPreviewPosition = Vector3.zero;
    private Quaternion _handPreviewRotation = Quaternion.identity;
    private Vector3 _handPreviewScale = Vector3.one, _cardSelectionPreviewScale = Vector3.one;

    private Vector3 _handOriginalPosition = Vector3.zero;
    private Quaternion _handOriginalRotation = Quaternion.identity, _cardSelectionOriginalRotation = Quaternion.identity;
    private Vector3 _cardSelectionOriginalScale = Vector3.one;

    private int _cardSelectionMaxSortOrder, _cardSelectionOriginalSortOrder;

    private bool _previewEnabled, _previewWasEnabled;
    private bool _inPlayerHand, _wasInPlayerHand;
    private bool _inCardSelection;
    private bool _isPreviewing;
    private bool _shouldStopPreview = true;

    private Action _onBeginPlayerHandPreview;

    private bool _isOverCollider = false;
    private Coroutine _previewRoutine = null;
    private Coroutine _previewStopRoutine = null;

    public PreviewLayoutHandler PreviewLayoutHandler
    {
        set { _previewLayoutHandler = value; }
    }

    public bool InPlayerHand
    {
        set
        {
            _wasInPlayerHand = _inPlayerHand;
            _inPlayerHand = value;
        }
    }
    
    public bool InCardSelection
    {
        set { _inCardSelection = value; }
    }

    public bool ShouldStopPreview
    {
        set { _shouldStopPreview = value; }
    }

    public bool PreviewEnabled
    {
        get { return _previewEnabled; }

        set
        {
            _previewWasEnabled = _previewEnabled;
            _previewEnabled = value;
            if (!_previewEnabled)
                StopThisPreview();
        }
    }

    #region Static Data Members

    private static HoverPreviewHandler _CurrentlyViewing = null;
    private static HoverPreviewHandler[] _AllHoverPreviews = null;
    
    #endregion

    private void Start()
    {
        if (_AllHoverPreviews == null)
        {
            _AllHoverPreviews = FindObjectsOfType<HoverPreviewHandler>();
        }

        if (!_MainCamera)
        {
            _MainCamera = Camera.main;
        }
    }
    
    public void BeginPreviewing()
    {
        _isOverCollider = true;
        
        if (_previewEnabled)
        {
            _previewRoutine = StartCoroutine(StartPreviewRoutine());
        }
    }

    public void EndPreviewing()
    {
        _isOverCollider = false;

        if (_previewRoutine != null)
        {
            StopCoroutine(_previewRoutine);
            _previewRoutine = null;
        }
        else if (!PreviewingSomeCard())
        {
            if (_shouldStopPreview)
                StopPreview();
            else if (_isPreviewing)
                _isPreviewing = false;
        }
    }

    public void SetPreviewParameters(Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale)
    {
        _handPreviewPosition = targetPosition;
        _handPreviewRotation = targetRotation;
        _handPreviewScale = targetScale;
    }

    public void SetPreviewParameters(int maxSortOrder, Vector3 originalScale, Vector3 targetScale)
    {
        _cardSelectionMaxSortOrder = maxSortOrder;
        _cardSelectionOriginalScale = originalScale;
        _cardSelectionPreviewScale = targetScale;
    }

    private IEnumerator StartPreviewRoutine()
    {
        if (!(_inPlayerHand || _inCardSelection)) 
            yield return new WaitForSeconds(GameParamsHolder.Instance.HoverBeforePreviewTime);
        
        // save this HoverPreview as current
        _CurrentlyViewing = this;

        PreviewThisObject();
        _previewRoutine = null;
    }

    private void PreviewThisObject()
    {
        // first disable the previous preview if there is one already
        if (_CurrentlyViewing != this)
            StopPreview();
        
        float transitionTime = GameParamsHolder.Instance.PreviewTransitionTime;

        // tween to target position
        if (_inPlayerHand)
        {
            if (!_isPreviewing) 
            {
                _handOriginalPosition = transform.position;
                _handOriginalRotation = transform.rotation;
                _onBeginPlayerHandPreview.Invoke();
                _isPreviewing = true;
            }

            if (_previewStopRoutine != null)
                StopCoroutine(_previewStopRoutine);

            Vector3 previewPosition = _handPreviewPosition;
            previewPosition.x = transform.position.x;

            transform.DOMove(previewPosition, transitionTime).SetEase(Ease.OutSine);
            transform.DORotateQuaternion(_handPreviewRotation, transitionTime).SetEase(Ease.OutSine);
            transform.DOScale(_handPreviewScale, transitionTime).SetEase(Ease.OutSine);
        }
        else if (_inCardSelection)
        {
            Canvas previewCanvas = _previewLayoutHandler.Canvas;
            Transform previewTransform = previewCanvas.transform;

            if (!_isPreviewing)
            {
                _cardSelectionOriginalSortOrder = previewCanvas.sortingOrder;
                previewCanvas.sortingOrder = _cardSelectionMaxSortOrder;
                _cardSelectionOriginalRotation = previewTransform.rotation;
                _isPreviewing = true;
            }

            if (_previewStopRoutine != null)
                StopCoroutine(_previewStopRoutine);

            previewTransform.DORotateQuaternion(Quaternion.Euler(Vector3.zero), transitionTime).SetEase(Ease.OutSine);
            previewTransform.DOScale(_cardSelectionPreviewScale, transitionTime).SetEase(Ease.OutSine);
        }
        else
        {
            if (!_isPreviewing)
                _isPreviewing = true;
            
            Transform hoverIntermediate = GameParamsHolder.Instance.HoverIntermediateTransform;

            //Flip X of position if Preview overlaps transform in ScreenPoint
            Vector3 intermediatePosition = hoverIntermediate.position;
            if (_MainCamera.WorldToScreenPoint(transform.position).x / Screen.width >
                GameParamsHolder.Instance.PreviewSideBoundFraction) 
                intermediatePosition.x = -intermediatePosition.x;

            Transform previewTransform = _previewLayoutHandler.transform;

            previewTransform.position = intermediatePosition;
            previewTransform.transform.rotation = hoverIntermediate.rotation;

            //Adjust Scale
            Vector3 intermediateLossyScale = hoverIntermediate.lossyScale;
            previewTransform.localScale = Vector3.one;
            Vector3 previewLossyScale = previewTransform.lossyScale;

            previewTransform.localScale = new Vector3(intermediateLossyScale.x / previewLossyScale.x, 
                intermediateLossyScale.y / previewLossyScale.y, intermediateLossyScale.z / previewLossyScale.z);
            
            _previewLayoutHandler.gameObject.SetActive(true);
        }
    }

    private void StopThisPreview()
    {
        if (_previewRoutine != null) 
        {
            StopCoroutine(_previewRoutine);
            _previewRoutine = null;
        }
        else
        {
            float transitionTime = GameParamsHolder.Instance.PreviewTransitionTime;

            if (_inPlayerHand)
            {
                transform.DOMove(_handOriginalPosition, transitionTime).SetEase(Ease.OutSine);
                transform.DORotateQuaternion(_handOriginalRotation, transitionTime).SetEase(Ease.OutSine);
                transform.DOScale(Vector3.one, transitionTime).SetEase(Ease.OutSine);

                _previewStopRoutine = StartCoroutine(StopPreviewRoutine(transitionTime));
            }
            else if (_inCardSelection)
            {
                _previewLayoutHandler.Canvas.sortingOrder = _cardSelectionOriginalSortOrder;

                Transform previewCanvasTransform = _previewLayoutHandler.Canvas.transform;

                previewCanvasTransform.DORotateQuaternion(_cardSelectionOriginalRotation, transitionTime).SetEase(Ease.OutSine);
                previewCanvasTransform.DOScale(_cardSelectionOriginalScale, transitionTime).SetEase(Ease.OutSine);
                
                _previewStopRoutine = StartCoroutine(StopPreviewRoutine(transitionTime));
            }
            else
            {
                _previewLayoutHandler.gameObject.SetActive(false);
                _previewLayoutHandler.transform.parent = transform;
                _previewLayoutHandler.transform.rotation = Quaternion.Euler(Vector3.zero);
                _isPreviewing = false;
            }
        }

        #region Local Functions

        IEnumerator StopPreviewRoutine(float transitionTime)
        {
            yield return new WaitForSeconds(transitionTime);
            _isPreviewing = false;
        }

        #endregion
    }

    #region Static Methods

    private static void StopPreview()
    {
        if (_CurrentlyViewing != null)
        {
            _CurrentlyViewing.StopThisPreview();
        }
    }

    private static bool PreviewingSomeCard()
    {
        foreach (HoverPreviewHandler hoverPreview in _AllHoverPreviews)
        {
            if (hoverPreview._isOverCollider && hoverPreview.PreviewEnabled)
                return true;
        }

        return false;
    }

    #endregion

    public void ResetPreviewEnabled()
    {
        PreviewEnabled = _previewWasEnabled;
    }

    public void ResetInPlayerHand()
    {
        InPlayerHand = _wasInPlayerHand;
    }

    public void RegisterOnBeginPlayerHandPreview(Action action)
    {
        _onBeginPlayerHandPreview += action;
    }
    
    public void DeregisterOnBeginPlayerHandPreview(Action action)
    {
        _onBeginPlayerHandPreview -= action;
    }
}
