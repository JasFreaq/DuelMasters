using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HoverPreviewHandler: MonoBehaviour
{
    [SerializeField] private Transform _previewTransform;
    
    private Vector3 _handPreviewPosition = Vector3.zero;
    private Vector3 _handPreviewScale = Vector3.one;
    private Quaternion _handPreviewRotation = Vector3.zero;

    private Vector3 _handOriginalPosition = Vector3.zero;
    private Quaternion _handOriginalRotation = Quaternion.identity;

    private bool _previewEnabled = false;
    private bool _inPlayerHand = false;
    private bool _isPreviewing = false;
    private bool _shouldStopPreview = true;

    private Action _onBeginPlayerHandPreview;

    private bool _isOverCollider = false;
    private Coroutine _previewRoutine = null;
    private Coroutine _handPreviewStopRoutine = null;

    public bool InPlayerHand
    {
        set { _inPlayerHand = value; }
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

    private IEnumerator StartPreviewRoutine()
    {
        if (!_inPlayerHand)
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

            if (_handPreviewStopRoutine != null)
                StopCoroutine(_handPreviewStopRoutine);

            Vector3 previewPosition = _handPreviewPosition;
            previewPosition.x = transform.position.x;

            transform.DOMove(previewPosition, transitionTime).SetEase(Ease.OutSine);
            transform.DOScale(_handPreviewScale, transitionTime).SetEase(Ease.OutSine);
            transform.DORotateQuaternion(_handPreviewRotation, transitionTime).SetEase(Ease.OutSine);
        }
        else
        {
            if (!_isPreviewing)
                _isPreviewing = true;
            
            Transform hoverIntermediate = GameParamsHolder.Instance.HoverIntermediateTransform;
            _previewTransform.parent = hoverIntermediate;
            
            _previewTransform.localPosition = Vector3.zero;
            _previewTransform.localRotation = Quaternion.Euler(Vector3.zero);
            _previewTransform.localScale = Vector3.one;

            _previewTransform.gameObject.SetActive(true);
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
            if (_inPlayerHand)
            {
                float transitionTime = GameParamsHolder.Instance.PreviewTransitionTime;

                transform.DOMove(_handOriginalPosition, transitionTime).SetEase(Ease.OutSine);
                transform.DORotateQuaternion(_handOriginalRotation, transitionTime).SetEase(Ease.OutSine);
                transform.DOScale(Vector3.one, transitionTime).SetEase(Ease.OutSine);

                _handPreviewStopRoutine = StartCoroutine(StopPreviewRoutine());

                IEnumerator StopPreviewRoutine()
                {
                    yield return new WaitForSeconds(transitionTime);
                    _isPreviewing = false;
                }
            }
            else
            {
                _previewTransform.gameObject.SetActive(false);
                _previewTransform.parent = transform;
                _previewTransform.localEulerAngles = Vector3.zero;

                _isPreviewing = false;
            }
        }
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

    public void RegisterOnBeginPlayerHandPreview(Action action)
    {
        _onBeginPlayerHandPreview += action;
    }
    
    public void DeregisterOnBeginPlayerHandPreview(Action action)
    {
        _onBeginPlayerHandPreview -= action;
    }
}
