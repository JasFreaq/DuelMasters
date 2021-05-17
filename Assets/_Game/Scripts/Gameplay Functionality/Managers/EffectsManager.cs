using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
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
    [SerializeField] private Transform _intermediateTransform;

    [Header("Tween Parameters")] 
    [SerializeField] private bool _isPlayer = true;
    [SerializeField] private float _makeShieldPauseTime = 0.5f;
    [SerializeField] private float _fromShieldsTransitionTime = 2f;
    [SerializeField] private float _toShieldsTransitionTime = 2f;
    [SerializeField] private float _fromDeckTransitionTime = 2f;
    [SerializeField] private float _fromHandTransitionTime = 2f;
    [SerializeField] private float _toHandTransitionTime = 2f;
    [SerializeField] private float _fromManaTransitionTime = 1f;
    [SerializeField] private float _toManaTransitionTime = 1f;
    [SerializeField] private float _toBattleTransitionTime = 1f;

    private void Awake()
    {
        _shieldsManager.Initialize(_isPlayer, _makeShieldPauseTime, _fromShieldsTransitionTime, _toShieldsTransitionTime);
    }

    private int counter = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(BreakShieldRoutine(counter++));
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(MakeShieldFromHandRoutine(0));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(DrawCardRoutine());
        }

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    StartCoroutine(ChargeManaRoutine(0));
        //}
    }


    public Coroutine SetupShields()
    {
        CardManager[] cards = new CardManager[5];
        for (int i = 0; i < 5; i++)
        {
            cards[i] = _deckManager.RemoveTopCard();
        }

        return StartCoroutine(_shieldsManager.SetupShieldsRoutine(cards));
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

    public IEnumerator ChargeManaRoutine(int index)
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
        CardManager card = _shieldsManager.GetCardAtIndex(shieldIndex);
        yield return _shieldsManager.BreakShieldRoutine(shieldIndex, _intermediateTransform);
        yield return StartCoroutine(MoveToHandRoutine(card.transform));
        card.HoverPreview.PreviewEnabled = true;
    }
    
    public IEnumerator MakeShieldFromHandRoutine(int index)
    {
        CardManager card = _handManager.RemoveCardAtIndex(index);
        card.CardLayout.Canvas.sortingOrder = 100;
        
        card.HoverPreview.PreviewEnabled = false;
        yield return MoveFromHandRoutine(card.transform, true);
        if (!_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(false);
        
        yield return StartCoroutine(_shieldsManager.MakeShieldRoutine(card));
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
        cardTransform.DOMove(_intermediateTransform.position, _fromDeckTransitionTime).SetEase(Ease.OutQuint);
        if (_isPlayer)
            cardTransform.DORotate(_intermediateTransform.rotation.eulerAngles, _fromDeckTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DOScale(Vector3.one, _fromDeckTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromDeckTransitionTime);
    }
    
    private IEnumerator MoveFromHandRoutine(Transform cardTransform, bool forShield = false)
    {
        cardTransform.DOMove(_intermediateTransform.position, _fromHandTransitionTime).SetEase(Ease.OutQuint);
        Vector3 rotation = new Vector3(-90, 0, 0);
        if (forShield && !_isPlayer)
            rotation = new Vector3(90, 0, 0);
        cardTransform.DORotate(rotation, _fromHandTransitionTime).SetEase(Ease.OutQuint);
        
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
        cardTransform.DOMove(_intermediateTransform.position, _fromManaTransitionTime).SetEase(Ease.OutQuint);
        cardTransform.DORotate(_intermediateTransform.rotation.eulerAngles, _fromManaTransitionTime).SetEase(Ease.OutQuint);

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
