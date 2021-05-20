using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour
{
    [Header("Manager Caches")] 
    [SerializeField] private DeckManager _deckManager;
    [SerializeField] private ShieldsManager _shieldsManager;
    [SerializeField] private HandManager _handManager;
    [SerializeField] private ManaZoneManager _manaZoneManager;
    [SerializeField] private BattleZoneManager _battleZoneManager;
    [SerializeField] private GraveyardManager _graveyardManager;

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
    [SerializeField] private float _fromBattleTransitionTime = 1f;
    [SerializeField] private float _toBattleTransitionTime = 1f;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(PlayCardRoutine(0));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(DrawCardRoutine());
        }
    }

    public void Initialize(Deck deck)
    {
        _deckManager.Initialize(deck, _isPlayer, _fromDeckTransitionTime, _intermediateTransform);
        
        _shieldsManager.Initialize(_isPlayer, _makeShieldPauseTime, _fromShieldsTransitionTime,
            _toShieldsTransitionTime, _intermediateTransform);
        
        _handManager.Initialize(_isPlayer, _fromHandTransitionTime, _toHandTransitionTime, _intermediateTransform);
        
        _manaZoneManager.Initialize(_fromManaTransitionTime, _toManaTransitionTime, _intermediateTransform);
        
        _battleZoneManager.Initialize(_fromBattleTransitionTime, _toBattleTransitionTime, _intermediateTransform);
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

        yield return _deckManager.MoveFromDeckRoutine(card.transform);
        yield return _handManager.MoveToHandRoutine(card.transform);
        card.HoverPreview.PreviewEnabled = true;
    }

    public IEnumerator ChargeManaRoutine(int index)
    {
        CardManager card = _handManager.RemoveCardAtIndex(index);
        card.ManaLayout.Canvas.sortingOrder = 100;

        card.HoverPreview.PreviewEnabled = false;
        yield return _handManager.MoveFromHandRoutine(card.transform);
        card.ActivateManaLayout();
        yield return _manaZoneManager.MoveToManaZoneRoutine(card.transform, card.Card);
        card.HoverPreview.PreviewEnabled = true;
    }

    public IEnumerator PlayCardRoutine(int index)
    {
        CardManager card = _handManager.RemoveCardAtIndex(index);
        card.HoverPreview.PreviewEnabled = false;
        yield return _handManager.MoveFromHandRoutine(card.transform);

        if (card is CreatureCardManager creatureCard)
            yield return StartCoroutine(SummonCreatureRoutine(creatureCard));
        else if (card is SpellCardManager spellCard)
            yield return StartCoroutine(CastSpellRoutine(spellCard));
    }

    private IEnumerator SummonCreatureRoutine(CreatureCardManager creatureCard)
    {
        creatureCard.ActivateBattleLayout();
        yield return _battleZoneManager.MoveToBattleZoneRoutine(creatureCard.transform);
        creatureCard.HoverPreview.PreviewEnabled = true;
    }

    private IEnumerator CastSpellRoutine(SpellCardManager spellCard)
    {
        spellCard.gameObject.SetActive(false);
        _graveyardManager.AddCard(spellCard.transform);
        spellCard.gameObject.SetActive(true);

        yield break;
    }

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        CardManager card = _shieldsManager.GetCardAtIndex(shieldIndex);
        yield return _shieldsManager.BreakShieldRoutine(shieldIndex);
        yield return _handManager.MoveToHandRoutine(card.transform);
        card.HoverPreview.PreviewEnabled = true;
    }
    
    public IEnumerator MakeShieldFromHandRoutine(int index)
    {
        CardManager card = _handManager.RemoveCardAtIndex(index);
        card.CardLayout.Canvas.sortingOrder = 100;
        
        card.HoverPreview.PreviewEnabled = false;
        yield return _handManager.MoveFromHandRoutine(card.transform, true);
        if (!_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(false);
        
        yield return StartCoroutine(_shieldsManager.MakeShieldRoutine(card));
    }

    public IEnumerator ReturnFromManaRoutine(int index)
    {
        CardManager card = _manaZoneManager.RemoveCardAtIndex(index);
        card.CardLayout.Canvas.sortingOrder = 100;

        card.HoverPreview.PreviewEnabled = false;
        yield return _manaZoneManager.MoveFromManaZoneRoutine(card.transform);
        card.ActivateCardLayout();
        yield return _handManager.MoveToHandRoutine(card.transform, true);
        card.HoverPreview.PreviewEnabled = true;
    }
    
    public IEnumerator ReturnFromBattleRoutine(int index)
    {
        CardManager card = _battleZoneManager.RemoveCardAtIndex(index);
        card.CardLayout.Canvas.sortingOrder = 100;

        card.HoverPreview.PreviewEnabled = false;
        yield return _battleZoneManager.MoveFromBattleZoneRoutine(card.transform);
        card.ActivateCardLayout();
        yield return _handManager.MoveToHandRoutine(card.transform, true);
        card.HoverPreview.PreviewEnabled = true;
    }
}
