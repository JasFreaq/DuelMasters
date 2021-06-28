using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShieldsManager : MonoBehaviour
{
    [SerializeField] private bool _isPlayer = true;

    [Header("Layout")]
    [SerializeField] private ShieldObject _shieldPrefab;
    [SerializeField] private float _shieldAreaWidth = 25;
    [SerializeField] private float _maxShieldWidth = 10;
    [SerializeField] private float _shieldScale = 12.5f;
    [SerializeField] private Transform _holderTransform;

    [Header("Animation")]
    [SerializeField] private string _shieldBreakTriggerName = "BreakShield";
    [SerializeField] private string _shieldUnbreakTriggerName = "UnbreakShield";
    [SerializeField] private float _animationTime = 1f;
    
    [Header("Transition")]
    [SerializeField] private float _pauseTime = 0.5f;
    [SerializeField] private float _fromTransitionTime = 1.5f;
    [SerializeField] private float _toTransitionTime = 1.5f;
    [SerializeField] private Transform _intermediateHolder;
    
    private PlayerDataHandler _playerData;
    
    private int _shieldBreakTriggerHash;
    private int _shieldUnbreakTriggerHash;
    
    private void Start()
    {
        _playerData = _isPlayer ? GameManager.PlayerDataHandler : GameManager.OpponentDataHandler;

        _shieldBreakTriggerHash = Animator.StringToHash(_shieldBreakTriggerName);
        _shieldUnbreakTriggerHash = Animator.StringToHash(_shieldUnbreakTriggerName);

        for (int i = 0; i < 5; i++)
        {
            ShieldObject shield = Instantiate(_shieldPrefab, _holderTransform);
            _playerData.Shields.Add(shield);
        }

        ArrangeShields();
    }

    #region Functionality Methods

    public IEnumerator SetupShieldsRoutine(CardInstanceObject[] cards)
    {
        for (int i = 4; i >= 0; i--)
        {
            MoveToShields(cards[i], _playerData.Shields[i]);
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
        ShieldObject shieldObj = _playerData.Shields[shieldIndex];
        shieldObj.SetAnimatorTrigger(_shieldBreakTriggerHash);
        shieldObj.CardHolder.DOScale(shieldObj.HolderScale, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);
        shieldObj.CardObject = null;

        CardInstanceObject card = RemoveShieldAtIndex(shieldIndex);
        card.CardLayout.Canvas.sortingOrder = 100;
        if (_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(true);

        yield return StartCoroutine(MoveFromShieldsRoutine(card));
        if (!_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(true);
    }

    public IEnumerator MakeShieldRoutine(CardInstanceObject card)
    {
        int emptyIndex = GetEmptyIndex();
        AddShield(emptyIndex, card);
        MoveToShields(card, _playerData.Shields[emptyIndex]);
        yield return new WaitForSeconds(_toTransitionTime);
        yield return StartCoroutine(PlayMakeShieldAnimationRoutine(emptyIndex));
    }

    public CardInstanceObject GetShieldAtIndex(int shieldIndex)
    {
        return _playerData.CardsInShields[shieldIndex];
    }

    public CardInstanceObject RemoveShieldAtIndex(int shieldIndex)
    {
        int n = _playerData.CardsInShields.Count;
        CardInstanceObject card = _playerData.CardsInShields[shieldIndex];
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
            _playerData.Shields.RemoveAt(shieldIndex);
            Transform shieldTransform = _holderTransform.GetChild(shieldIndex);
            shieldTransform.parent = transform;
            Destroy(shieldTransform.gameObject);
            ArrangeShields();
        }

        return card;
    }

    private void AddShield(int shieldIndex, CardInstanceObject card)
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
                ShieldObject shield = Instantiate(_shieldPrefab, _holderTransform);
                _playerData.Shields.Add(shield);
                ArrangeShields();
            }
        }

        card.CurrentZone = CardZone.Shields;
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

    private IEnumerator MoveFromShieldsRoutine(CardInstanceObject cardObj)
    {
        cardObj.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        Quaternion rotation = _intermediateHolder.rotation;
        if (!_isPlayer)
            rotation *= Quaternion.Euler(0, 0, 180);
        cardObj.transform.DORotateQuaternion(rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardObj.transform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    private void MoveToShields(CardInstanceObject cardObj, ShieldObject shieldObj)
    {
        Transform holderTransform = shieldObj.CardHolder;
        cardObj.transform.transform.DOMove(holderTransform.position, _toTransitionTime).SetEase(Ease.OutQuint);
        
        Quaternion rotation = holderTransform.rotation;
        if (!_isPlayer)
            rotation = Quaternion.Euler(0, 0, 0);
        cardObj.transform.DORotateQuaternion(rotation, _toTransitionTime).SetEase(Ease.OutQuint);
        
        cardObj.transform.DOScale(holderTransform.lossyScale, _toTransitionTime).SetEase(Ease.OutQuint);

        shieldObj.CardObject = cardObj;
        cardObj.transform.parent = holderTransform;
    }

    private IEnumerator PlayMakeShieldAnimationRoutine(int shieldIndex)
    {
        _playerData.Shields[shieldIndex].SetAnimatorTrigger(_shieldUnbreakTriggerHash);
        _playerData.Shields[shieldIndex].CardHolder.DOScale(Vector3.zero, _animationTime).SetEase(Ease.OutQuint);

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
