using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerManager _opponentManager;

    [SerializeField] private Deck _playerDeck;
    [SerializeField] private Deck _opponentDeck;

    public void Start()
    {
        _playerManager.Initialize(_playerDeck);
        _opponentManager.Initialize(_opponentDeck);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(StartGameRoutine());
    }
    
    private IEnumerator StartGameRoutine()
    {
        _playerManager.SetupShields();
        yield return _opponentManager.SetupShields();
        
        StartCoroutine(DrawStartingHandRoutine(_playerManager));
        yield return StartCoroutine(DrawStartingHandRoutine(_opponentManager));
    }

    private IEnumerator DrawStartingHandRoutine(PlayerManager playerManager)
    {
        for (int i = 0; i < 5; i++)
        {
            yield return StartCoroutine(playerManager.DrawCardRoutine());
        }
    }
}
