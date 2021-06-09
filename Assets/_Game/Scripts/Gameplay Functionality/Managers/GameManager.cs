using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameParams;

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

    private static GameStep _currentStep = GameStep.BeginStep;
    private bool _firstTurn = true;
    private bool _playerTurn = true;
    private bool _gameOver = false;

    private List<CardManager> _playerCards;
    private PlayerDataHandler _playerDataHandler;
    private List<CardManager> _opponentCards;
    private PlayerDataHandler _opponentDataHandler;

    public GameStep CurrentStep
    {
        get { return _currentStep; }
    }

    private void Awake()
    {
        _playerDataHandler = _playerManager.GetComponent<PlayerDataHandler>();
        _opponentDataHandler = _opponentManager.GetComponent<PlayerDataHandler>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(GameLoopRoutine(_playerDeck, _opponentDeck));

        if (Input.GetKeyDown(KeyCode.Alpha1))
            _currentStep = GameStep.ChargeStep;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _currentStep = GameStep.MainStep;
    }

    private IEnumerator GameLoopRoutine(Deck playerDeck, Deck opponentDeck)
    {
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
        _playerManager.Initialize(playerDeck, ProcessGameActionHandler);
        _opponentManager.Initialize(opponentDeck, ProcessGameActionHandler);

        _playerCards = new List<CardManager>(_playerDataHandler.CardsInDeck);
        _opponentCards = new List<CardManager>(_opponentDataHandler.CardsInDeck);

        _playerManager.SetupShields();
        yield return _opponentManager.SetupShields();

        StartCoroutine(DrawStartingHandRoutine(_playerManager));
        yield return DrawStartingHandRoutine(_opponentManager);

        EnableHandInteraction(_playerDataHandler);
        EnableHandInteraction(_opponentDataHandler);

        _playerManager.CanSelect = true;
        _opponentManager.CanSelect = true;

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
                card.DragHandler.CanDrag = true;
            }
        }
    }

    private IEnumerator ProcessGameLoop(PlayerManager manager)
    {
        switch (_currentStep)
        {
            case GameStep.BeginStep:
                _currentStep = GameStep.UntapStep;
                break;

            case GameStep.UntapStep:
                foreach (CardManager card in _playerDataHandler.TappedCards)
                {
                    card.ToggleTap();
                }
                _playerDataHandler.TappedCards.Clear();
                _currentStep = GameStep.StartOfTurnStep;
                break;

            case GameStep.StartOfTurnStep:
                _currentStep = GameStep.DrawStep;
                break;

            case GameStep.DrawStep:
                if (!_firstTurn)
                    yield return manager.DrawCardRoutine();
                _currentStep = GameStep.ChargeStep;
                break;

            case GameStep.ChargeStep:

                break;

            case GameStep.MainStep:
                break;

            case GameStep.AttackStep:
                break;

            case GameStep.EndStep:
                if (_firstTurn)
                    _firstTurn = false;
                _playerTurn = !_playerTurn;
                _currentStep = GameStep.BeginStep;
                break;
        }
    }

    private void ProcessGameActionHandler(CardManager card)
    {
        if (_playerTurn)
        {
            if (_playerCards.Contains(card))
            {
                StartCoroutine(ProcessGameAction(card, _playerManager, _playerDataHandler));
            }
        }
        else if ( _opponentCards.Contains(card))
        {
            StartCoroutine(ProcessGameAction(card, _opponentManager, _opponentDataHandler));
        }
    }

    private IEnumerator ProcessGameAction(CardManager card, PlayerManager manager, PlayerDataHandler dataHandler)
    {
        if (!card.DragHandler.IsReturningToPosition) 
        {
            switch (_currentStep)
            {
                case GameStep.ChargeStep:
                    yield return manager.ChargeManaRoutine(card);
                    if (card.CardData.Civilization.Length > 1)
                    {
                        card.ToggleTap();
                        dataHandler.TappedCards.Add(card);
                    }

                    _currentStep = GameStep.MainStep;
                    break;

                case GameStep.MainStep:
                    Card cardData = card.CardData;
                    if (dataHandler.CanPayCost(cardData.Civilization, cardData.Cost))
                    {
                        dataHandler.PayCost(cardData.Civilization, cardData.Cost);
                        manager.ManaZoneManager.ArrangeCards();
                        yield return manager.PlayCardRoutine(card);
                    }
                    else
                    {
                        card.DragHandler.ReturnToPosition();
                    }

                    break;
            }
        }
    }
}
