﻿using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragHandler : MonoBehaviour
{
    private bool _isDragging = false;
    private bool _canDrag = false;
    private bool _returnToPosition = true;

    private Vector3 _originalPosition;
    private Vector3 _pointerDisplacement;
    private float _zDisplacement;

    private Action<Transform> _onDrag;
    private Action _onDragRelease;

    public bool CanDrag
    {
        set { _canDrag = value; }
    }

    public bool ReturnToPosition
    {
        get { return _returnToPosition;}
        set { _returnToPosition = value; }
    }

    public Vector3 OriginalPosition
    {
        set { _originalPosition = value; }
    }

    #region Static Data Members

    private static DragHandler _CurrentlyDragging;

    public static DragHandler CurrentlyDragging
    {
        get { return _CurrentlyDragging; }
    }

    #endregion

    private void Update()
    {
        if (_isDragging)
        {
            Vector3 mousePos = MouseInWorldCoords();
            transform.position = new Vector3(mousePos.x - _pointerDisplacement.x, mousePos.y - _pointerDisplacement.y, transform.position.z);
            _onDrag.Invoke(transform);
        }
    }

    private void OnMouseDown()
    {
        if (_canDrag)
        {
            _isDragging = true;
            HoverPreviewHandler.PreviewsAllowed = false;

            _CurrentlyDragging = this;
            _originalPosition = transform.position;
            _zDisplacement = -Camera.main.transform.position.z + _originalPosition.z;
            _pointerDisplacement = -_originalPosition + MouseInWorldCoords();
        }
    }

    private void OnMouseUp()
    {
        if (_isDragging)
        {
            _isDragging = false;
            HoverPreviewHandler.PreviewsAllowed = true;
            _CurrentlyDragging = null;

            if (_returnToPosition)
            {
                transform.DOMove(_originalPosition, 0.8f).SetEase(Ease.OutQuint);
            }
            else
            {
                _onDragRelease.Invoke();
            }
        }
    }

    public void RegisterOnDrag(Action<Transform> action)
    {
        _onDrag += action;
    }
    
    public void DeregisterOnDrag(Action<Transform> action)
    {
        _onDrag -= action;
    }
    
    public void RegisterOnDragRelease(Action action)
    {
        _onDragRelease += action;
    }
    
    public void DeregisterOnDragRelease(Action action)
    {
        _onDragRelease -= action;
    }

    private Vector3 MouseInWorldCoords()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = _zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
}
