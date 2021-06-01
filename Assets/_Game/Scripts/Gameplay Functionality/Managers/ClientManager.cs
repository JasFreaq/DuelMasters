using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerManager _opponentManager;
    
    public IEnumerator GameStartRoutine(Action<CardManager> processAction, Deck playerDeck, Deck opponentDeck)
    {
        _playerManager.Initialize(playerDeck);
        _opponentManager.Initialize(opponentDeck);

        _playerManager.SetupShields();
        yield return _opponentManager.SetupShields();

        StartCoroutine(DrawStartingHandRoutine(_playerManager, processAction));
        yield return DrawStartingHandRoutine(_opponentManager, processAction);
    }

    private IEnumerator DrawStartingHandRoutine(PlayerManager playerManager, Action<CardManager> processAction)
    {
        for (int i = 0; i < 5; i++)
        {
            yield return playerManager.DrawCardRoutine(processAction);
        }
    }
}
