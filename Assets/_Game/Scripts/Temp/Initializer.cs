using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private CardData _cardData;

    private CardManager _cardManager;

    void Awake()
    {
        _cardManager = GetComponent<CardManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            _cardManager.SetupCard(_cardData);
            _cardManager.ActivateManaLayout();
        }
    }
}
