using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EffectsManager _playerEffectsManager;
    [SerializeField] private EffectsManager _opponentEffectsManager;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        StartCoroutine(_playerEffectsManager.SetupShieldsRoutine());
        yield break;
        //yield return StartCoroutine(_opponentEffectsManager.SetupShieldsRoutine());
        
        //StartCoroutine(DrawStartingHandRoutine(_playerEffectsManager));
        //yield return StartCoroutine(DrawStartingHandRoutine(_opponentEffectsManager));
    }

    private IEnumerator DrawStartingHandRoutine(EffectsManager effectsManager)
    {
        for (int i = 0; i < 5; i++)
        {
            yield return StartCoroutine(effectsManager.DrawCardRoutine());
        }
    }
}
