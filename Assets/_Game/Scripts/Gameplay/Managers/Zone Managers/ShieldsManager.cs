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
    [SerializeField] private float _animationTime = 1f;
    
    [Header("Transition")]
    [SerializeField] private float _pauseTime = 0.5f;
    [SerializeField] private float _fromTransitionTime = 1.5f;
    [SerializeField] private float _toTransitionTime = 1.5f;
    [SerializeField] private Transform _intermediateHolder;
    
    private PlayerDataHandler _playerData;
    private List<ShieldObject> _shields = new List<ShieldObject>();

    private void Start()
    {
        _playerData = GameDataHandler.Instance.GetDataHandler(_isPlayer);
        
        for (int i = 0; i < GameParamsHolder.Instance.BaseCardCount; i++)
        {
            ShieldObject shieldObj = Instantiate(_shieldPrefab, _holderTransform);
            _playerData.Shields.Add(shieldObj);
            _shields.Add(shieldObj);
        }

        ArrangeShields();
    }

    #region Functionality Methods

    public IEnumerator SetupShieldsRoutine(CardObject[] cards)
    {
        int baseShieldCountMinusOne = GameParamsHolder.Instance.BaseCardCount - 1;
        for (int i = baseShieldCountMinusOne; i >= 0; i--)
        {
            MoveToShields(cards[i], _shields[i]);
            yield return new WaitForSeconds(_pauseTime);
        }

        yield return new WaitForSeconds(_pauseTime);

        for (int i = 0; i < baseShieldCountMinusOne; i++)
        {
            AddShield(i, cards[i]);
            StartCoroutine(PlayMakeShieldAnimationRoutine(i));
        }
        AddShield(baseShieldCountMinusOne, cards[baseShieldCountMinusOne]);
        yield return StartCoroutine(PlayMakeShieldAnimationRoutine(baseShieldCountMinusOne));
    }

    public IEnumerator BreakShieldRoutine(ShieldObject shieldObj)
    {
        shieldObj.SetAnimatorTrigger(true);
        shieldObj.CardHolder.DOScale(shieldObj.HolderScale, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);

        CardObject cardObj = shieldObj.CardObj;
        cardObj.CardLayout.Canvas.sortingOrder = 100;
        cardObj.transform.parent = transform;
        cardObj.CardLayout.Canvas.gameObject.SetActive(true);
        
        int shieldIndex = shieldObj.transform.GetSiblingIndex();
        int playerShieldIndex = shieldIndex;
        for (int i = 0; i <= shieldIndex; i++)
        {
            if (!_shields[i].ActiveState)
                playerShieldIndex--;
        }

        shieldObj.ActivateShield(false, null);
        if (_shields.Count > GameParamsHolder.Instance.BaseCardCount && shieldIndex < _shields.Count)
        {
            _shields.RemoveAt(shieldIndex);
            shieldObj.transform.parent = transform;
            Destroy(shieldObj.gameObject);
            ArrangeShields();
        }
        _playerData.Shields.RemoveAt(playerShieldIndex);

        yield return StartCoroutine(MoveFromShieldsRoutine(cardObj));
    }

    public IEnumerator MakeShieldRoutine(CardObject cardObj)
    {
        int emptyIndex = GetEmptyIndex();
        AddShield(emptyIndex, cardObj);
        MoveToShields(cardObj, _shields[emptyIndex]);
        yield return new WaitForSeconds(_toTransitionTime);

        yield return StartCoroutine(PlayMakeShieldAnimationRoutine(emptyIndex));
    }
    
    private void AddShield(int shieldIndex, CardObject cardObj)
    {
        ShieldObject shieldObj;
        if (_shields.Count > shieldIndex)
        {
            shieldObj = _shields[shieldIndex];
            shieldObj.ActivateShield(true, cardObj);
        }
        else
        {
            shieldObj = Instantiate(_shieldPrefab, _holderTransform);
            shieldObj.ActivateShield(true, cardObj);
            _shields.Add(shieldObj);
            ArrangeShields(true);
        }
        
        if (!_playerData.Shields.Contains(shieldObj))
            _playerData.Shields.Add(shieldObj);

        cardObj.CardInst.SetCurrentZone(CardZoneType.Shields);
    }

    private int GetEmptyIndex()
    {
        int n = _shields.Count;
        for (int i = 0; i < n; i++)
        {
            if (_shields[i].CardObj == null)
            {
                return i;
            }
        }

        return n;
    }

    #endregion

    #region Transition Methods

    private IEnumerator MoveFromShieldsRoutine(CardObject cardObj)
    {
        Transform cardTransform = cardObj.transform;
        cardTransform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        Quaternion rotation = _intermediateHolder.rotation;
        cardTransform.DORotateQuaternion(rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    private void MoveToShields(CardObject cardObj, ShieldObject shieldObj)
    {
        Transform holderTransform = shieldObj.CardHolder;
        Transform cardTransform = cardObj.transform;
        cardTransform.DOMove(holderTransform.position, _toTransitionTime).SetEase(Ease.OutQuint);
        
        Quaternion rotation = holderTransform.rotation;
        if (!_isPlayer)
            rotation = Quaternion.Euler(0, 0, 0);
        cardTransform.DORotateQuaternion(rotation, _toTransitionTime).SetEase(Ease.OutQuint);

        cardTransform.DOScale(holderTransform.lossyScale, _toTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.parent = holderTransform;
    }

    private IEnumerator PlayMakeShieldAnimationRoutine(int shieldIndex)
    {
        _shields[shieldIndex].SetAnimatorTrigger(false);
        _shields[shieldIndex].CardHolder.DOScale(Vector3.zero, _animationTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_animationTime);
    }

    #endregion

    #region Layout Methods

    private void ArrangeShields(bool newShield = false)
    {
        int n = _holderTransform.childCount;
        float shieldWidth = Mathf.Min((_shieldAreaWidth * 2) / n, _maxShieldWidth);
        float arrangeTime = GameParamsHolder.Instance.LayoutsArrangeMoveTime;
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
            Vector3 shieldScale = new Vector3(sizeRatio, sizeRatio, sizeRatio);

            if (i == n - 1 && newShield)
            {
                shieldTransform.localPosition = shieldPos;
                shieldTransform.localScale = shieldScale;
            }
            else
            {
                shieldTransform.DOLocalMove(shieldPos, arrangeTime).SetEase(Ease.OutQuint);
                shieldTransform.DOScale(shieldScale, arrangeTime).SetEase(Ease.OutQuint);
            }
        }
    }

    #endregion
}
