using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShieldsManager : MonoBehaviour
{
    [SerializeField] private string _shieldBreakTriggerName = "BreakShield";
    [SerializeField] private string _shieldUnbreakTriggerName = "UnbreakShield";
    [SerializeField] private float _animationTime = 1f;

    private int _shieldBreakTriggerHash;
    private int _shieldUnbreakTriggerHash;

    private ShieldsLayoutHandler _shieldsLayoutHandler;

    private List<Shield> _shields = new List<Shield>();
    private CardManager[] _cards = new CardManager[5];

    public List<Shield> Shields
    {
        get { return _shields; }
    }

    private void Awake()
    {
        _shieldBreakTriggerHash = Animator.StringToHash(_shieldBreakTriggerName);
        _shieldUnbreakTriggerHash = Animator.StringToHash(_shieldUnbreakTriggerName);

        _shieldsLayoutHandler = GetComponent<ShieldsLayoutHandler>();
    }

    private void Start()
    {
        _shieldsLayoutHandler.Initialize(this);
    }

    public Transform GetCardHolderTransform(int shieldIndex)
    {
        return _shields[shieldIndex].CardHolder;
    }

    public IEnumerator AddShieldRoutine(int shieldIndex, CardManager card)
    {
        _shields[shieldIndex].SetAnimatorTrigger(_shieldUnbreakTriggerHash);
        _shields[shieldIndex].CardHolder.DOScale(Vector3.zero, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);

        _cards[shieldIndex] = card;
    }

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        _shields[shieldIndex].SetAnimatorTrigger(_shieldBreakTriggerHash);
        _shields[shieldIndex].CardHolder.DOScale(_shields[shieldIndex].HolderScale, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);
    }
    
    public CardManager RemoveCardAtIndex(int shieldIndex)
    {
        CardManager card = _cards[shieldIndex];
        _cards[shieldIndex] = null;
        return card;
    }
}
