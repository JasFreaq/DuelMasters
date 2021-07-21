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

        if (_handManager.transform.GetChild(0).childCount > 0)
        {
            CardObject card = _playerData.CardsInHand[_handManager.transform.GetChild(0).GetChild(0).GetInstanceID()];
            if (Input.GetKeyDown(KeyCode.M))
                StartCoroutine(ChargeManaRoutine(card));
            if (Input.GetKeyDown(KeyCode.P))
                StartCoroutine(PlayCardRoutine(card));
        }
    }

    #region Setup Methods

    public void GenerateDeck(List<CardInstance> cardsInsts, Action<CardObject> dragReleaseAction)
    {
        _deckManager.GenerateCardObjects(cardsInsts, dragReleaseAction, MoveToGraveyard);
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

    #region Frequent Region Movements

    public IEnumerator ChargeManaRoutine(CardObject cardObj)
    {
        yield return MoveFromHandRoutine(cardObj);
        yield return MoveToManaZoneRoutine(cardObj);
    }

    public IEnumerator PlayCardRoutine(CardObject cardObj)
    {
        yield return MoveFromHandRoutine(cardObj);

        if (cardObj is CreatureObject creatureCard)
            yield return SummonCreatureRoutine(creatureCard);
        else if (cardObj is SpellObject spellCard)
            yield return CastSpellRoutine(spellCard);
    }

    private IEnumerator SummonCreatureRoutine(CreatureObject creatureCard)
    {
        creatureCard.ActivateBattleLayout();
        yield return _battleZoneManager.MoveToBattleZoneRoutine(creatureCard);
        creatureCard.HoverPreviewHandler.PreviewEnabled = true;
    }

    private IEnumerator CastSpellRoutine(SpellObject spellCard)
    {
        yield return MoveToGraveyardRoutine(spellCard);
    }
    
    public IEnumerator DrawCardRoutine()
    {
        Coroutine<CardObject> routine = this.StartCoroutine<CardObject>(ChooseDeckMoveCard(DeckCardMoveType.Top));
        yield return routine.coroutine;

        CardObject cardObj = routine.returnVal;
        yield return MoveFromDeckRoutine(cardObj);
        yield return MoveToHandRoutine(cardObj);
    }

    public IEnumerator BreakShieldRoutine(ShieldObject shieldObj)
    {
        CardObject cardObj = shieldObj.CardObject;
        yield return _shieldsManager.BreakShieldRoutine(shieldObj);
        yield return MoveToHandRoutine(cardObj);
    }

    #endregion

    #region From Zone Moves

    public IEnumerator ChooseDeckMoveCard(DeckCardMoveType deckCardMove)
    {
        CardObject cardObj = null;
        switch (deckCardMove)
        {
            case DeckCardMoveType.Top:
                cardObj = _deckManager.RemoveTopCard();
                break;

            case DeckCardMoveType.SearchShuffle:
                break;
        }

        yield return cardObj;
    }

    public IEnumerator MoveFromDeckRoutine(CardObject cardObj)
    {
        cardObj.CardLayout.Canvas.sortingOrder = 100;
        cardObj.CardLayout.Canvas.gameObject.SetActive(true);
        yield return _deckManager.MoveFromDeckRoutine(cardObj);
    }

    public IEnumerator MoveFromHandRoutine(CardObject cardObj)
    {
        cardObj.ManaLayout.Canvas.sortingOrder = 100;
        cardObj.HoverPreviewHandler.PreviewEnabled = false;

        yield return _handManager.MoveFromHandRoutine(cardObj);
    }

    //public IEnumerator MoveFromShieldsRoutine(int shieldIndex)
    //{
    //    yield return _shieldsManager.BreakShieldRoutine(shieldIndex);
    //}

    //public IEnumerator MoveFromGraveyard(CardObject cardObj)
    //{

    //}

    public IEnumerator MoveFromManaZoneRoutine(CardObject cardObj)
    {
        cardObj.CardLayout.Canvas.sortingOrder = 100;
        cardObj.HoverPreviewHandler.PreviewEnabled = false;
        cardObj.IsVisible = true;

        yield return _manaZoneManager.MoveFromManaZoneRoutine(cardObj);
    }

    public IEnumerator MoveFromBattleZoneRoutine(CardObject cardObj)
    {
        cardObj.CardLayout.Canvas.sortingOrder = 100;
        cardObj.HoverPreviewHandler.PreviewEnabled = false;
        cardObj.IsVisible = true;

        yield return _battleZoneManager.MoveFromBattleZoneRoutine(cardObj);
    }

    #endregion

    #region To Zone Moves

    //public IEnumerator MoveToDeck(CardObject cardObj)
    //{

    //}

    public IEnumerator MoveToHandRoutine(CardObject cardObj)
    {
        cardObj.ActivateCardLayout();
        yield return _handManager.MoveToHandRoutine(cardObj);

        cardObj.HoverPreviewHandler.PreviewEnabled = true;
    }

    public IEnumerator MoveToShieldsRoutine(CardObject cardObj)
    {
        yield return StartCoroutine(_shieldsManager.MakeShieldRoutine(cardObj));
    }

    public Coroutine MoveToGraveyard(CardObject cardObj)
    {
        return StartCoroutine(MoveToGraveyardRoutine(cardObj));
    }

    private IEnumerator MoveToGraveyardRoutine(CardObject cardObj)
    {
        cardObj.gameObject.SetActive(false);
        cardObj.ActivateCardLayout();

        PlayerManager manager = GameManager.Instance.GetManager(_isPlayer);
        manager.GraveyardManager.AddCard(cardObj);
        switch (cardObj.CardInst.CurrentZone)
        {
            case CardZoneType.BattleZone:
                GameDataHandler.Instance.GetDataHandler(_isPlayer).CardsInBattle.Remove(cardObj.transform.GetInstanceID());
                GameManager.Instance.GetManager(_isPlayer).BattleZoneManager.ArrangeCards();
                break;
        }

        cardObj.gameObject.SetActive(true);
        yield break;
    }

    public IEnumerator MoveToManaZoneRoutine(CardObject cardObj)
    {
        cardObj.ActivateManaLayout();
        yield return _manaZoneManager.MoveToManaZoneRoutine(cardObj);

        cardObj.HoverPreviewHandler.PreviewEnabled = true;
    }

    #endregion
}
