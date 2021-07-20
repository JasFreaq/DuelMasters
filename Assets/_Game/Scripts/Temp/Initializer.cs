using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private CardData cardData;

    private CardObject _cardObject;

    void Awake()
    {
        _cardObject = GetComponent<CardObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            _cardObject.SetupCard(new CardInstance(cardData));
        }
    }
}
