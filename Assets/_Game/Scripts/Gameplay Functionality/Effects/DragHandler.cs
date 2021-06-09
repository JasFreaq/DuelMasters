using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragHandler : MonoBehaviour
{
    private Transform _cameraTransform;

    private bool _isDragging = false;
    private bool _canDrag = false;
    private bool _returnToPosition = true;

    private Vector3 _originalPosition;
    private Vector3 _originalRotation;
    private Vector3 _pointerDisplacement;
    private float _zDisplacement;

    private Action<Transform> _onDragBegin;
    private Action _onDragEnd;
    
    public bool CanDrag
    {
        set { _canDrag = value; }
    }

    public bool IsReturningToPosition
    {
        get { return _returnToPosition;}
        set { _returnToPosition = value; }
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

    private void Update()
    {
        if (_isDragging)
        {
            Vector3 mousePos = MouseInWorldCoords();
            transform.position = new Vector3(mousePos.x - _pointerDisplacement.x, mousePos.y - _pointerDisplacement.y, transform.position.z);
        }
    }

    public void BeginDragging()
    {
        if (_canDrag)
        {
            _isDragging = true;
            HoverPreviewHandler.PreviewsAllowed = false;
            _CurrentlyDragging = this;

            _zDisplacement = -_cameraTransform.position.z + transform.position.z;
            _pointerDisplacement = -transform.position + MouseInWorldCoords();

            _onDragBegin?.Invoke(transform);
        }
    }

    public void EndDragging()
    {
        if (_isDragging)
        {
            _isDragging = false;
            HoverPreviewHandler.PreviewsAllowed = true;
            _CurrentlyDragging = null;

            if (_returnToPosition)
                ReturnToPosition();
            else
                _onDragEnd?.Invoke();
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
        StartCoroutine(InvokeDragEndRoutine());

        IEnumerator InvokeDragEndRoutine()
        {
            yield return new WaitForSeconds(returnTime);
            _onDragEnd.Invoke();
        }
    }

    #region Register Callbacks

    public void RegisterOnDragBegin(Action<Transform> action)
    {
        _onDragBegin += action;
    }
    
    public void DeregisterOnDragBegin(Action<Transform> action)
    {
        _onDragBegin -= action;
    }
    
    public void RegisterOnDragEnd(Action action)
    {
        _onDragEnd += action;
    }
    
    public void DeregisterOnDragEnd(Action action)
    {
        _onDragEnd -= action;
    }

    #endregion

    private Vector3 MouseInWorldCoords()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = _zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
}
