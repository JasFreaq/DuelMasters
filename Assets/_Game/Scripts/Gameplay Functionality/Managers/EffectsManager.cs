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
    [SerializeField] private float _fromManaTransitionTime = 1f;
    [SerializeField] private float _toManaTransitionTime = 1f;

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
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            ReturnFromMana(0);
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
        CardManager card = _handManager.RemoveCardAtIndex(index);
        card.CardLayout.Canvas.sortingOrder = 100;

        StartCoroutine(AddManaRoutine(card, card.CardData));
    }

    public void ReturnFromMana(int index)
    {
        CardManager card = _manaZoneManager.RemoveCardAtIndex(index);
        card.CardLayout.Canvas.sortingOrder = 100;

        StartCoroutine(ReturnFromManaRoutine(card));
    }

    private IEnumerator DrawCardRoutine(CardManager card)
    {
        yield return StartCoroutine(MoveFromDeckRoutine(card.transform));
        yield return StartCoroutine(MoveToHandRoutine(card.transform));
        card.HoverPreview.PreviewEnabled = true;
    }

    private IEnumerator AddManaRoutine(CardManager card, CardData cardData)
    {
        card.HoverPreview.PreviewEnabled = false;
        yield return MoveFromHandRoutine(card.transform);
        card.ActivateManaLayout();
        yield return MoveToManaZoneRoutine(card.transform, cardData);
        card.HoverPreview.PreviewEnabled = true;
    }
    
    private IEnumerator ReturnFromManaRoutine(CardManager card)
    {
        card.HoverPreview.PreviewEnabled = false;
        yield return MoveFromManaZoneRoutine(card.transform);
        card.ActivateCardLayout();
        yield return MoveToHandRoutine(card.transform, true);
        card.HoverPreview.PreviewEnabled = true;
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

    private IEnumerator MoveToHandRoutine(Transform cardTransform, bool opponentVisible = false)
    {
        Transform tempCard = _handManager.AssignTempCard();
        cardTransform.DOMove(tempCard.position, _toHandTransitionTime).SetEase(Ease.OutQuint);

        Vector3 rotation = tempCard.rotation.eulerAngles;
        if (!_isPlayer && opponentVisible)
        {
            rotation -= new Vector3(0, 0, 180);
        }
        cardTransform.DORotate(rotation, _toHandTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toHandTransitionTime);

        if (!_isPlayer && opponentVisible)
        {
            //TODO: Call Visible Icon Code
        }

        _handManager.AddCard(cardTransform);
    }

    private IEnumerator MoveFromManaZoneRoutine(Transform cardTransform)
    {
        cardTransform.DOMove(_drawIntermediateTransform.position, _fromManaTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(_drawIntermediateTransform.rotation.eulerAngles, _fromManaTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromManaTransitionTime);
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
