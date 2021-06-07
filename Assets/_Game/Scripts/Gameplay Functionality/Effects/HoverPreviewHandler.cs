using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HoverPreviewHandler: MonoBehaviour
{
    private const float TRANSITION_TIME = 0.75f;

    [SerializeField] private GameObject _previewGameObject;
    [SerializeField] private float _hoverTimeBeforePreview = 0.5f;
    [SerializeField] private List<GameObject> _objectsToHide = new List<GameObject>();

    private Vector3 _targetPosition = Vector3.zero;
    private Vector3 _targetRotation = Vector3.zero;
    private Vector3 _targetScale = Vector3.one;
    private bool _previewEnabled = false;
    private bool _inPlayerHand = false;
    private Coroutine _previewRoutine = null;

    public bool OverCollider { get; set; }

    public Vector3 TargetPosition
    {
        set { _targetPosition = value; }
    }
    
    public Vector3 TargetRotation
    {
        set { _targetRotation = value; }
    }

    public Vector3 TargetScale
    {
        set { _targetScale = value; }
    }

    public bool InPlayerHand
    {
        set { _inPlayerHand = value; }
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

    private static bool _PreviewsAllowed = true;
    public static bool PreviewsAllowed
    {
        get { return _PreviewsAllowed;}

        set 
        { 
            _PreviewsAllowed = value;
            if (!_PreviewsAllowed)
                StopAllPreviews();
        }
    }

    #endregion
    
    void Start()
    {
        if (_AllHoverPreviews == null)
        {
            _AllHoverPreviews = GameObject.FindObjectsOfType<HoverPreviewHandler>();
        }
    }
            
    void OnMouseEnter()
    {
        OverCollider = true;
        if (PreviewsAllowed && PreviewEnabled)
        {
            _previewRoutine = StartCoroutine(StartPreviewRoutine());
        }
    }
        
    void OnMouseExit()
    {
        OverCollider = false;

        if (_previewRoutine != null)
        {
            StopCoroutine(_previewRoutine);
            _previewRoutine = null;
        }

        if (!PreviewingSomeCard())
        {
            StopAllPreviews();
        }
    }

    IEnumerator StartPreviewRoutine()
    {
        // save this HoverPreview as current
        _CurrentlyViewing = this;

        yield return new WaitForSeconds(_hoverTimeBeforePreview);
        PreviewThisObject();
        _previewRoutine = null;
    }

    void PreviewThisObject()
    {
        // first disable the previous preview if there is one already
        StopAllPreviews();

        // enable Preview game object
        _previewGameObject.SetActive(true);

        // disable
        foreach (GameObject @object in _objectsToHide)
        {
            @object.SetActive(false);
        }

        //// tween to target position
        _previewGameObject.transform.localPosition = Vector3.zero;
        _previewGameObject.transform.localEulerAngles = Vector3.zero;
        _previewGameObject.transform.localScale = Vector3.one;

        if (_inPlayerHand)
        {
            _previewGameObject.transform.DOMove(_targetPosition, TRANSITION_TIME).SetEase(Ease.OutQuint);
            _previewGameObject.transform.DORotate(_targetRotation, TRANSITION_TIME).SetEase(Ease.OutQuint);
        }
        else
        {
            _previewGameObject.transform.DOLocalMove(_targetPosition, TRANSITION_TIME).SetEase(Ease.OutQuint);
            _previewGameObject.transform.DOLocalRotate(_targetRotation, TRANSITION_TIME).SetEase(Ease.OutQuint);
        }
        _previewGameObject.transform.DOScale(_targetScale, TRANSITION_TIME).SetEase(Ease.OutQuint);
    }

    void StopThisPreview()
    {
        _previewGameObject.SetActive(false);
        _previewGameObject.transform.localPosition = Vector3.zero;
        _previewGameObject.transform.localEulerAngles = Vector3.zero;
        _previewGameObject.transform.localScale = Vector3.one;
        foreach (GameObject @object in _objectsToHide)
        {
            @object.SetActive(true);
        }
    }

    #region Static Methods

    private static void StopAllPreviews()
    {
        if (_CurrentlyViewing != null)
        {
            if (_CurrentlyViewing._previewRoutine != null)
                _CurrentlyViewing.StopCoroutine(_CurrentlyViewing._previewRoutine);
            _CurrentlyViewing._previewGameObject.SetActive(false);
            _CurrentlyViewing._previewGameObject.transform.localPosition = Vector3.zero;
            _CurrentlyViewing._previewGameObject.transform.localEulerAngles = Vector3.zero;
            _CurrentlyViewing._previewGameObject.transform.localScale = Vector3.one;
            foreach (GameObject @object in _CurrentlyViewing._objectsToHide)
            {
                @object.SetActive(true);
            }
        }
    }

    private static bool PreviewingSomeCard()
    {
        if (!PreviewsAllowed)
            return false;
        
        foreach (HoverPreviewHandler hoverPreview in _AllHoverPreviews)
        {
            if (hoverPreview.OverCollider && hoverPreview.PreviewEnabled)
                return true;
        }

        return false;
    }

    #endregion
}
