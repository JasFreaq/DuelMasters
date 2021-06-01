using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStep
{
    BeginStep,
    UntapStep,
    StartOfTurnStep,
    DrawStep,
    ChargeStep,
    AttackStep,
    EndStep
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private ClientManager _clientManager;
    [SerializeField] private ActionMenu _actionMenu;
    [SerializeField] private Deck _playerDeck;
    [SerializeField] private Deck _opponentDeck;

    private GameStep _currentStep;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(GameLoopRoutine());
    }

    private IEnumerator GameLoopRoutine()
    {
        yield return _clientManager.GameStartRoutine(ProcessGameActions, _playerDeck, _opponentDeck);
    }

    private void ProcessGameActions(CardManager card)
    {
        //_playerManager.StartCoroutine(_playerManager.ChargeManaRoutine(card.transform.GetSiblingIndex()));
    }
}
