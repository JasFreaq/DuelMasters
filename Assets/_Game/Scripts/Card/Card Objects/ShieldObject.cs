using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldObject : CardBehaviour
{
    [SerializeField] private MeshRenderer _shieldRenderer;
    [SerializeField] private Transform _cardHolderTransform;

    private Animator _animator;
    private CardObject _cardObject = null;

    private bool _isHighlighted = false;
    private Vector3 _holderScale;

    #region Properties

    public CardObject CardObject
    {
        get { return _cardObject;}
        set { _cardObject = value; }
    }

    public Transform CardHolder
    {
        get { return _cardHolderTransform; }
    }

    public Vector3 HolderScale
    {
        get { return _holderScale; }
    }

    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _holderScale = _cardHolderTransform.localScale;
    }

    public void SetHighlight(bool highlight)
    {
        if (highlight != _isHighlighted)
        {
            Material meshMat = highlight ? GameParamsHolder.Instance.ShieldHighlightMat
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

    public void SetAnimatorTrigger(int hash)
    {
        _animator.SetTrigger(hash);
    }
}
