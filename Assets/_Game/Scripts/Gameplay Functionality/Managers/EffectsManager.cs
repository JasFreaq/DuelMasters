using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class EffectsManager : MonoBehaviour
{
    [Header("Manager Caches")] 
    [SerializeField] private DeckManager _deckManager;
    [SerializeField] private HandManager _handManager;
    [SerializeField] private ManaZoneManager _manaZoneManager;

    [Header("Position Markers")]
    [SerializeField] private Transform _drawIntermediateTransform;

    [Header("Tween Parameters")] 
    [SerializeField] private bool _isPlayer = true;
    [SerializeField] private float _fromDeckTransitionTime = 2f;
    [SerializeField] private float _fromHandTransitionTime = 2f;
    [SerializeField] private float _toHandTransitionTime = 2f;
    [SerializeField] private float _toManaTransitionTime = 2f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawCard();
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddMana(0);
        }
    }

    public void DrawCard()
    {
        CardManager card = _deckManager.RemoveTopCard();
        card.CardLayout.Canvas.sortingOrder = 100;
        card.CardLayout.Canvas.gameObject.SetActive(true);

        StartCoroutine(DrawCardRoutine(card));
    }

    public void AddMana(int index)
    {
        CardManager card = _handManager.RemoveCardLayoutAtIndex(index);
        card.CardLayout.Canvas.sortingOrder = 100;

        StartCoroutine(AddManaRoutine(card, card.CardData));
    }

    private IEnumerator DrawCardRoutine(CardManager card)
    {
        yield return StartCoroutine(MoveFromDeckRoutine(card.transform));
        yield return StartCoroutine(MoveToHandRoutine(card.transform));
    }

    private IEnumerator AddManaRoutine(CardManager card, CardData cardData)
    {
        yield return MoveFromHandRoutine(card.transform);
        card.ActivateManaLayout();
        yield return MoveToManaZoneRoutine(card.transform, cardData);
    }

    private IEnumerator MoveFromDeckRoutine(Transform cardTransform)
    {
        cardTransform.DOMove(_drawIntermediateTransform.position, _fromDeckTransitionTime).SetEase(Ease.OutQuint);
        if (_isPlayer)
            cardTransform.DORotate(_drawIntermediateTransform.rotation.eulerAngles, _fromDeckTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(Vector3.one, _fromDeckTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromDeckTransitionTime);
    }

    private IEnumerator MoveFromHandRoutine(Transform cardTransform)
    {
        cardTransform.DOMove(_drawIntermediateTransform.position, _fromHandTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(new Vector3(-90, 0, 0), _fromHandTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromHandTransitionTime);
    }

    private IEnumerator MoveToHandRoutine(Transform cardTransform)
    {
        Transform tempCard = _handManager.AssignTempCard();
        cardTransform.DOMove(tempCard.position, _toHandTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(tempCard.rotation.eulerAngles, _toHandTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toHandTransitionTime);

        _handManager.AddCard(cardTransform);
    }

    private IEnumerator MoveToManaZoneRoutine(Transform cardTransform, CardData cardData)
    {
        Transform tempCard = _manaZoneManager.AssignTempCard(cardData);
        cardTransform.DOMove(tempCard.position, _toManaTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(tempCard.rotation.eulerAngles, _toManaTransitionTime).SetEase(Ease.OutQuint);
        if (!_isPlayer)
            cardTransform.DOScale(_manaZoneManager.transform.localScale, _fromDeckTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toHandTransitionTime);

        _manaZoneManager.AddCard(cardTransform);
    }
}
