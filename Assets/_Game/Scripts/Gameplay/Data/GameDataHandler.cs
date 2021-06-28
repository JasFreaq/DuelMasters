using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataHandler : MonoBehaviour
{
    [SerializeField] private PlayerDataHandler _playerDataHandler;
    [SerializeField] private PlayerDataHandler _opponentDataHandler;
    
    #region Static Data Members

    private static GameDataHandler _Instance = null;

    public static GameDataHandler Instance
    {
        get
        {
            if (!_Instance)
                _Instance = FindObjectOfType<GameDataHandler>();
            return _Instance;
        }
    }

    #endregion

    private void Awake()
    {
        int count = FindObjectsOfType<GameDataHandler>().Length;
        if (count > 1)
            Destroy(gameObject);
        else
            _Instance = this;
    }

    public PlayerDataHandler GetDataHandler(bool isPlayer)
    {
        return isPlayer ? _playerDataHandler : _opponentDataHandler;
    }
}
