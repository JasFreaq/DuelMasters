using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragHandler : MonoBehaviour
{
    private const float BUFFER_TIME = 0.1f;
    private const float RETURN_TIME = 0.8f;

    private Transform _cameraTransform;

    private bool _attemptDrag = false;
    private bool _isDragging = false;
    private bool _canDrag = false;
    private bool _returnToPosition = true;

    private Vector3 _originalPosition;
    private Vector3 _originalRotation;
    private Vector3 _pointerDisplacement;
    private float _zDisplacement;
    private float _attemptTimer = 0f;

    private Action<Transform> _onDragBegin;
    private Action _onDragEnd;

    public bool IsDragging
    {
        get { return _isDragging; }
    }

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
        if (_attemptDrag)
        {
            if (_attemptTimer < BUFFER_TIME)
                _attemptTimer += Time.deltaTime;
            else
            {
                _attemptDrag = false;
                _isDragging = true;
                HoverPreviewHandler.PreviewsAllowed = false;
                _CurrentlyDragging = this;

                _zDisplacement = -_cameraTransform.position.z + transform.position.z;
                _pointerDisplacement = -transform.position + MouseInWorldCoords();

                _onDragBegin?.Invoke(transform);
            }
        }
        else if (_isDragging)
        {
            Vector3 mousePos = MouseInWorldCoords();
            transform.position = new Vector3(mousePos.x - _pointerDisplacement.x, mousePos.y - _pointerDisplacement.y, transform.position.z);
        }
    }

    public void BeginDragging()
    {
        if (_canDrag)
        {
            _attemptDrag = true;
            _attemptTimer = 0f;
        }
    }

    public void EndDragging()
    {
        _attemptDrag = false;

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
        transform.DOLocalMove(_originalPosition, RETURN_TIME).SetEase(Ease.OutQuint);
        transform.DOLocalRotate(_originalRotation, RETURN_TIME).SetEase(Ease.OutQuint);
        transform.DOScale(Vector3.one, RETURN_TIME).SetEase(Ease.OutQuint);
        StartCoroutine(InvokeDragEndRoutine());

        IEnumerator InvokeDragEndRoutine()
        {
            yield return new WaitForSeconds(RETURN_TIME);
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
