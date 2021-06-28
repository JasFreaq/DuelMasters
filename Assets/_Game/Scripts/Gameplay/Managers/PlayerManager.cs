using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour
{
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

    private List<CardInstanceObject> _playableCards = new List<CardInstanceObject>();
    private CardInstanceObject _currentlySelected;
    private bool _canSelect = false;

    public PlayerDataHandler DataHandler
    {
        get { return _playerData; }
    }

    public ManaZoneManager ManaZoneManager
    {
        get { return _manaZoneManager; }
    }

    public CardInstanceObject CurrentlySelected
    {
        get { return _currentlySelected; }
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
            CardInstanceObject card = _playerData.CardsInHand[_handManager.transform.GetChild(0).GetChild(0).GetInstanceID()];
            if (Input.GetKeyDown(KeyCode.M))
                StartCoroutine(ChargeManaRoutine(card));
            if (Input.GetKeyDown(KeyCode.P))
                StartCoroutine(PlayCardRoutine(card));
            if (Input.GetKeyDown(KeyCode.S))
                StartCoroutine(MakeShieldFromHandRoutine(card));
        }

        if (_manaZoneManager.transform.GetChild(0).childCount > 0)
        {
            CardInstanceObject card = _playerData.CardsInMana[_manaZoneManager.transform.GetChild(0).GetChild(0).GetInstanceID()];
            if (Input.GetKeyDown(KeyCode.N))
                StartCoroutine(ReturnFromManaRoutine(card));
        }
    }

    #region Setup Methods

    public void Initialize(Deck deck, Action<CardInstanceObject> processAction)
    {
        _deckManager.Initialize(deck, processAction, SelectCard);
    }

    public Coroutine SetupShields()
    {
        CardInstanceObject[] cards = new CardInstanceObject[5];
        for (int i = 0; i < 5; i++)
            cards[i] = _deckManager.RemoveTopCard();

        return StartCoroutine(_shieldsManager.SetupShieldsRoutine(cards));
    }

    #endregion

    #region Interactivity Methods

    private void SelectCard(CardInstanceObject card)
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
                        foreach (CardInstanceObject cardManager in _playableCards)
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
                            foreach (CardInstanceObject cardManager in _playableCards)
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

        foreach (KeyValuePair<int, CardInstanceObject> pair in _playerData.CardsInHand)
        {
            CardInstanceObject card = pair.Value;
            if (_playerData.CanPayCost(card.CardData.Civilization, card.CardData.Cost))
            {
                card.SetHighlight(true);
                _playableCards.Add(card);
            }
        }

        return _playableCards.Count;
    }

    #endregion
    
    #region From-Hand Transitions

    public IEnumerator ChargeManaRoutine(CardInstanceObject card)
    {
        card.ManaLayout.Canvas.sortingOrder = 100;
        card.HoverPreviewHandler.PreviewEnabled = false;
        
        yield return _handManager.MoveFromHandRoutine(card);

        card.ActivateManaLayout();

        yield return _manaZoneManager.MoveToManaZoneRoutine(card);

        card.HoverPreviewHandler.PreviewEnabled = true;
    }

    public IEnumerator PlayCardRoutine(CardInstanceObject card)
    {
        card.HoverPreviewHandler.PreviewEnabled = false;

        yield return _handManager.MoveFromHandRoutine(card);

        if (card is CreatureInstanceObject creatureCard)
            yield return StartCoroutine(SummonCreatureRoutine(creatureCard));
        else if (card is SpellInstanceObject spellCard)
            yield return StartCoroutine(CastSpellRoutine(spellCard));
    }

    private IEnumerator SummonCreatureRoutine(CreatureInstanceObject creatureCard)
    {
        creatureCard.ActivateBattleLayout();
        yield return _battleZoneManager.MoveToBattleZoneRoutine(creatureCard);
        creatureCard.HoverPreviewHandler.PreviewEnabled = true;
    }

    private IEnumerator CastSpellRoutine(SpellInstanceObject spellCard)
    {
        spellCard.gameObject.SetActive(false);
        _graveyardManager.AddCard(spellCard);
        spellCard.gameObject.SetActive(true);

        yield break;
    }
    
    public IEnumerator MakeShieldFromHandRoutine(CardInstanceObject card)
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

    public IEnumerator DrawCardRoutine(bool disableInteraction = false)
    {
        CardInstanceObject card = _deckManager.RemoveTopCard();
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

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        CardInstanceObject card = _shieldsManager.GetShieldAtIndex(shieldIndex);
        yield return _shieldsManager.BreakShieldRoutine(shieldIndex);
        yield return _handManager.MoveToHandRoutine(card);
        card.HoverPreviewHandler.PreviewEnabled = true;
    }

    public IEnumerator ReturnFromManaRoutine(CardInstanceObject card)
    {
        card.CardLayout.Canvas.sortingOrder = 100;
        card.HoverPreviewHandler.PreviewEnabled = false;
        card.IsVisible = true;

        yield return _manaZoneManager.MoveFromManaZoneRoutine(card);
        card.ActivateCardLayout();
        yield return _handManager.MoveToHandRoutine(card);

        card.HoverPreviewHandler.PreviewEnabled = true;
    }
    
    public IEnumerator ReturnFromBattleRoutine(CardInstanceObject card)
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
