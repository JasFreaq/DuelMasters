using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParamsHolder : MonoBehaviour
{
    [Header("Floats")]
    [SerializeField] private float _layoutsArrangeMoveTime = 0.5f;
    [SerializeField] private float _hoverBeforePreviewTime = 0.5f;
    [SerializeField] private float _previewTransitionTime = 0.25f;
    [SerializeField] private float _dragReturnTime = 0.8f;
    [SerializeField] private float _tapAngle = 15f;
    [SerializeField] private float _tapTransitionTime = 0.5f;

    [Header("Transforms")] 
    [SerializeField] private Transform _hoverIntermediateTransform = null;

    [Header("Colors")]
    [SerializeField] private Color _highlightGlowColor = new Color(0f, 1f, 1f, 1f);
    [SerializeField] private Color _playGlowColor = new Color(1f, 0.4117647f, 0f, 1f);

    #region Static Variables

    private static GameParamsHolder _Instance = null;

    public static GameParamsHolder Instance
    {
        get
        {
            if (!_Instance)
                _Instance = FindObjectOfType<GameParamsHolder>();
            return _Instance;
        }
    }

    #endregion

    #region Float Properties

    public float LayoutsArrangeMoveTime
    {
        get { return _layoutsArrangeMoveTime; }
    }

    public float HoverBeforePreviewTime
    {
        get { return _hoverBeforePreviewTime; }
    }
    
    public float PreviewTransitionTime
    {
        get { return _previewTransitionTime; }
    }

    public float DragReturnTime
    {
        get { return _dragReturnTime; }
    }

    public float TapAngle
    {
        get { return _tapAngle; }
    }

    public float TapTransitionTime
    {
        get { return _tapTransitionTime; }
    }

    #endregion

    #region Transform Properties

    public Transform HoverIntermediateTransform
    {
        get { return _hoverIntermediateTransform; }
    }

    #endregion

    #region Color Properties

    public Color HighlightGlowColor
    {
        get { return _highlightGlowColor; }
    }
    
    public Color PlayGlowColor
    {
        get { return _playGlowColor; }
    }

    #endregion

    private void Awake()
    {
        int count = FindObjectsOfType<GameParamsHolder>().Length;
        if (count > 1)
            Destroy(gameObject);
        else
            _Instance = this;
    }
}
