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

    private bool _isPlayer = true;
    private float _pauseTime;
    private float _fromTransitionTime;
    private float _toTransitionTime;
    private Transform _intermediateHolder;

    private List<CardManager> _cards = new List<CardManager>();
    
    private ShieldsLayoutHandler _shieldsLayoutHandler;
    
    private void Awake()
    {
        _shieldBreakTriggerHash = Animator.StringToHash(_shieldBreakTriggerName);
        _shieldUnbreakTriggerHash = Animator.StringToHash(_shieldUnbreakTriggerName);

        _shieldsLayoutHandler = GetComponent<ShieldsLayoutHandler>();
    }

    private void Start()
    {
        _shieldsLayoutHandler.Initialize();
    }

    public void Initialize(bool isPlayer, float pauseTime, float fromTransitionTime, float toTransitionTime, Transform intermediateTransform)
    {
        _isPlayer = isPlayer;
        _pauseTime = pauseTime;
        _fromTransitionTime = fromTransitionTime;
        _toTransitionTime = toTransitionTime;
        _intermediateHolder = intermediateTransform;
    }

    public IEnumerator SetupShieldsRoutine(CardManager[] cards)
    {
        for (int i = 0; i < 5; i++)
        {
            MoveToShields(cards[i].transform, _shieldsLayoutHandler.Shields[i].CardHolder);
            yield return new WaitForSeconds(_pauseTime);
        }

        yield return new WaitForSeconds(_toTransitionTime);

        for (int i = 0; i < 4; i++)
        {
            AddShield(i, cards[i]);
            StartCoroutine(PlayMakeShieldAnimationRoutine(i));
        }
        AddShield(4, cards[4]);
        yield return StartCoroutine(PlayMakeShieldAnimationRoutine(4));
    }

    private IEnumerator MoveFromShieldsRoutine(Transform cardTransform)
    {
        cardTransform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        Vector3 rotation = _intermediateHolder.rotation.eulerAngles;
        if (!_isPlayer)
            rotation += new Vector3(0, 0, 180);
        cardTransform.DORotate(rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    private void MoveToShields(Transform cardTransform, Transform holderTransform, bool fromDeck = true)
    {
        cardTransform.transform.DOMove(holderTransform.position, _toTransitionTime).SetEase(Ease.OutQuint);
        Vector3 rotation = holderTransform.eulerAngles;
        if (!_isPlayer)
        {
            if (fromDeck)
                rotation += new Vector3(30, 180, 0);
            else
                rotation = new Vector3(0, 0, 0);
        }

        cardTransform.DORotate(rotation, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(holderTransform.lossyScale, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.parent = holderTransform;
    }

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        Shield shield = _shieldsLayoutHandler.Shields[shieldIndex];
        shield.SetAnimatorTrigger(_shieldBreakTriggerHash);
        shield.CardHolder.DOScale(shield.HolderScale, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);

        CardManager card = RemoveCardAtIndex(shieldIndex);
        card.CardLayout.Canvas.sortingOrder = 100;
        if (_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(true);

        yield return StartCoroutine(MoveFromShieldsRoutine(card.transform));
        if (!_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(true);
    }

    public IEnumerator MakeShieldRoutine(CardManager card)
    {
        int emptyIndex = GetEmptyIndex();
        AddShield(emptyIndex, card);
        MoveToShields(card.transform, _shieldsLayoutHandler.Shields[emptyIndex].CardHolder, false);
        yield return new WaitForSeconds(_toTransitionTime);
        yield return StartCoroutine(PlayMakeShieldAnimationRoutine(emptyIndex));
    }

    public CardManager GetCardAtIndex(int shieldIndex)
    {
        return _cards[shieldIndex];
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

        card.transform.parent = transform;
        _shieldsLayoutHandler.RemoveShield(shieldIndex);
        return card;
    }

    private void AddShield(int shieldIndex, CardManager card)
    {
        if (_cards.Count > shieldIndex)
        {
            _cards[shieldIndex] = card;
        }
        else
        {
            _cards.Add(card);
            if (_cards.Count > 5)
                _shieldsLayoutHandler.AddShield();
        }
    }

    private IEnumerator PlayMakeShieldAnimationRoutine(int shieldIndex)
    {
        _shieldsLayoutHandler.Shields[shieldIndex].SetAnimatorTrigger(_shieldUnbreakTriggerHash);
        _shieldsLayoutHandler.Shields[shieldIndex].CardHolder.DOScale(Vector3.zero, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);
    }
    
    private int GetEmptyIndex()
    {
        int n = _cards.Count;
        for (int i = 0; i < n; i++)
        {
            if (_cards[i] == null)
            {
                return i;
            }
        }

        return n;
    }
}
