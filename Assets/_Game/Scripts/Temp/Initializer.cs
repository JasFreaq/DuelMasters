using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private Card card;

    private CardManager _cardManager;

    void Awake()
    {
        _cardManager = GetComponent<CardManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            _cardManager.SetupCard(card);
            _cardManager.ActivateManaLayout();
        }
    }
}
