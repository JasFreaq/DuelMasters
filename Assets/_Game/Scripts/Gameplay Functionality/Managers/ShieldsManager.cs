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
    private List<CardManager> _cards = new List<CardManager>();

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

        if (_cards.Count > shieldIndex)
            _cards[shieldIndex] = card;
        else
        {
            _cards.Add(card);
            if (_cards.Count > 5)
                _shieldsLayoutHandler.AddShield();
        }
    }

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        _shields[shieldIndex].SetAnimatorTrigger(_shieldBreakTriggerHash);
        _shields[shieldIndex].CardHolder.DOScale(_shields[shieldIndex].HolderScale, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);

        _shieldsLayoutHandler.RemoveShield(shieldIndex);
    }

    public IEnumerator MakeShieldRoutine(CardManager card)
    {
        int n = _cards.Count;
        for (int i = 0; i < n; i++)
        {
            if (_cards[i] == null)
            {
                yield return StartCoroutine(AddShieldRoutine(i, card));
                yield break;
            }
        }

        yield return StartCoroutine(AddShieldRoutine(n, card));
    }

    public CardManager RemoveCardAtIndex(int shieldIndex)
    {
        int n = _cards.Count;
        CardManager card = _cards[shieldIndex];
        if (n <= 5) 
        {
            _cards[shieldIndex] = null;
        }
        else
        {
            _cards.RemoveAt(shieldIndex);
        }
        return card;
    }
}
