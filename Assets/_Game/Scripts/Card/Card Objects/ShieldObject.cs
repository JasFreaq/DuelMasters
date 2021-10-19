using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldObject : CardBehaviour
{
    public static int ShieldBreakTriggerHash = 0, ShieldUnbreakTriggerHash = 0;

    [SerializeField] private MeshRenderer _shieldRenderer;
    [SerializeField] private Transform _cardHolderTransform;

    private Animator _animator;
    private BoxCollider _boxCollider;
    private CardObject _cardObj;

    private bool _activeState, _isHighlighted, _keepHighlighted;
    private Vector3 _holderScale;
    
    #region Properties

    public CardObject CardObj
    {
        get { return _cardObj;}
    }

    public Transform CardHolder
    {
        get { return _cardHolderTransform; }
    }

    public bool ActiveState
    {
        get { return _activeState; }
    }

    public Vector3 HolderScale
    {
        get { return _holderScale; }
    }

    public bool KeepHighlighted
    {
        set { _keepHighlighted = value; }
    }

    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider>();

        _holderScale = _cardHolderTransform.localScale;
    }

    public void SetHighlight(bool highlight)
    {
        if(!(_isHighlighted && _keepHighlighted))
        {
            if (highlight != _isHighlighted)
            {
                Material meshMat = highlight
                    ? GameParamsHolder.Instance.ShieldHighlightMat
                    : GameParamsHolder.Instance.ShieldBaseMat;

                Material[] shieldMaterials = _shieldRenderer.materials;
                for (int i = 0, n = shieldMaterials.Length; i < n; i++)
                {
                    shieldMaterials[i] = meshMat;
                }

                _shieldRenderer.materials = shieldMaterials;

                _isHighlighted = highlight;
            }
        }
    }

    public void ActivateShield(bool activate, CardObject cardObj)
    {
        _activeState = activate;
        if (_boxCollider.enabled != _activeState)
            _boxCollider.enabled = _activeState;
        _cardObj = cardObj;

        if (activate && !cardObj)
            Debug.LogError("ActivateShield called in activate state without valid CardObject.");
    }

    public void SetAnimatorTrigger(bool breakShieldAnimation)
    {
        _animator.SetTrigger(breakShieldAnimation ? ShieldBreakTriggerHash : ShieldUnbreakTriggerHash);
    }
}
