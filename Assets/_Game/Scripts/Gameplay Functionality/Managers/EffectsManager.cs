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
    [SerializeField] private ShieldsManager _shieldsManager;
    [SerializeField] private HandManager _handManager;
    [SerializeField] private ManaZoneManager _manaZoneManager;
    [SerializeField] private BattleZoneManager _battleZoneManager;

    [Header("Position Markers")]
    [SerializeField] private Transform _drawIntermediateTransform;

    [Header("Tween Parameters")] 
    [SerializeField] private bool _isPlayer = true;
    [SerializeField] private float _makeShieldTransitionTime = 2f;
    [SerializeField] private float _makeShieldPauseTime = 0.5f;
    [SerializeField] private float _fromShieldsTransitionTime = 2f;
    [SerializeField] private float _fromDeckTransitionTime = 2f;
    [SerializeField] private float _fromHandTransitionTime = 2f;
    [SerializeField] private float _toHandTransitionTime = 2f;
    [SerializeField] private float _fromManaTransitionTime = 1f;
    [SerializeField] private float _toManaTransitionTime = 1f;
    [SerializeField] private float _toBattleTransitionTime = 1f;
    private int counter = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(BreakShieldRoutine(counter++));
        }
    }
    
    public IEnumerator SetupShieldsRoutine()
    {
        CardManager[] _cards = new CardManager[5];
        for (int i = 0; i < 5; i++)
        {
            _cards[i] = _deckManager.RemoveTopCard();
            MoveToShields(_cards[i].transform, _shieldsManager.GetCardHolderTransform(i));

            yield return new WaitForSeconds(_makeShieldPauseTime);
        }

        yield return new WaitForSeconds(_makeShieldTransitionTime);

        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(_shieldsManager.AddShieldRoutine(i, _cards[i]));
        }
        yield return StartCoroutine(_shieldsManager.AddShieldRoutine(4, _cards[4]));
    }

    public IEnumerator DrawCardRoutine()
    {
        CardManager card = _deckManager.RemoveTopCard();
        card.CardLayout.Canvas.sortingOrder = 100;
        card.CardLayout.Canvas.gameObject.SetActive(true);

        yield return StartCoroutine(MoveFromDeckRoutine(card.transform));
        yield return StartCoroutine(MoveToHandRoutine(card.transform));
        card.HoverPreview.PreviewEnabled = true;
    }

    public IEnumerator AddManaRoutine(int index)
    {
        CardManager card = _handManager.RemoveCardAtIndex(index);
        card.ManaLayout.Canvas.sortingOrder = 100;

        card.HoverPreview.PreviewEnabled = false;
        yield return MoveFromHandRoutine(card.transform);
        card.ActivateManaLayout();
        yield return MoveToManaZoneRoutine(card.transform, card.CardData);
        card.HoverPreview.PreviewEnabled = true;
    }

    public IEnumerator PlayCardRoutine(int index)
    {
        CardManager card = _handManager.RemoveCardAtIndex(index);
        if (card is CreatureCardManager creatureCard)
            yield return StartCoroutine(SummonCreatureRoutine(creatureCard));
        else if (card is SpellCardManager spellCard)
            yield return StartCoroutine(CastSpellRoutine(spellCard));
    }

    private IEnumerator SummonCreatureRoutine(CreatureCardManager creatureCard)
    {
        creatureCard.HoverPreview.PreviewEnabled = false;
        yield return MoveFromHandRoutine(creatureCard.transform);
        creatureCard.ActivateBattleLayout();
        yield return MoveToBattleZoneRoutine(creatureCard.transform);
        creatureCard.HoverPreview.PreviewEnabled = true;
    }

    private IEnumerator CastSpellRoutine(SpellCardManager spellCard)
    {
        yield break;
    }

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        yield return _shieldsManager.BreakShieldRoutine(shieldIndex);

        CardManager card = _shieldsManager.RemoveCardAtIndex(shieldIndex);
        card.CardLayout.Canvas.sortingOrder = 100;
        if (_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(true);

        yield return StartCoroutine(MoveFromShieldsRoutine(card.transform));
        if (!_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(true);
        yield return StartCoroutine(MoveToHandRoutine(card.transform));
        card.HoverPreview.PreviewEnabled = true;
    }
    
    public IEnumerator MakeShieldRoutine(int index)
    {
        CardManager card = _handManager.RemoveCardAtIndex(index);
        card.CardLayout.Canvas.sortingOrder = 100;
        
        card.HoverPreview.PreviewEnabled = false;
        yield return MoveFromHandRoutine(card.transform);
        yield return StartCoroutine(MoveToHandRoutine(card.transform));
    }

    public IEnumerator ReturnFromManaRoutine(int index)
    {
        CardManager card = _manaZoneManager.RemoveCardAtIndex(index);
        card.CardLayout.Canvas.sortingOrder = 100;

        card.HoverPreview.PreviewEnabled = false;
        yield return MoveFromManaZoneRoutine(card.transform);
        card.ActivateCardLayout();
        yield return MoveToHandRoutine(card.transform, true);
        card.HoverPreview.PreviewEnabled = true;
    }

    #region Move Methods

    private IEnumerator MoveFromDeckRoutine(Transform cardTransform)
    {
        cardTransform.DOMove(_drawIntermediateTransform.position, _fromDeckTransitionTime).SetEase(Ease.OutQuint);
        if (_isPlayer)
            cardTransform.DORotate(_drawIntermediateTransform.rotation.eulerAngles, _fromDeckTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(Vector3.one, _fromDeckTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromDeckTransitionTime);
    }

    private IEnumerator MoveFromShieldsRoutine(Transform cardTransform)
    {
        cardTransform.DOMove(_drawIntermediateTransform.position, _fromShieldsTransitionTime).SetEase(Ease.OutQuint);
        Vector3 rotation = _drawIntermediateTransform.rotation.eulerAngles;
        if (!_isPlayer)
            rotation += new Vector3(0, 0, 180);
        cardTransform.DORotate(rotation, _fromShieldsTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(Vector3.one, _fromShieldsTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromDeckTransitionTime);
    }

    private void MoveToShields(Transform cardTransform, Transform holderTransform)
    {
        cardTransform.transform.DOMove(holderTransform.position, _makeShieldTransitionTime).SetEase(Ease.OutQuint);
        Vector3 rotation = holderTransform.eulerAngles;
        if (!_isPlayer)
            rotation += new Vector3(30, 180, 0);
        cardTransform.transform.DORotate(rotation, _makeShieldTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.transform.DOScale(holderTransform.lossyScale, _makeShieldTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.transform.parent = holderTransform;
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
        cardTransform.DOScale(_manaZoneManager.transform.localScale, _toManaTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toManaTransitionTime);

        _manaZoneManager.AddCard(cardTransform);
    }

    private IEnumerator MoveToBattleZoneRoutine(Transform cardTransform)
    {
        Transform tempCard = _battleZoneManager.AssignTempCard();
        cardTransform.DOMove(tempCard.position, _toBattleTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(tempCard.rotation.eulerAngles, _toBattleTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(tempCard.lossyScale, _toBattleTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_toBattleTransitionTime);

        _battleZoneManager.AddCard(cardTransform);
    }

    #endregion
}
