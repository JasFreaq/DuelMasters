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

    private PlayerDataHandler _playerDataHandler;

    private List<CardObject> _playableCards = new List<CardObject>();

    public PlayerDataHandler DataHandler
    {
        get { return _playerDataHandler; }
    }

    public DeckManager DeckManager
    {
        get { return _deckManager; }
    }

    public ManaZoneManager ManaZoneManager
    {
        get { return _manaZoneManager; }
    }

    public List<CardObject> PlayableCards
    {
        get { return _playableCards; }
    }
    
    private void Awake()
    {
        _playerDataHandler = GetComponent<PlayerDataHandler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(DrawCardRoutine());

        if (_handManager.transform.GetChild(0).childCount > 0)
        {
            CardObject card = _playerDataHandler.CardsInHand[_handManager.transform.GetChild(0).GetChild(0).GetInstanceID()];
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
    
    public int SetPlayableCards()
    {
        _playableCards.Clear();

        foreach (CardObject cardObj in _playerDataHandler.CardsInHand.Values)
        {
            if (_playerDataHandler.CanPayCost(cardObj.CardData.Civilization, cardObj.CardData.Cost))
            {
                if (cardObj is CreatureObject creatureObj && creatureObj.IsEvolutionCreature)
                {
                    CardParams.Race[] races = creatureObj.CardData.Race;
                    if (_playerDataHandler.CheckCreaturesInBattle(races))
                    {
                        if (_isPlayer)
                            cardObj.SetHighlight(true);
                        _playableCards.Add(cardObj);
                    }
                }
                else
                {
                    if (_isPlayer)
                        cardObj.SetHighlight(true);
                    _playableCards.Add(cardObj);
                }
            }
        }

        return _playableCards.Count;
    }

    #endregion

    #region Frequent Region Movements

    public IEnumerator DrawCardRoutine()
    {
        CardObject cardObj = _deckManager.RemoveTopCard();
        yield return MoveFromDeckRoutine(cardObj);
        yield return MoveToHandRoutine(cardObj);
    }

    public IEnumerator ChargeManaRoutine(CardObject cardObj)
    {
        yield return MoveFromHandRoutine(cardObj);
        yield return MoveToManaZoneRoutine(cardObj);
    }

    public IEnumerator PlayCardRoutine(CardObject cardObj)
    {
        HoverPreviewHandler.FlipPreviewLocation = true;
        yield return MoveFromHandRoutine(cardObj);

        if (cardObj is CreatureObject creatureCard)
            yield return SummonCreatureRoutine(creatureCard);
        else if (cardObj is SpellObject spellCard)
            yield return CastSpellRoutine(spellCard);

        HoverPreviewHandler.FlipPreviewLocation = false;
        GameDataHandler.Instance.CheckWhileConditions();
    }

    private IEnumerator SummonCreatureRoutine(CreatureObject creatureObj)
    {
        if (creatureObj.IsEvolutionCreature)
        {
            Controller controller = GameManager.Instance.GetController(_isPlayer);

            Coroutine<CreatureObject> routine =
                controller.StartCoroutine<CreatureObject>(controller.ProcessEvolvingRoutine(creatureObj.CardData.Race));
            yield return routine.coroutine;
            CreatureObject underEvolvingCard = routine.returnVal;

            if (underEvolvingCard)
            {
                creatureObj.ActivateBattleLayout();
                yield return _battleZoneManager.MoveToBattleZoneRoutine(creatureObj, underEvolvingCard);
                creatureObj.HoverPreviewHandler.PreviewEnabled = true;
                creatureObj.CardInst.InstanceEffectHandler.TriggerWhenPutIntoBattle();
            }
            else
                yield return MoveToHandRoutine(creatureObj);
        }
        else
        {
            creatureObj.ActivateBattleLayout();
            yield return _battleZoneManager.MoveToBattleZoneRoutine(creatureObj);
            creatureObj.HoverPreviewHandler.PreviewEnabled = true;
            creatureObj.CardInst.InstanceEffectHandler.TriggerWhenPutIntoBattle();
        }
    }

    private IEnumerator CastSpellRoutine(SpellObject spellObj)
    {
        spellObj.CardInst.InstanceEffectHandler.TriggerWhenPlayed();
        yield return MoveToGraveyardRoutine(spellObj);
    }
    
    public IEnumerator BreakShieldRoutine(CardObject cardObj)
    {
        yield return _shieldsManager.BreakShieldRoutine(cardObj);
        yield return MoveToHandRoutine(cardObj);
    }
    
    public IEnumerator BreakShieldRoutine(ShieldObject shieldObj)
    {
        yield return _shieldsManager.BreakShieldRoutine(shieldObj);
        yield return MoveToHandRoutine(shieldObj.CardObj);
    }

    #endregion

    #region From Zone Moves
    
    public IEnumerator MoveFromDeckRoutine(CardObject cardObj)
    {
        InitiateMove(cardObj);
        cardObj.CardLayout.Canvas.gameObject.SetActive(true);

        yield return _deckManager.MoveFromDeckRoutine(cardObj);
    }

    public IEnumerator MoveFromHandRoutine(CardObject cardObj)
    {
        InitiateMove(cardObj);

        yield return _handManager.MoveFromHandRoutine(cardObj);
    }

    public IEnumerator MoveFromShieldsRoutine(CardObject cardObj)
    {
        InitiateMove(cardObj);

        yield return _shieldsManager.BreakShieldRoutine(cardObj);
    }
    
    public IEnumerator MoveFromShieldsRoutine(ShieldObject shieldObj)
    {
        InitiateMove(shieldObj.CardObj);

        yield return _shieldsManager.BreakShieldRoutine(shieldObj);
    }

    public IEnumerator MoveFromGraveyard(CardObject cardObj)
    {
        _graveyardManager.RemoveCard(cardObj);
        yield break;
    }

    public IEnumerator MoveFromManaZoneRoutine(CardObject cardObj)
    {
        InitiateMove(cardObj);
        cardObj.IsVisible = true;

        yield return _manaZoneManager.MoveFromManaZoneRoutine(cardObj);
    }

    public IEnumerator MoveFromBattleZoneRoutine(CardObject cardObj)
    {
        InitiateMove(cardObj);
        cardObj.IsVisible = true;

        yield return _battleZoneManager.MoveFromBattleZoneRoutine(cardObj);
    }

    private void InitiateMove(CardObject cardObj)
    {
        cardObj.CardLayout.Canvas.sortingOrder = 100;
        cardObj.HoverPreviewHandler.PreviewEnabled = false;
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
        if (!cardObj.CardInst.InstanceEffectHandler.TriggerWhenWouldBeDestroyed()) 
            return StartCoroutine(MoveToGraveyardRoutine(cardObj));

        return null;
    }

    private IEnumerator MoveToGraveyardRoutine(CardObject cardObj)
    {
        switch (cardObj.CardInst.CurrentZone)
        {
            case CardZoneType.Deck:
                yield return MoveFromDeckRoutine(cardObj);
                break;

            case CardZoneType.Hand:
                yield return MoveFromHandRoutine(cardObj);
                break;

            case CardZoneType.Shields:
                yield return MoveFromShieldsRoutine(cardObj);
                break;

            case CardZoneType.ManaZone:
                yield return MoveFromManaZoneRoutine(cardObj);
                break;

            case CardZoneType.BattleZone:
                yield return MoveFromBattleZoneRoutine(cardObj);
                break;
        }

        cardObj.gameObject.SetActive(false);
        cardObj.ActivateCardLayout();
        _graveyardManager.AddCard(cardObj);
        cardObj.gameObject.SetActive(true);

        cardObj.CardInst.InstanceEffectHandler.TriggerWhenDestroyed();
    }

    public IEnumerator MoveToManaZoneRoutine(CardObject cardObj)
    {
        cardObj.ActivateManaLayout();
        yield return _manaZoneManager.MoveToManaZoneRoutine(cardObj);

        cardObj.HoverPreviewHandler.PreviewEnabled = true;
    }

    public IEnumerator MoveToBattleZoneRoutine(CardObject cardObj)
    {
        yield return SummonCreatureRoutine((CreatureObject) cardObj);
    }

    #endregion
}
