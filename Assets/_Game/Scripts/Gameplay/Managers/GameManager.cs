using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _playerGoesFirst = true;

    [Header("UI")]
    [SerializeField] private ActionMenu _actionMenu;

    [Header("Player Managers")]
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private Deck _playerDeck;
    [SerializeField] private PlayerManager _opponentManager;
    [SerializeField] private Deck _opponentDeck;

    private static GameStep _currentStep = null;
    private Dictionary<GameStepType, GameStep> _gameSteps = new Dictionary<GameStepType, GameStep>();

    private bool _endCurrentStep = false;
    private bool _firstTurn = true;
    private bool _playerTurn = true;
    private bool _gameOver = false;

    #region Static Data Members

    private static Dictionary<int, CardManager> _playerCards = new Dictionary<int, CardManager>();
    private static Dictionary<int, CardManager> _opponentCards = new Dictionary<int, CardManager>();

    public static IReadOnlyDictionary<int, CardManager> PlayerCards
    {
        get { return _playerCards; }
    }

    public static IReadOnlyDictionary<int, CardManager> OpponentCards
    {
        get { return _opponentCards; }
    }

    #endregion

    public static GameStepType CurrentStep
    {
        get { return _currentStep.StepType; }
    }

    public bool FirstTurn
    {
        get { return _firstTurn; }
    }
    
    private void Start()
    {
        SetupSteps();

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Time.timeScale = 0.25f;

        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(GameLoopRoutine(_playerDeck, _opponentDeck));

        if (Input.GetKeyDown(KeyCode.Alpha1))
            _currentStep = _gameSteps[GameStepType.ChargeStep];

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _currentStep = _gameSteps[GameStepType.MainStep];
    }

    #region Game Loop Handling

    private IEnumerator GameLoopRoutine(Deck playerDeck, Deck opponentDeck)
    {
        _currentStep = _gameSteps[GameStepType.BeginStep];

        yield return GameStartRoutine(playerDeck, opponentDeck);

        PlayerManager currentManager;
        _playerTurn = _playerGoesFirst;
        currentManager = _playerTurn ? _playerManager : _opponentManager;
        
        while (!_gameOver)
        {
            yield return ProcessGameLoop(currentManager);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator GameStartRoutine(Deck playerDeck, Deck opponentDeck)
    {
        _playerManager.Initialize(playerDeck, ProcessGameAction);
        _opponentManager.Initialize(opponentDeck, ProcessGameAction);

        SetupCardDatabase(_playerManager.DataHandler.CardsInDeck, ref _playerCards);
        SetupCardDatabase(_opponentManager.DataHandler.CardsInDeck, ref _opponentCards);

        _playerManager.SetupShields();
        yield return _opponentManager.SetupShields();

        StartCoroutine(DrawStartingHandRoutine(_playerManager));
        yield return DrawStartingHandRoutine(_opponentManager);

        EnableHandInteraction(_playerManager.DataHandler);
        EnableHandInteraction(_opponentManager.DataHandler);

        _playerManager.CanSelect = true;
        _opponentManager.CanSelect = true;

        #region Internal Functions

        void SetupCardDatabase(List<CardManager> cards, ref Dictionary<int, CardManager> database)
        {
            foreach (CardManager card in cards)
            {
                database.Add(card.transform.GetInstanceID(), card);
            }
        }

        IEnumerator DrawStartingHandRoutine(PlayerManager playerManager)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return playerManager.DrawCardRoutine(true);
            }
        }

        void EnableHandInteraction(PlayerDataHandler playerData)
        {
            foreach (KeyValuePair<int, CardManager> pair in playerData.CardsInHand)
            {
                CardManager card = pair.Value;
                card.HoverPreviewHandler.PreviewEnabled = true;
                card.CanDrag = true;
            }
        }

        #endregion
    }

    private IEnumerator ProcessGameLoop(PlayerManager manager)
    {
        if (!_currentStep.UpdateStep || _endCurrentStep)
        {
            GameStep nextStep = _gameSteps[_currentStep.NextStepType];
            yield return _currentStep.FinishStepRoutine(manager);
            _currentStep = nextStep;
            yield return _currentStep.StartStepRoutine(manager);

            if (_endCurrentStep)
                _endCurrentStep = false;
        }
    }
    
    private void ProcessGameAction(CardManager card)
    {
        if (_playerTurn)
        {
            if (_playerCards.ContainsValue(card))
            {
                StartCoroutine(_currentStep.ProcessGameAction(card, _playerManager));
            }
        }
        else if ( _opponentCards.ContainsValue(card))
        {
            StartCoroutine(_currentStep.ProcessGameAction(card, _opponentManager));
        }
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
}
