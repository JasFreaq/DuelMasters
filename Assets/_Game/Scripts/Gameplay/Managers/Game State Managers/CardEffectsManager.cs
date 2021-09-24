using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectsManager : MonoBehaviour
{
    #region Helper Data Structures

    public struct CardSelectionData
    {
        public CardZoneType fromZone;
        public bool fromBothPlayers, searchAndShuffleDeck, showSearchedCard;

        public CountChoiceType countChoice;
        public int moveCount;

        public CardSelectionData(MovementZones movementZones)
        {
            fromZone = movementZones.fromZone;
            
            fromBothPlayers = movementZones.fromBothPlayers;
            searchAndShuffleDeck = movementZones.deckCardMove != DeckCardMoveType.Top;
            showSearchedCard = movementZones.showSearchedCard;

            countChoice = movementZones.countChoice;
            moveCount = movementZones.moveCount;
        }

        public CardSelectionData(EffectFunctionality functionality)
        {
            fromZone = functionality.TargetingParameter.ZoneType;

            fromBothPlayers = functionality.TargetPlayer == PlayerTargetType.Both;
            searchAndShuffleDeck = false;
            showSearchedCard = false;

            countChoice = functionality.TargetingParameter.CountChoice;
            moveCount = functionality.TargetingParameter.Count;
        }
    }

    #endregion

    [SerializeField] private CardBrowserOverlay _cardBrowserOverlay;

    private Action _onEndOfTurnAction;

    #region Static Data Members

    private static CardEffectsManager _Instance = null;

    public static CardEffectsManager Instance
    {
        get
        {
            if (!_Instance)
                _Instance = FindObjectOfType<CardEffectsManager>();
            return _Instance;
        }
    }

    #endregion

    private void Awake()
    {
        int count = FindObjectsOfType<CardEffectsManager>().Length;
        if (count > 1)
            Destroy(gameObject);
        else
            _Instance = this;
    }

    private void OnEnable()
    {
        _cardBrowserOverlay.RegisterOnToggleTab(EnablePlayerControllerInteract);
    }

    private void OnDisable()
    {
        _cardBrowserOverlay.DeregisterOnToggleTab(EnablePlayerControllerInteract);
    }

    public Coroutine ProcessRegionMovement(bool playerChooses, bool affectPlayer, MovementZones movementZones, EffectTargetingCondition targetingCondition, bool mayUse = false)
    {
        if (mayUse)
            return StartCoroutine(MayProcessRegionMovementRoutine(playerChooses, affectPlayer, movementZones,
                targetingCondition));

        return StartCoroutine(AdjustMovementTargetRoutine(playerChooses, affectPlayer, movementZones,
                targetingCondition, false));
    }

    public Coroutine ProcessRegionMovement(CardBehaviour card, MovementZones movementZones)
    {
        return StartCoroutine(ProcessRegionMovementRoutine(card, movementZones));
    }
    
    public IEnumerator ProcessCardSelectionRoutine(bool playerChooses, bool affectPlayer, CardSelectionData selectionData, 
        EffectTargetingCondition targetingCondition, bool mayUse)
    {
        Controller choosingController = GameManager.Instance.GetController(playerChooses);
        PlayerController playerController = choosingController as PlayerController;

        PlayerManager affectedPlayer = GameManager.Instance.GetManager(affectPlayer);

        int lower = 1, upper = selectionData.moveCount;
        if (selectionData.countChoice == CountChoiceType.Exactly)
            lower = upper;

        List<CardBehaviour> selectedCards = null;

        switch (selectionData.fromZone)
        {
            case CardZoneType.Deck:
                if (selectionData.searchAndShuffleDeck)
                {
                    if (playerChooses)
                    {
                        playerController.EnableFullControl(false);
                        Coroutine<List<CardBehaviour>> routine2 =
                            _cardBrowserOverlay.StartCoroutine<List<CardBehaviour>>(_cardBrowserOverlay.CardSelectionRoutine(lower, upper,
                                affectedPlayer.DataHandler.CardsInDeck, targetingCondition, mayUse));
                        yield return routine2.coroutine;
                        selectedCards = routine2.returnVal;
                        playerController.EnableFullControl(true);
                    }
                    else
                    {
                        Coroutine<List<CardBehaviour>> routine2 =
                            choosingController.StartCoroutine<List<CardBehaviour>>(choosingController.SelectCardsRoutine(lower, upper, true,
                                affectedPlayer.DataHandler.CardsInDeck, null, mayUse));
                        yield return routine2.coroutine;
                        selectedCards = routine2.returnVal;
                    }

                    //TODO: Shuffle Deck
                }
                break;

            case CardZoneType.Hand:
                if (playerChooses && !affectPlayer)
                {
                    playerController.EnableFullControl(false);
                    Coroutine<List<CardBehaviour>> routine =
                        _cardBrowserOverlay.StartCoroutine<List<CardBehaviour>>(_cardBrowserOverlay.CardSelectionRoutine(lower, upper,
                            affectedPlayer.DataHandler.CardsInHand, targetingCondition, mayUse));
                    yield return routine.coroutine;
                    selectedCards = routine.returnVal;
                    playerController.EnableFullControl(true);
                }
                else
                {
                    Coroutine<List<CardBehaviour>> routine =
                        choosingController.StartCoroutine<List<CardBehaviour>>(choosingController.SelectCardsRoutine(lower, upper, true,
                            affectedPlayer.DataHandler.CardsInHandList, targetingCondition, mayUse));
                    yield return routine.coroutine;
                    selectedCards = routine.returnVal;
                }
                break;

            case CardZoneType.Shields:
                Coroutine<List<CardBehaviour>> routine3 =
                    choosingController.StartCoroutine<List<CardBehaviour>>(choosingController.SelectCardsRoutine(lower, upper, false,
                        affectedPlayer.DataHandler.Shields, mayUse));
                yield return routine3.coroutine;
                selectedCards = routine3.returnVal;
                break;

            case CardZoneType.Graveyard:
                if (playerChooses)
                {
                    playerController.EnableFullControl(false);
                    Coroutine<List<CardBehaviour>> routine4 =
                        _cardBrowserOverlay.StartCoroutine<List<CardBehaviour>>(_cardBrowserOverlay.CardSelectionRoutine(lower, upper,
                                affectedPlayer.DataHandler.CardsInGrave, targetingCondition, mayUse));
                    yield return routine4.coroutine;
                    selectedCards = routine4.returnVal;
                    playerController.EnableFullControl(true);
                }
                else
                {
                    Coroutine<List<CardBehaviour>> routine4 =
                        choosingController.StartCoroutine<List<CardBehaviour>>(choosingController.SelectCardsRoutine(lower, upper, true,
                            affectedPlayer.DataHandler.CardsInGrave, null, mayUse));
                    yield return routine4.coroutine;
                    selectedCards = routine4.returnVal;
                }
                break;

            case CardZoneType.ManaZone:
                Coroutine<List<CardBehaviour>> routine5 =
                    choosingController.StartCoroutine<List<CardBehaviour>>(choosingController.SelectCardsRoutine(lower, upper, true,
                        affectedPlayer.DataHandler.CardsInManaList, targetingCondition, mayUse));
                yield return routine5.coroutine;
                selectedCards = routine5.returnVal;
                break;

            case CardZoneType.BattleZone:
                Coroutine<List<CardBehaviour>> routine6;
                if (selectionData.fromBothPlayers)
                {
                    routine6 = choosingController.StartCoroutine<List<CardBehaviour>>(choosingController.SelectCardsRoutine(lower, upper, true,
                        GameManager.Instance.GetManager(true).DataHandler.CardsInBattle,
                        GameManager.Instance.GetManager(false).DataHandler.CardsInBattle,
                        targetingCondition, mayUse));
                }
                else
                {
                    routine6 = choosingController.StartCoroutine<List<CardBehaviour>>(choosingController.SelectCardsRoutine(lower, upper, true,
                            affectedPlayer.DataHandler.CardsInBattle, targetingCondition, mayUse));
                }

                yield return routine6.coroutine;
                selectedCards = routine6.returnVal;
                break;
        }

        yield return selectedCards;
    }

    private IEnumerator MayProcessRegionMovementRoutine(bool playerChooses, bool affectPlayer, MovementZones movementZones, EffectTargetingCondition targetingCondition)
    {
        Controller choosingPlayer = GameManager.Instance.GetController(playerChooses);
        Coroutine<bool> routine = choosingPlayer.StartCoroutine<bool>(choosingPlayer.ChooseEffectActivationRoutine());
        yield return routine.coroutine;
        if (routine.returnVal)
            yield return AdjustMovementTargetRoutine(playerChooses, affectPlayer, movementZones, targetingCondition, true);
    }

    private IEnumerator AdjustMovementTargetRoutine(bool playerChooses, bool affectPlayer, MovementZones movementZones, EffectTargetingCondition targetingCondition, bool mayUse)
    {
        Controller choosingController = GameManager.Instance.GetController(playerChooses);
        PlayerManager affectedPlayer = GameManager.Instance.GetManager(affectPlayer);
        
        if (movementZones.fromZone == CardZoneType.Deck && movementZones.deckCardMove == DeckCardMoveType.Top)
        {
            int lower = 1, upper = movementZones.moveCount;
            if (movementZones.countChoice == CountChoiceType.Exactly)
                lower = upper;

            int moveCount = 0;
            if (movementZones.countChoice == CountChoiceType.Exactly || movementZones.moveCount == 1)
                moveCount = movementZones.moveCount;
            else if (movementZones.countChoice == CountChoiceType.Upto)
            {
                Coroutine<int> routine1 = choosingController.StartCoroutine<int>(choosingController.GetNumberSelectionRoutine(lower, upper));
                yield return routine1.coroutine;
                moveCount = routine1.returnVal;
            }

            for (int i = 0; i < moveCount; i++)
            {
                CardObject cardObj = affectedPlayer.DeckManager.RemoveTopCard();
                yield return ProcessRegionMovementRoutine(cardObj, movementZones);
            }
        }
        else
        {
            Coroutine<List<CardBehaviour>> routine = this.StartCoroutine<List<CardBehaviour>>(ProcessCardSelectionRoutine(playerChooses, affectPlayer,
                    new CardSelectionData(movementZones), targetingCondition, mayUse));
            yield return routine.coroutine;
            List<CardBehaviour> selectedCards = routine.returnVal;

            if (selectedCards != null)
            {
                foreach (CardBehaviour card in selectedCards)
                    yield return ProcessRegionMovementRoutine(card, movementZones);
            }
        }
    }

    private IEnumerator ProcessRegionMovementRoutine(CardBehaviour card, MovementZones movementZones)
    {
        CardObject cardObj = card as CardObject;
        ShieldObject shieldObj = null;

        if (!cardObj)
        {
            shieldObj = card as ShieldObject;
            if (shieldObj)
                cardObj = shieldObj.CardObj;
        }

        PlayerManager owner = GameManager.Instance.GetManager(cardObj.IsPlayer);

        switch (movementZones.fromZone)
        {
            case CardZoneType.Deck:
                yield return owner.MoveFromDeckRoutine(cardObj);
                break;

            case CardZoneType.Hand:
                yield return owner.MoveFromHandRoutine(cardObj);
                break;

            case CardZoneType.Shields:
                yield return owner.MoveFromShieldsRoutine(shieldObj);
                break;

            case CardZoneType.Graveyard:
                yield return owner.MoveFromGraveyard(cardObj);
                break;

            case CardZoneType.ManaZone:
                yield return owner.MoveFromManaZoneRoutine(cardObj);
                break;

            case CardZoneType.BattleZone:
                yield return owner.MoveFromBattleZoneRoutine(cardObj);
                break;
        }

        switch (movementZones.toZone)
        {
            case CardZoneType.Deck:
                break;

            case CardZoneType.Hand:
                yield return owner.MoveToHandRoutine(cardObj);
                break;

            case CardZoneType.Shields:
                yield return owner.MoveToShieldsRoutine(cardObj);
                break;

            case CardZoneType.Graveyard:
                yield return owner.MoveToGraveyard(cardObj);
                break;

            case CardZoneType.ManaZone:
                yield return owner.MoveToManaZoneRoutine(cardObj);
                break;

            case CardZoneType.BattleZone:
                if (cardObj is CreatureObject creatureObj)
                    yield return owner.MoveToBattleZoneRoutine(creatureObj);
                else
                    Debug.LogError($"{card} selected to MoveToBattleZone but it is not a creature");
                break;
        }

        GameDataHandler.Instance.CheckWhileConditions();
    }

    private void EnablePlayerControllerInteract(bool enable)
    {
        PlayerController playerController = GameManager.Instance.GetController(true) as PlayerController;
        playerController.CanInteract = enable;
    }

    #region Register Callbacks

    public void RegisterOnEndOfTurn(Action action)
    {
        _onEndOfTurnAction += action;
    }
    
    public void DeregisterOnEndOfTurn(Action action)
    {
        _onEndOfTurnAction -= action;
    }

    #endregion
}
