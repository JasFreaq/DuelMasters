using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragHandler : MonoBehaviour
{
    private Transform _cameraTransform;

    private bool _isDragging = false;
    private bool _canDrag = false;

    private Vector3 _originalPosition;
    private Vector3 _originalRotation;
    private Vector3 _pointerDisplacement;
    private float _zDisplacement;

    private Action<Transform> _onDrag;
    
    public bool CanDrag
    {
        set { _canDrag = value; }
    }
    
    #region Static Data Members

    private static DragHandler _CurrentlyDragging;

    public static DragHandler CurrentlyDragging
    {
        get { return _CurrentlyDragging; }
    }

    #endregion

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (_isDragging)
        {
            Vector3 mousePos = MouseInWorldCoords();
            transform.position = new Vector3(mousePos.x - _pointerDisplacement.x, mousePos.y - _pointerDisplacement.y, transform.position.z);
            _onDrag?.Invoke(transform);
        }
    }

    public void BeginDragging()
    {
        if (_canDrag)
        {
            _isDragging = true;
            _CurrentlyDragging = this;
            
            _zDisplacement = -_cameraTransform.position.z + transform.position.z;
            _pointerDisplacement = -transform.position + MouseInWorldCoords();

            _onDrag?.Invoke(transform);
        }
    }

    public void ResetDragging()
    {
        _isDragging = false;
        _CurrentlyDragging = null;
    }

    public void EndDragging()
    {
        if (_isDragging)
        {
            ResetDragging();
            ReturnToPosition();
        }
    }
    
    public void SetOriginalOrientation(Vector3 position, Vector3 rotation)
    {
        _originalPosition = position;
        _originalRotation = rotation;
    }

    public void ReturnToPosition()
    {
        float returnTime = GameParamsHolder.Instance.DragReturnTime;

        transform.DOLocalMove(_originalPosition, returnTime).SetEase(Ease.OutQuint);
        transform.DOLocalRotate(_originalRotation, returnTime).SetEase(Ease.OutQuint);
        transform.DOScale(Vector3.one, returnTime).SetEase(Ease.OutQuint);
    }

    #region Register Callbacks

    public void RegisterOnDrag(Action<Transform> action)
    {
        _onDrag += action;
    }
    
    public void DeregisterOnDrag(Action<Transform> action)
    {
        _onDrag -= action;
    }
    
    #endregion

    private Vector3 MouseInWorldCoords()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = _zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
}
