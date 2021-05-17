using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private Transform _cardHolderTransform;

    private Animator _animator;

    private Vector3 _holderScale;

    public Transform CardHolder
    {
        get { return _cardHolderTransform; }
    }

    public Vector3 HolderScale
    {
        get { return _holderScale; }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetAnimatorTrigger(int hash)
    {
        _animator.SetTrigger(hash);
    }

    public void UpdateHolderScale()
    {
        _holderScale = _cardHolderTransform.localScale;
    }
}
