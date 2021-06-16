using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShieldsManager : MonoBehaviour
{
    [SerializeField] private PlayerDataHandler _playerData;
    
    [Header("Layout")]
    [SerializeField] private Shield _shieldPrefab;
    [SerializeField] private float _shieldAreaWidth = 25;
    [SerializeField] private float _maxShieldWidth = 10;
    [SerializeField] private float _shieldScale = 12.5f;
    [SerializeField] private Transform _holderTransform;

    [Header("Animation")]
    [SerializeField] private string _shieldBreakTriggerName = "BreakShield";
    [SerializeField] private string _shieldUnbreakTriggerName = "UnbreakShield";
    [SerializeField] private float _animationTime = 1f;
    
    [Header("Transition")]
    [SerializeField] private bool _isPlayer = true;
    [SerializeField] private float _pauseTime = 0.5f;
    [SerializeField] private float _fromTransitionTime = 1.5f;
    [SerializeField] private float _toTransitionTime = 1.5f;
    [SerializeField] private Transform _intermediateHolder;
    
    private List<Shield> _shields = new List<Shield>();

    private int _shieldBreakTriggerHash;
    private int _shieldUnbreakTriggerHash;
    
    private void Awake()
    {
        _shieldBreakTriggerHash = Animator.StringToHash(_shieldBreakTriggerName);
        _shieldUnbreakTriggerHash = Animator.StringToHash(_shieldUnbreakTriggerName);
    }

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Shield shield = Instantiate(_shieldPrefab, _holderTransform);
            _shields.Add(shield);
        }

        ArrangeShields();
    }

    #region Functionality Methods

    public IEnumerator SetupShieldsRoutine(CardManager[] cards)
    {
        for (int i = 4; i >= 0; i--)
        {
            MoveToShields(cards[i], _shields[i].CardHolder);
            yield return new WaitForSeconds(_pauseTime);
        }

        yield return new WaitForSeconds(_pauseTime);

        for (int i = 0; i < 4; i++)
        {
            AddShield(i, cards[i]);
            StartCoroutine(PlayMakeShieldAnimationRoutine(i));
        }
        AddShield(4, cards[4]);
        yield return StartCoroutine(PlayMakeShieldAnimationRoutine(4));
    }

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        Shield shield = _shields[shieldIndex];
        shield.SetAnimatorTrigger(_shieldBreakTriggerHash);
        shield.CardHolder.DOScale(shield.HolderScale, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);

        CardManager card = RemoveShieldAtIndex(shieldIndex);
        card.CardLayout.Canvas.sortingOrder = 100;
        if (_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(true);

        yield return StartCoroutine(MoveFromShieldsRoutine(card));
        if (!_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(true);
    }

    public IEnumerator MakeShieldRoutine(CardManager card)
    {
        int emptyIndex = GetEmptyIndex();
        AddShield(emptyIndex, card);
        MoveToShields(card, _shields[emptyIndex].CardHolder);
        yield return new WaitForSeconds(_toTransitionTime);
        yield return StartCoroutine(PlayMakeShieldAnimationRoutine(emptyIndex));
    }

    public CardManager GetShieldAtIndex(int shieldIndex)
    {
        return _playerData.CardsInShields[shieldIndex];
    }

    public CardManager RemoveShieldAtIndex(int shieldIndex)
    {
        int n = _playerData.CardsInShields.Count;
        CardManager card = _playerData.CardsInShields[shieldIndex];
        if (n <= 5)
        {
            _playerData.CardsInShields[shieldIndex] = null;
        }
        else
        {
            _playerData.CardsInShields.RemoveAt(shieldIndex);
        }

        card.transform.parent = transform;
        n = _holderTransform.childCount;
        if (n > 5 && shieldIndex < n)
        {
            _shields.RemoveAt(shieldIndex);
            Transform shieldTransform = _holderTransform.GetChild(shieldIndex);
            shieldTransform.parent = transform;
            Destroy(shieldTransform.gameObject);
            ArrangeShields();
        }

        return card;
    }

    private void AddShield(int shieldIndex, CardManager card)
    {
        if (_playerData.CardsInShields.Count > shieldIndex)
        {
            _playerData.CardsInShields[shieldIndex] = card;
        }
        else
        {
            _playerData.CardsInShields.Add(card);
            if (_playerData.CardsInShields.Count > 5)
            {
                Shield shield = Instantiate(_shieldPrefab, _holderTransform);
                _shields.Add(shield);
                ArrangeShields();
            }
        }
    }

    private int GetEmptyIndex()
    {
        int n = _playerData.CardsInShields.Count;
        for (int i = 0; i < n; i++)
        {
            if (_playerData.CardsInShields[i] == null)
            {
                return i;
            }
        }

        return n;
    }

    #endregion

    #region Transition Methods

    private IEnumerator MoveFromShieldsRoutine(CardManager card)
    {
        card.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        Quaternion rotation = _intermediateHolder.rotation;
        if (!_isPlayer)
            rotation *= Quaternion.Euler(0, 0, 180);
        card.transform.DORotateQuaternion(rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    private void MoveToShields(CardManager card, Transform holderTransform)
    {
        card.transform.transform.DOMove(holderTransform.position, _toTransitionTime).SetEase(Ease.OutQuint);
        Quaternion rotation = holderTransform.rotation;
        if (!_isPlayer)
            rotation = Quaternion.Euler(0, 0, 0);

        card.transform.DORotateQuaternion(rotation, _toTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DOScale(holderTransform.lossyScale, _toTransitionTime).SetEase(Ease.OutQuint);
        card.transform.parent = holderTransform;
    }

    private IEnumerator PlayMakeShieldAnimationRoutine(int shieldIndex)
    {
        _shields[shieldIndex].SetAnimatorTrigger(_shieldUnbreakTriggerHash);
        _shields[shieldIndex].CardHolder.DOScale(Vector3.zero, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);
    }

    #endregion

    #region Layout Methods

    private void ArrangeShields()
    {
        int n = _holderTransform.childCount;
        float shieldWidth = Mathf.Min((_shieldAreaWidth * 2) / n, _maxShieldWidth);
        float sizeRatio = (shieldWidth / _maxShieldWidth) * _shieldScale;

        float startOffset = (n % 2) * shieldWidth;
        if (n % 2 == 0)
            startOffset += shieldWidth / 2;
        Vector3 startPos = new Vector3(_holderTransform.localPosition.x - startOffset,
            _holderTransform.localPosition.y, _holderTransform.localPosition.z);

        for (int i = 0; i < n; i++)
        {
            Transform shieldTransform = _holderTransform.GetChild(i);
            Vector3 shieldPos = new Vector3(startPos.x + (i - n / 2 + 1) * shieldWidth, startPos.y, startPos.z);

            shieldTransform.localPosition = shieldPos;
            shieldTransform.localScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);
        }
    }

    #endregion
}
