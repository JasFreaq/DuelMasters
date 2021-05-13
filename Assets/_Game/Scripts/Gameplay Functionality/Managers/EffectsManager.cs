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

    [Header("Position Markers")]
    [SerializeField] private Transform _drawIntermediateTransform;

    [Header("Tween Parameters")] 
    [SerializeField] private bool _drawFlip = true;
    [SerializeField] private float _drawTransitionTimeA = 2f;
    [SerializeField] private float _drawTransitionTimeB = 2f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        CardLayoutHandler cardLayout = _deckManager.RemoveTopCard();
        cardLayout.Canvas.sortingOrder = 100;
        cardLayout.Canvas.gameObject.SetActive(true);

        StartCoroutine(DrawCardRoutine(cardLayout.transform));
    }

    public void AddMana(int index)
    {
        CardLayoutHandler cardLayout = _handManager.GetCardLayoutAtIndex(index);
    }

    private IEnumerator DrawCardRoutine(Transform cardTransform)
    {
        yield return StartCoroutine(MoveFromDeckRoutine(cardTransform, _drawTransitionTimeA));
        yield return StartCoroutine(MoveToHandRoutine(cardTransform, _drawTransitionTimeB));
    }

    private IEnumerator MoveFromDeckRoutine(Transform cardTransform, float transitionTime)
    {
        cardTransform.DOMove(_drawIntermediateTransform.position, transitionTime).SetEase(Ease.OutQuint);
        if (_drawFlip)
            cardTransform.DORotate(_drawIntermediateTransform.rotation.eulerAngles, transitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(Vector3.one, transitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(transitionTime);
    }

    private IEnumerator MoveToHandRoutine(Transform cardTransform, float transitionTime)
    {
        Transform tempCard = _handManager.AssignTempCard();
        cardTransform.DOMove(tempCard.position, transitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(tempCard.rotation.eulerAngles, transitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(transitionTime);

        _handManager.AddCard(cardTransform);
    }
}
