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
    [SerializeField] private float _previewSideBoundFraction = 0.651f;
    [SerializeField] private float _dragReturnTime = 0.8f;
    [SerializeField] private float _tapAngle = 15f;
    [SerializeField] private float _tapTransitionTime = 0.5f;
    [SerializeField] private float _attackTime = 0.5f;

    [Header("Transforms")] 
    [SerializeField] private Transform _hoverIntermediateTransform = null;

    [Header("Colors")]
    [SerializeField] private Color _baseHighlightColor = new Color(0f, 1f, 1f, 1f);
    [SerializeField] private Color _playHighlightColor = new Color(1f, 0.4117647f, 0f, 1f);

    [Header("Materials")] 
    [SerializeField] private Material _shieldBaseMat;
    [SerializeField] private Material _shieldHighlightMat;

    #region Static Data Members

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

    #region Properties

    #region Float(s)

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

    public float PreviewSideBoundFraction
    {
        get { return _previewSideBoundFraction; }
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

    public float AttackTime
    {
        get { return _attackTime; }
    }

    #endregion

    #region Transform(s)

    public Transform HoverIntermediateTransform
    {
        get { return _hoverIntermediateTransform; }
    }

    #endregion

    #region Color(s)

    public Color BaseHighlightColor
    {
        get { return _baseHighlightColor; }
    }
    
    public Color PlayHighlightColor
    {
        get { return _playHighlightColor; }
    }

    #endregion

    #region Material(s)
    
    public Material ShieldBaseMat
    {
        get { return _shieldBaseMat; }
    }

    public Material ShieldHighlightMat
    {
        get { return _shieldHighlightMat; }
    }

    #endregion

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
