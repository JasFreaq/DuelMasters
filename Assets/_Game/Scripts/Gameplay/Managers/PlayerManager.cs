using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform _attackTarget;

    [Header("Effect Params")]
    [SerializeField] private bool _isPlayer = true;

    [Header("Manager Caches")] 
    [SerializeField] private DeckManager _deckManager;
    [SerializeField] private ShieldsManager _shieldsManager;
    [SerializeField] private HandManager _handManager;
    [SerializeField] private ManaZoneManager _manaZoneManager;
    [SerializeField] private BattleZoneManager _battleZoneManager;
    [SerializeField] private GraveyardManager _graveyardManager;

    private PlayerDataHandler _playerData;

    private List<CardManager> _playableCards = new List<CardManager>();
    private CardManager _currentlySelected;
    private bool _canSelect = false;

    public PlayerDataHandler DataHandler
    {
        get { return _playerData; }
    }

    public ManaZoneManager ManaZoneManager
    {
        get { return _manaZoneManager; }
    }

    public bool CanSelect
    {
        set { _canSelect = value; }
    }

    private void Awake()
    {
        _playerData = GetComponent<PlayerDataHandler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(DrawCardRoutine());

        if (_handManager.transform.GetChild(0).childCount > 0) 
        {
            CardManager card = _playerData.CardsInHand[_handManager.transform.GetChild(0).GetChild(0).GetInstanceID()];
            if (Input.GetKeyDown(KeyCode.M))
                StartCoroutine(ChargeManaRoutine(card));
            if (Input.GetKeyDown(KeyCode.P))
                StartCoroutine(PlayCardRoutine(card));
            if (Input.GetKeyDown(KeyCode.S))
                StartCoroutine(MakeShieldFromHandRoutine(card));
        }

        if (_manaZoneManager.transform.GetChild(0).childCount > 0)
        {
            CardManager card = _playerData.CardsInMana[_manaZoneManager.transform.GetChild(0).GetChild(0).GetInstanceID()];
            if (Input.GetKeyDown(KeyCode.N))
                StartCoroutine(ReturnFromManaRoutine(card));
        }

        if (Input.GetKeyDown(KeyCode.A))
            _playerData.CardsInBattle[_battleZoneManager.transform.GetChild(0).GetChild(0).GetInstanceID()].AttackTarget(_attackTarget);
    }

    #region Setup Methods

    public void Initialize(Deck deck, Action<CardManager> processAction)
    {
        _deckManager.Initialize(deck, processAction, SelectCard);
    }

    public Coroutine SetupShields()
    {
        CardManager[] cards = new CardManager[5];
        for (int i = 0; i < 5; i++)
            cards[i] = _deckManager.RemoveTopCard();

        return StartCoroutine(_shieldsManager.SetupShieldsRoutine(cards));
    }

    #endregion

    #region Interactivity Methods

    private void SelectCard(CardManager card)
    {
        if (_canSelect) 
        {
            if (_currentlySelected != card)
            {
                _currentlySelected = card;

                switch (GameManager.CurrentStep)
                {
                    case GameStepType.ChargeStep:
                        _currentlySelected.SetHighlight(true);
                        break;

                    case GameStepType.MainStep:
                        foreach (CardManager cardManager in _playableCards)
                        {
                            if (cardManager != _currentlySelected)
                                cardManager.SetHighlight(false);
                        }
                        break;
                }
            }
            else if (_currentlySelected)
            {
                switch (GameManager.CurrentStep)
                {
                    case GameStepType.ChargeStep:
                        _currentlySelected.SetHighlight(false);
                        break;

                    case GameStepType.MainStep:
                        if (_currentlySelected.ProcessAction) 
                            _currentlySelected.SetHighlight(false);
                        else
                        {
                            foreach (CardManager cardManager in _playableCards)
                            {
                                if (cardManager != _currentlySelected)
                                    cardManager.SetHighlight(true);
                            }
                        }
                        break;
                }

                _currentlySelected = null;
            }
        }
    }

    public int HighlightPlayableCards()
    {
        _playableCards.Clear();

        foreach (KeyValuePair<int, CardManager> pair in _playerData.CardsInHand)
        {
            CardManager card = pair.Value;
            if (_playerData.CanPayCost(card.CardData.Civilization, card.CardData.Cost))
            {
                card.SetHighlight(true);
                _playableCards.Add(card);
            }
        }

        return _playableCards.Count;
    }

    #endregion

    public IEnumerator DrawCardRoutine(bool disableInteraction = false)
    {
        CardManager card = _deckManager.RemoveTopCard();
        card.CardLayout.Canvas.sortingOrder = 100;
        card.CardLayout.Canvas.gameObject.SetActive(true);

        yield return _deckManager.MoveFromDeckRoutine(card);
        yield return _handManager.MoveToHandRoutine(card);
        
        if (!disableInteraction)
        {
            card.HoverPreviewHandler.PreviewEnabled = true;
            card.CanDrag = true;
        }
    }

    #region From-Hand Transitions

    public IEnumerator ChargeManaRoutine(CardManager card)
    {
        card.ManaLayout.Canvas.sortingOrder = 100;
        card.HoverPreviewHandler.PreviewEnabled = false;
        
        yield return _handManager.MoveFromHandRoutine(card);

        card.ActivateManaLayout();

        yield return _manaZoneManager.MoveToManaZoneRoutine(card);

        card.HoverPreviewHandler.PreviewEnabled = true;
    }

    public IEnumerator PlayCardRoutine(CardManager card)
    {
        card.HoverPreviewHandler.PreviewEnabled = false;

        yield return _handManager.MoveFromHandRoutine(card);

        if (card is CreatureCardManager creatureCard)
            yield return StartCoroutine(SummonCreatureRoutine(creatureCard));
        else if (card is SpellCardManager spellCard)
            yield return StartCoroutine(CastSpellRoutine(spellCard));
    }

    private IEnumerator SummonCreatureRoutine(CreatureCardManager creatureCard)
    {
        creatureCard.ActivateBattleLayout();
        yield return _battleZoneManager.MoveToBattleZoneRoutine(creatureCard);
        creatureCard.HoverPreviewHandler.PreviewEnabled = true;
    }

    private IEnumerator CastSpellRoutine(SpellCardManager spellCard)
    {
        spellCard.gameObject.SetActive(false);
        _graveyardManager.AddCard(spellCard);
        spellCard.gameObject.SetActive(true);

        yield break;
    }
    
    public IEnumerator MakeShieldFromHandRoutine(CardManager card)
    {
        card.CardLayout.Canvas.sortingOrder = 100;
        card.HoverPreviewHandler.PreviewEnabled = false;

        yield return _handManager.MoveFromHandRoutine(card, true);

        if (!_isPlayer)
            card.CardLayout.Canvas.gameObject.SetActive(false);
        
        yield return StartCoroutine(_shieldsManager.MakeShieldRoutine(card));
    }

    #endregion

    #region To-Hand Transitions

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        CardManager card = _shieldsManager.GetShieldAtIndex(shieldIndex);
        yield return _shieldsManager.BreakShieldRoutine(shieldIndex);
        yield return _handManager.MoveToHandRoutine(card);
        card.HoverPreviewHandler.PreviewEnabled = true;
    }

    public IEnumerator ReturnFromManaRoutine(CardManager card)
    {
        card.CardLayout.Canvas.sortingOrder = 100;
        card.HoverPreviewHandler.PreviewEnabled = false;
        card.IsVisible = true;

        yield return _manaZoneManager.MoveFromManaZoneRoutine(card);
        card.ActivateCardLayout();
        yield return _handManager.MoveToHandRoutine(card);

        card.HoverPreviewHandler.PreviewEnabled = true;
    }
    
    public IEnumerator ReturnFromBattleRoutine(CardManager card)
    {
        card.CardLayout.Canvas.sortingOrder = 100;
        card.HoverPreviewHandler.PreviewEnabled = false;
        card.IsVisible = true;

        yield return _battleZoneManager.MoveFromBattleZoneRoutine(card);
        card.ActivateCardLayout();
        yield return _handManager.MoveToHandRoutine(card);

        card.HoverPreviewHandler.PreviewEnabled = true;
    }

    #endregion
}
