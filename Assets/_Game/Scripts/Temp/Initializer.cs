using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private CardData _cardData;

    private CardLayoutHandler _cardLayoutHandler;
    private CompactCardLayoutHandler _compactCardLayoutHandler;

    void Start()
    {
        _cardLayoutHandler = GetComponent<CardLayoutHandler>();
        _compactCardLayoutHandler = GetComponent<CompactCardLayoutHandler>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (_cardLayoutHandler) 
                _cardLayoutHandler.SetupCard(_cardData);
            else if (_compactCardLayoutHandler) 
                _compactCardLayoutHandler.SetupCard(_cardData);
            else
                Debug.LogError("Initializer missing both CardObject and CompactCardObject");
        }
    }
}
