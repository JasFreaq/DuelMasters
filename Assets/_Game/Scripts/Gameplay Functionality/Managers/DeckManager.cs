using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class DeckManager : MonoBehaviour
{
    private bool _isPlayer = true;
    private float _fromTransitionTime = 2f;
    private Transform _intermediateHolder;

    private DeckLayoutHandler _deckLayoutHandler;

    private void Awake()
    {
        _deckLayoutHandler = GetComponent<DeckLayoutHandler>();
    }

    public void Initialize(Deck deck, bool isPlayer, float fromTransitionTime, Transform intermediateTransform)
    {
        _deckLayoutHandler.SetupDeck(deck.GetCards());
        _isPlayer = isPlayer;
        _fromTransitionTime = fromTransitionTime;
        _intermediateHolder = intermediateTransform;
    }

    public IEnumerator MoveFromDeckRoutine(Transform cardTransform)
    {
        cardTransform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        if (_isPlayer)
            cardTransform.DORotate(_intermediateHolder.rotation.eulerAngles, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    public CardManager GetTopCard()
    {
        return _deckLayoutHandler.GetTopCard();
    }

    public CardManager RemoveTopCard()
    {
        CardManager card = _deckLayoutHandler.RemoveTopCard();
        return card;
    }
}
