using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private MeshRenderer _shieldRenderer;
    [SerializeField] private Transform _cardHolderTransform;

    private Animator _animator;

    private bool _isHighlighted = false;
    private Vector3 _holderScale;

    #region Properties

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

            for (int i = 0, n = _shieldRenderer.materials.Length; i < n; i++)
            {
                _shieldRenderer.materials[i] = meshMat;
            }

            _isHighlighted = highlight;
        }
    }

    public void SetAnimatorTrigger(int hash)
    {
        _animator.SetTrigger(hash);
    }
}
