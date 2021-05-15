using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShieldsManager : MonoBehaviour
{
    [SerializeField] private Transform[] _cardHolders = new Transform[5];
    [SerializeField] private string _shieldBreakTriggerName = "BreakShield";
    [SerializeField] private string _shieldUnbreakTriggerName = "UnbreakShield";
    [SerializeField] private float _animationTime = 1f;

    private int _shieldBreakTriggerHash;
    private int _shieldUnbreakTriggerHash;

    private Animator[] _shieldAnimators;
    private CardManager[] _shields = new CardManager[5];
    private Vector3 _holderScale;
    
    private void Awake()
    {
        _shieldAnimators = GetComponentsInChildren<Animator>();

        _shieldBreakTriggerHash = Animator.StringToHash(_shieldBreakTriggerName);
        _shieldUnbreakTriggerHash = Animator.StringToHash(_shieldUnbreakTriggerName);

        _holderScale = _cardHolders[0].localScale;
    }

    public Transform GetCardHolderTransform(int shieldIndex)
    {
        return _cardHolders[shieldIndex];
    }

    public IEnumerator AddShieldRoutine(int shieldIndex, CardManager card)
    {
        _shieldAnimators[shieldIndex].SetTrigger(_shieldUnbreakTriggerHash);
        _cardHolders[shieldIndex].DOScale(Vector3.zero, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);

        _shields[shieldIndex] = card;
    }

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        _shieldAnimators[shieldIndex].SetTrigger(_shieldBreakTriggerHash);
        _cardHolders[shieldIndex].DOScale(_holderScale, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);
    }

    public CardManager RemoveCardAtIndex(int shieldIndex)
    {
        CardManager card = _shields[shieldIndex];
        _shields[shieldIndex] = null;
        return card;
    }
}
