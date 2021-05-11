using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HoverPreview: MonoBehaviour
{
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _targetScale;
    [SerializeField] private GameObject _previewGameObject;
    [SerializeField] private bool _activateOnStart = false;
    [SerializeField] private List<GameObject> _objectsToHide = new List<GameObject>();
    
    public bool OverCollider { get; set; }

    private bool _previewEnabled = false;
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

    private static HoverPreview _CurrentlyViewing = null;
    private static HoverPreview[] _AllHoverPreviews = null;

    private static bool _PreviewsAllowed = true;
    public static bool PreviewsAllowed
    {
        get { return _PreviewsAllowed;}

        set 
        { 
            //Debug.Log("Hover Previews Allowed is now: " + value);
            _PreviewsAllowed = value;
            if (!_PreviewsAllowed)
                StopAllPreviews();
        }
    }

    #endregion
    
    void Start()
    {
        PreviewEnabled = _activateOnStart;
        if (_AllHoverPreviews == null)
        {
            _AllHoverPreviews = GameObject.FindObjectsOfType<HoverPreview>();
        }
    }
            
    void OnMouseEnter()
    {
        OverCollider = true;
        if (PreviewsAllowed && PreviewEnabled)
            PreviewThisObject();
    }
        
    void OnMouseExit()
    {
        OverCollider = false;

        if (!PreviewingSomeCard())
            StopAllPreviews();
    }

    void PreviewThisObject()
    {
        // first disable the previous preview if there is one already
        StopAllPreviews();

        // save this HoverPreview as curent
        _CurrentlyViewing = this;

        // enable Preview game object
        _previewGameObject.SetActive(true);

        // disable
        foreach (GameObject @object in _objectsToHide)
        {
            @object.SetActive(false);
        }

        // tween to target position
        _previewGameObject.transform.localPosition = Vector3.zero;
        _previewGameObject.transform.localScale = Vector3.one;

        _previewGameObject.transform.DOLocalMove(_targetPosition, 1f).SetEase(Ease.OutQuint);
        _previewGameObject.transform.DOScale(_targetScale, 1f).SetEase(Ease.OutQuint);
    }

    void StopThisPreview()
    {
        _previewGameObject.SetActive(false);
        _previewGameObject.transform.localScale = Vector3.one;
        _previewGameObject.transform.localPosition = Vector3.zero;
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
            _CurrentlyViewing._previewGameObject.SetActive(false);
            _CurrentlyViewing._previewGameObject.transform.localScale = Vector3.one;
            _CurrentlyViewing._previewGameObject.transform.localPosition = Vector3.zero;
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
        
        foreach (HoverPreview hoverPreview in _AllHoverPreviews)
        {
            if (hoverPreview.OverCollider && hoverPreview.PreviewEnabled)
                return true;
        }

        return false;
    }

    #endregion
}
