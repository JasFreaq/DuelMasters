using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private bool _isPlayer = true;

    [Header("Manager Caches")] 
    [SerializeField] private DeckManager _deckManager;
    [SerializeField] private ShieldsManager _shieldsManager;
    [SerializeField] private HandManager _handManager;
    [SerializeField] private ManaZoneManager _manaZoneManager;
    [SerializeField] private BattleZoneManager _battleZoneManager;
    [SerializeField] private GraveyardManager _graveyardManager;

    private PlayerDataHandler _playerData;

    private List<CardObject> _playableCards = new List<CardObject>();

    public PlayerDataHandler DataHandler
    {
        get { return _playerData; }
    }

    public ManaZoneManager ManaZoneManager
    {
        get { return _manaZoneManager; }
    }

    public BattleZoneManager BattleZoneManager
    {
        get { return _battleZoneManager; }
    }

    public GraveyardManager GraveyardManager
    {
        get { return _graveyardManager; }
    }

    public IReadOnlyList<CardObject> PlayableCards
    {
        get { return _playableCards; }
    }
    
    private void Awake()
    {
        _playerData = GetComponent<PlayerDataHandler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(DrawCardRoutine());
        if (Input.GetKeyDown(KeyCode.B))
            StartCoroutine(BreakShieldRoutine(0));

        if (_handManager.transform.GetChild(0).childCount > 0) 
        {
            CardObject card = _playerData.CardsInHand[_handManager.transform.GetChild(0).GetChild(0).GetInstanceID()];
            if (Input.GetKeyDown(KeyCode.M))
                StartCoroutine(ChargeManaRoutine(card));
            if (Input.GetKeyDown(KeyCode.P))
                StartCoroutine(PlayCardRoutine(card));
            if (Input.GetKeyDown(KeyCode.S))
                StartCoroutine(MakeShieldFromHandRoutine(card));
        }

        if (_manaZoneManager.transform.GetChild(0).childCount > 0)
        {
            CardObject card = _playerData.CardsInMana[_manaZoneManager.transform.GetChild(0).GetChild(0).GetInstanceID()];
            if (Input.GetKeyDown(KeyCode.N))
                StartCoroutine(ReturnFromManaRoutine(card));
        }
    }

    #region Setup Methods

    public void GenerateDeck(List<CardInstance> cardsInsts, Action<CardObject> processAction)
    {
        _deckManager.GenerateCardObjects(cardsInsts, processAction);
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        GameDataHandler.Instance.ShuffleCards(_isPlayer);
        _deckManager.ArrangeCards();
    }

    public Coroutine SetupShields()
    {
        CardObject[] cards = new CardObject[5];
        for (int i = 0; i < 5; i++)
            cards[i] = _deckManager.RemoveTopCard();

        return StartCoroutine(_shieldsManager.SetupShieldsRoutine(cards));
    }

    #endregion

    #region Affordance Methods
    
    public int HighlightPlayableCards()
    {
        _playableCards.Clear();

        foreach (KeyValuePair<int, CardObject> pair in _playerData.CardsInHand)
        {
            CardObject cardObj = pair.Value;
            CardInstance cardInst = cardObj.CardInst;

            if (_playerData.CanPayCost(cardInst.CardData.Civilization, cardInst.CardData.Cost))
            {
                cardObj.SetHighlight(true);
                _playableCards.Add(cardObj);
            }
        }

        return _playableCards.Count;
    }
    
    #endregion

    #region From-Hand Transitions

    public IEnumerator ChargeManaRoutine(CardObject card)
    {
        card.ManaLayout.Canvas.sortingOrder = 100;
        card.HoverPreviewHandler.PreviewEnabled = false;
        
        yield return _handManager.MoveFromHandRoutine(card);

        card.ActivateManaLayout();

        yield return _manaZoneManager.MoveToManaZoneRoutine(card);

        card.HoverPreviewHandler.PreviewEnabled = true;
    }

    public IEnumerator PlayCardRoutine(CardObject card)
    {
        card.HoverPreviewHandler.PreviewEnabled = false;

        yield return _handManager.MoveFromHandRoutine(card);

        if (card is CreatureObject creatureCard)
            yield return StartCoroutine(SummonCreatureRoutine(creatureCard));
        else if (card is SpellObject spellCard)
            yield return StartCoroutine(CastSpellRoutine(spellCard));
    }

    private IEnumerator SummonCreatureRoutine(CreatureObject creatureCard)
    {
        creatureCard.ActivateBattleLayout();
        yield return _battleZoneManager.MoveToBattleZoneRoutine(creatureCard);
        creatureCard.HoverPreviewHandler.PreviewEnabled = true;
    }

    private IEnumerator CastSpellRoutine(SpellObject spellCard)
    {
        spellCard.DestroyCard();

        yield break;
    }
    
    public IEnumerator MakeShieldFromHandRoutine(CardObject card)
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

    public IEnumerator DrawCardRoutine()
    {
        CardObject card = _deckManager.RemoveTopCard();
        card.CardLayout.Canvas.sortingOrder = 100;
        card.CardLayout.Canvas.gameObject.SetActive(true);

        yield return _deckManager.MoveFromDeckRoutine(card);
        yield return _handManager.MoveToHandRoutine(card);

        card.HoverPreviewHandler.PreviewEnabled = true;
        card.CanDrag = true;
    }

    public IEnumerator BreakShieldRoutine(int shieldIndex)
    {
        CardObject card = _shieldsManager.GetShieldAtIndex(shieldIndex);
        yield return _shieldsManager.BreakShieldRoutine(shieldIndex);
        yield return _handManager.MoveToHandRoutine(card);
        card.HoverPreviewHandler.PreviewEnabled = true;
    }

    public IEnumerator ReturnFromManaRoutine(CardObject card)
    {
        card.CardLayout.Canvas.sortingOrder = 100;
        card.HoverPreviewHandler.PreviewEnabled = false;
        card.IsVisible = true;

        yield return _manaZoneManager.MoveFromManaZoneRoutine(card);
        card.ActivateCardLayout();
        yield return _handManager.MoveToHandRoutine(card);

        card.HoverPreviewHandler.PreviewEnabled = true;
    }
    
    public IEnumerator ReturnFromBattleRoutine(CardObject card)
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
