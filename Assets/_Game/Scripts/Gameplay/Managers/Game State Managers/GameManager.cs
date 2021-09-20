using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private ActionButtonOverlay _actionOverlay;

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
        Test();
        
        if (_gameBegun && !_gameOver)
        {
            _gameLoopRoutine ??= StartCoroutine(ProcessGameLoopRoutine());
        }

        void Test()
        {
            if (Input.GetKeyDown(KeyCode.T))
                Time.timeScale = 7.5f;
            
            if (Input.GetKeyDown(KeyCode.Y))
                Time.timeScale = 1f;

            if (Input.GetKeyDown(KeyCode.Return))
                StartCoroutine(StartGameRoutine(_playerDeck, _opponentDeck));

            if (Input.GetKeyDown(KeyCode.Alpha1))
                _currentStep = _gameSteps[GameStepType.ChargeStep];

            if (Input.GetKeyDown(KeyCode.Alpha2))
                _currentStep = _gameSteps[GameStepType.MainStep];
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
                _currentStep = _gameSteps[GameStepType.AttackStep];

            if (Input.GetKeyDown(KeyCode.Alpha0))
                print(_currentStep);

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                MovementZones movementZones = new MovementZones
                {
                    fromZone = CardZoneType.Hand,
                    toZone = CardZoneType.Shields,
                    deckCardMove = DeckCardMoveType.SearchShuffle,
                    countChoice = CountChoiceType.Exactly,
                    moveCount = 3
                };

                //EffectTargetingCondition targetingCondition = new EffectTargetingCondition();
                //targetingCondition.AddPowerCondition(new PowerCondition
                //{
                //    comparator = ComparisonType.GreaterThan,
                //    power = 1
                //});

                CardEffectsManager.Instance.ProcessRegionMovement(true, true, movementZones, null);
            }
            
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                MovementZones movementZones = new MovementZones
                {
                    fromZone = CardZoneType.Shields,
                    toZone = CardZoneType.ManaZone,
                    deckCardMove = DeckCardMoveType.SearchShuffle,
                    countChoice = CountChoiceType.Upto,
                    moveCount = 3
                };

                //EffectTargetingCondition targetingCondition = new EffectTargetingCondition();
                //targetingCondition.AssignedCardTypeCondition = true;
                //targetingCondition.CardTypeCondition = CardParams.CardType.Creature;

                CardEffectsManager.Instance.ProcessRegionMovement(true, true, movementZones, null);
            }
        }
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
            GameDataHandler.Instance.CheckWhileConditions();

            _currentStep = nextStep;
            yield return _currentStep.StartStepRoutine(_currentManager);
            GameDataHandler.Instance.CheckWhileConditions();

            if (_endCurrentStep)
            {
                _endCurrentStep = false;
                GameDataHandler.Instance.CheckWhileConditions();
            }
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

    #endregion
    
    #region Game Loop Parameter(s) Alterers

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
    
    public Controller GetController(bool isPlayer)
    {
        if (isPlayer)
            return _playerController;

        return _opponentController;
    }
}
