using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private CardData _cardData;

    private CardObject _cardObject;
    private CompactCardObject _compactCardObject;

    void Start()
    {
        _cardObject = GetComponent<CardObject>();
        _compactCardObject = GetComponent<CompactCardObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (_cardObject) 
                _cardObject.SetupCard(_cardData);
            else if (_compactCardObject) 
                _compactCardObject.SetupCard(_cardData);
            else
                Debug.LogError("Initializer missing both CardObject and CompactCardObject");
        }
    }
}
