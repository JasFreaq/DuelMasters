using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HoverPreviewHandler: MonoBehaviour
{
    [SerializeField] private GameObject _previewGameObject;
    [SerializeField] private float _hoverTimeBeforePreview = 0.5f;
    [SerializeField] private List<GameObject> _objectsToHide = new List<GameObject>();
    
    private Vector3 _targetPosition = Vector3.zero;
    private Vector3 _targetScale = Vector3.one;
    private Vector3 _targetRotation = Vector3.zero;

    private Vector3 _handPosition = Vector3.zero;
    private Vector3 _handRotation = Vector3.zero;

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

    public void SetPreviewParameters(Vector3 targetPosition, Vector3 targetScale, Vector3 targetRotation = new Vector3())
    {
        _targetPosition = targetPosition;
        _targetScale = targetScale;
        _targetRotation = targetRotation;
    }

    private IEnumerator StartPreviewRoutine()
    {
        if (!_inPlayerHand)
            yield return new WaitForSeconds(_hoverTimeBeforePreview);

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
                _handPosition = transform.position;
                _handRotation = transform.eulerAngles;
                _onBeginPlayerHandPreview.Invoke();
                _isPreviewing = true;
            }

            if (_handPreviewStopRoutine != null)
                StopCoroutine(_handPreviewStopRoutine);

            Vector3 previewPosition = _targetPosition;
            previewPosition.x = transform.position.x;

            transform.DOMove(previewPosition, transitionTime).SetEase(Ease.OutQuint);
            transform.DOScale(_targetScale, transitionTime).SetEase(Ease.OutQuint);
            transform.DORotate(_targetRotation, transitionTime).SetEase(Ease.OutQuint);
        }
        else
        {
            if (!_isPreviewing)
                _isPreviewing = true;

            // enable Preview game object
            _previewGameObject.SetActive(true);

            // disable
            foreach (GameObject @object in _objectsToHide)
            {
                @object.SetActive(false);
            }

            _previewGameObject.transform.DOLocalMove(_targetPosition, transitionTime).SetEase(Ease.OutQuint);
            _previewGameObject.transform.DOScale(_targetScale, transitionTime).SetEase(Ease.OutQuint);
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

                transform.DOMove(_handPosition, transitionTime).SetEase(Ease.OutQuint);
                transform.DORotate(_handRotation, transitionTime).SetEase(Ease.OutQuint);
                transform.DOScale(Vector3.one, transitionTime).SetEase(Ease.OutQuint);

                _handPreviewStopRoutine = StartCoroutine(StopPreviewRoutine());

                IEnumerator StopPreviewRoutine()
                {
                    yield return new WaitForSeconds(transitionTime);
                    _isPreviewing = false;
                }
            }
            else
            {
                _previewGameObject.SetActive(false);
                _previewGameObject.transform.localPosition = Vector3.zero;
                _previewGameObject.transform.localScale = Vector3.one;
                foreach (GameObject @object in _objectsToHide)
                {
                    @object.SetActive(true);
                }

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
