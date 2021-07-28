using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _playerGoesFirst = true;
    
    [Header("Controllers")] 
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private AIController _opponentController;
    [Header("Player Managers")] 
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerManager _opponentManager;
    [Header("Decks")] 
    [SerializeField] private Deck _playerDeck;
    [SerializeField] private Deck _opponentDeck;

    [Header("UI")]
    [SerializeField] private ActionButtonOverlay actionButton;
    [SerializeField] private NumberSelector _numberSelector;
    [SerializeField] private CardSelectionOverlay _cardSelectionOverlay;

    private PlayerManager _currentManager;
    private Coroutine _gameLoopRoutine = null;

    private GameStep _currentStep = null;
    private Dictionary<GameStepType, GameStep> _gameSteps = new Dictionary<GameStepType, GameStep>();

    private bool _gameBegun = false;
    private bool _endCurrentStep = false;
    private bool _firstTurn = true;
    private bool _playerTurn = true;
    private bool _gameOver = false;

    public GameStepType CurrentStep
    {
        get { return _currentStep.StepType; }
    }

    public bool FirstTurn
    {
        get { return _firstTurn; }
    }

    #region Static Data Members

    private static GameManager _Instance = null;

    public static GameManager Instance
    {
        get
        {
            if (!_Instance)
                _Instance = FindObjectOfType<GameManager>();
            return _Instance;
        }
    }

    #endregion

    private void Awake()
    {
        int count = FindObjectsOfType<GameManager>().Length;
        if (count > 1)
            Destroy(gameObject);
        else
            _Instance = this;
    }

    private void OnEnable()
    {
        _cardSelectionOverlay.RegisterOnToggleTab(EnablePlayerControllerInteract);
    }

    private void Start()
    {
        SetupSteps();

        #region Local Functions

        void SetupSteps()
        {
            _gameSteps[GameStepType.BeginStep] = new BeginStep(this);
            _gameSteps[GameStepType.UntapStep] = new UntapStep(this);
            _gameSteps[GameStepType.StartOfTurnStep] = new StartOfTurnStep(this);
            _gameSteps[GameStepType.DrawStep] = new DrawStep(this);
            _gameSteps[GameStepType.ChargeStep] = new ChargeStep(this);
            _gameSteps[GameStepType.MainStep] = new MainStep(this);
            _gameSteps[GameStepType.AttackStep] = new AttackStep(this);
            _gameSteps[GameStepType.EndStep] = new EndStep(this);
        }

        #endregion
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Time.timeScale = 0.25f;

        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(StartGameRoutine(_playerDeck, _opponentDeck));

        if (Input.GetKeyDown(KeyCode.Alpha1))
            _currentStep = _gameSteps[GameStepType.ChargeStep];

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _currentStep = _gameSteps[GameStepType.MainStep];
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
            print(_currentStep);

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            MovementZones movementZones = new MovementZones
            {
                fromZone = CardZoneType.Deck,
                toZone = CardZoneType.Hand,
                deckCardMove = DeckCardMoveType.SearchShuffle,
                countChoice = CountChoiceType.Upto,
                moveCount = 4
            };
            ProcessRegionMovement(true, movementZones);
        }

        if (_gameBegun && !_gameOver)
        {
            if (_gameLoopRoutine == null)
                _gameLoopRoutine = StartCoroutine(ProcessGameLoopRoutine());
        }
    }

    private void OnDisable()
    {
        _cardSelectionOverlay.DeregisterOnToggleTab(EnablePlayerControllerInteract);
    }

    #region Game Loop Handling
    
    public IEnumerator StartGameRoutine(Deck playerDeck, Deck opponentDeck)
    {
        _currentStep = _gameSteps[GameStepType.BeginStep];

        _playerManager.GenerateDeck(GameDataHandler.Instance.GenerateCardInstances(playerDeck), ProcessDragAction);
        _opponentManager.GenerateDeck(GameDataHandler.Instance.GenerateCardInstances(opponentDeck), ProcessDragAction);

        _playerManager.DataHandler.SetAllCards();
        _opponentManager.DataHandler.SetAllCards();

        _playerManager.SetupShields();
        yield return _opponentManager.SetupShields();

        StartCoroutine(DrawStartingHandRoutine(_playerManager));
        yield return DrawStartingHandRoutine(_opponentManager);

        _playerController.EnableFullControl(true);

        _playerTurn = _playerGoesFirst;
        _currentManager = _playerTurn ? _playerManager : _opponentManager;
        _gameBegun = true;

        #region Local Functions

        IEnumerator DrawStartingHandRoutine(PlayerManager playerManager)
        {
            for (int i = 0; i < GameParamsHolder.Instance.BaseCardCount; i++)
            {
                yield return playerManager.DrawCardRoutine();
            }
        }

        #endregion
    }

    private IEnumerator ProcessGameLoopRoutine()
    {
        if (!_currentStep.UpdateStep || _endCurrentStep)
        {
            GameStep nextStep = _gameSteps[_currentStep.NextStepType];
            yield return _currentStep.FinishStepRoutine(_currentManager);
            _currentStep = nextStep;
            yield return _currentStep.StartStepRoutine(_currentManager);

            if (_endCurrentStep)
                _endCurrentStep = false;
        }

        _gameLoopRoutine = null;
    }
    
    private void ProcessDragAction(CardObject card)
    {
        int iD = card.transform.GetInstanceID();
        if (_playerTurn)
        {
            if (_playerManager.DataHandler.AllCards.ContainsKey(iD))
            {
                StartCoroutine(_currentStep.ProcessGameAction(card, _playerManager));
            }
        }
        else if (_opponentManager.DataHandler.AllCards.ContainsKey(iD))
        {
            StartCoroutine(_currentStep.ProcessGameAction(card, _opponentManager));
        }
    }

    public void AttemptAttack(bool isPlayer, CardBehaviour target)
    {
        StartCoroutine(AttemptAttackRoutine(isPlayer, target));
    }

    private IEnumerator AttemptAttackRoutine(bool isPlayer, CardBehaviour target)
    {
        Controller controller, oppController;
        if (isPlayer)
        {
            controller = _playerController;
            oppController = _opponentController;
        }
        else
        {
            controller = _opponentController;
            oppController = _playerController;
        }

        if (controller.CurrentlySelected)
        {
            CardObject selectedCardObj = controller.CurrentlySelected;

            if (selectedCardObj.InZone(CardZoneType.BattleZone) && !selectedCardObj.CardInst.IsTapped)
            {
                TargetingLinesHandler.Instance.DisableLines();

                CreatureObject creatureObj = (CreatureObject) selectedCardObj;
                float attackTime = GameParamsHolder.Instance.AttackTime;
                Vector3 creaturePos = creatureObj.transform.position;
                
                if (target is CreatureObject)
                {
                    CreatureObject attackedCreatureObj = (CreatureObject) target;
                    if (attackedCreatureObj.CardInst.IsTapped)
                    {
                        creatureObj.transform.DOMove(target.transform.position, attackTime).SetEase(Ease.InCubic);
                        yield return new WaitForSeconds(attackTime);
                        controller.DeselectCurrentlySelected();

                        int attackingCraturePower = ((CreatureData) creatureObj.CardInst.CardData).Power;
                        int attackedCreaturePower = ((CreatureData) attackedCreatureObj.CardInst.CardData).Power;

                        if (attackingCraturePower > attackedCreaturePower)
                            yield return attackedCreatureObj.SendToGraveyard();
                        else if (attackingCraturePower == attackedCreaturePower)
                        {
                            yield return creatureObj.SendToGraveyard();
                            creatureObj = null;
                            yield return attackedCreatureObj.SendToGraveyard();
                        }
                        else
                        {
                            yield return creatureObj.SendToGraveyard();
                            creatureObj = null;
                        }

                        yield return ReturnCreature();
                    }
                }
                else if (target is ShieldObject)
                {
                    creatureObj.transform.DOMove(target.transform.position, attackTime).SetEase(Ease.InCubic);
                    yield return new WaitForSeconds(attackTime);
                    controller.DeselectCurrentlySelected();

                    PlayerManager opponent = GetManager(!isPlayer);
                    StartCoroutine(opponent.BreakShieldRoutine((ShieldObject) target));

                    yield return ReturnCreature();
                }

                #region Local Functions

                IEnumerator ReturnCreature()
                {
                    if (creatureObj)
                    {
                        creatureObj.transform.DOMove(creaturePos, attackTime).SetEase(Ease.InCubic);
                        yield return new WaitForSeconds(attackTime);
                        creatureObj.ToggleTapState();
                    }
                }

                #endregion
            }
        }
    }

    

    #endregion

    #region Card Effect Processing

    public Coroutine ProcessRegionMovement(bool affectPlayer, MovementZones movementZones)
    {
        return StartCoroutine(AdjustMovementTargetRoutine(affectPlayer, movementZones));
    }

    private IEnumerator AdjustMovementTargetRoutine(bool affectPlayer, MovementZones movementZones)
    {
        PlayerManager affectedPlayer = GetManager(affectPlayer);

        int moveCount = 0, lower = 1, upper = movementZones.moveCount;
        List<CardObject> selectedCards;

        switch (movementZones.fromZone)
        {
            case CardZoneType.Deck:

                switch (movementZones.deckCardMove)
                {
                    case DeckCardMoveType.Top:
                        if (movementZones.countChoice == CountChoiceType.Upto)
                        {
                            Coroutine<int> routine1 = _numberSelector.StartCoroutine<int>(_numberSelector.GetSelectionRoutine(lower, upper));
                            yield return routine1.coroutine;
                            moveCount = routine1.returnVal;
                        }
                        else if (movementZones.countChoice == CountChoiceType.Exactly)
                            moveCount = movementZones.moveCount;

                        for (int i = 0; i < moveCount; i++)
                        {
                            CardObject cardObj = affectedPlayer.DeckManager.RemoveTopCard();
                            yield return ProcessRegionMovementRoutine(cardObj, movementZones);
                        }

                        break;

                    case DeckCardMoveType.SearchShuffle:
                        Coroutine<List<CardObject>> routine2 = this.StartCoroutine<List<CardObject>>(CardSelectionRoutine(lower, upper, affectedPlayer.DataHandler.CardsInDeck));
                        yield return routine2.coroutine;
                        selectedCards = routine2.returnVal;

                        foreach (CardObject cardObj in selectedCards)
                            yield return ProcessRegionMovementRoutine(cardObj, movementZones);

                        break;
                }

                break;

            case CardZoneType.Hand:

                if (affectPlayer) 
                {
                    
                }
                else
                {
                    Coroutine<List<CardObject>> routine3 = this.StartCoroutine<List<CardObject>>(CardSelectionRoutine(lower, upper, affectedPlayer.DataHandler.CardsInHand));
                    yield return routine3.coroutine;
                    selectedCards = routine3.returnVal;

                    foreach (CardObject cardObj in selectedCards)
                        yield return ProcessRegionMovementRoutine(cardObj, movementZones);
                }

                break;
            
            case CardZoneType.Graveyard:

                Coroutine<List<CardObject>> routine4 = this.StartCoroutine<List<CardObject>>(CardSelectionRoutine(lower, upper, affectedPlayer.DataHandler.CardsInGrave));
                yield return routine4.coroutine;
                selectedCards = routine4.returnVal;

                foreach (CardObject cardObj in selectedCards)
                    yield return ProcessRegionMovementRoutine(cardObj, movementZones);

                break;
        }
    }

    private IEnumerator ProcessRegionMovementRoutine(CardObject cardObj, MovementZones movementZones)
    {
        PlayerManager owner = GetManager(cardObj.IsPlayer);

        switch (movementZones.fromZone)
        {
            case CardZoneType.Deck:
                yield return owner.MoveFromDeckRoutine(cardObj);
                break;

            case CardZoneType.Hand:
                yield return owner.MoveFromHandRoutine(cardObj);
                break;

            case CardZoneType.Shields:
                break;
            
            case CardZoneType.Graveyard:
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
                if (cardObj is CreatureObject)
                    yield return owner.MoveToBattleZoneRoutine(cardObj);
                else
                    Debug.LogError($"{cardObj} selected to MoveToBattleZone but it is not a creature");
                break;
        }
    }

    private IEnumerator CardSelectionRoutine(int lower, int upper, List<CardObject> cardList)
    {
        List<CardObject> validCards = new List<CardObject>();
        foreach (CardObject cardObj in cardList)
        {
            if (Random.Range(0f, 10f) < 3.5f)
            {
                cardObj.PreviewLayoutHandler.SetValidity(true);
                validCards.Add(cardObj);
            }
            else
                cardObj.PreviewLayoutHandler.SetValidity(false);
        }

        foreach (CardObject cardObj in validCards)
        {
            cardList.Remove(cardObj);
        }

        cardList.AddRange(validCards);

        _playerController.EnableFullControl(false);

        Coroutine<List<CardObject>> routine = _numberSelector.StartCoroutine<List<CardObject>>(_cardSelectionOverlay.GenerateLayoutRoutine(lower, upper, cardList));
        yield return routine.coroutine;

        _playerController.EnableFullControl(true);

        yield return routine.returnVal;
    }

    private IEnumerator CardSelectionRoutine(int lower, int upper, Dictionary<int, CardObject> cardDict)
    {
        List<CardObject> cardList = new List<CardObject>();
        foreach (KeyValuePair<int, CardObject> pair in cardDict)
        {
            CardObject cardObj = pair.Value;
            cardList.Add(cardObj);
        }

        Coroutine<List<CardObject>> routine = this.StartCoroutine<List<CardObject>>(CardSelectionRoutine(lower, upper, cardList));
        yield return routine.coroutine;
        yield return routine.returnVal;
    }

    #endregion

    #region Game Parameter(s) Alterers

    public void EndCurrentStep()
    {
        _endCurrentStep = true;
    }

    public void EndFirstTurn()
    {
        _firstTurn = false;
    }

    public void TogglePlayerTurn()
    {
        _playerTurn = !_playerTurn;
    }

    #endregion

    public PlayerManager GetManager(bool isPlayer)
    {
        return isPlayer ? _playerManager : _opponentManager;
    }

    private void EnablePlayerControllerInteract(bool enable)
    {
        _playerController.CanInteract = enable;
    }
}
