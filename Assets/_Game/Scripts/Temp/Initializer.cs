using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private CardData cardData;

    private CardInstanceObject _cardInstanceObject;

    void Awake()
    {
        _cardInstanceObject = GetComponent<CardInstanceObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            _cardInstanceObject.SetupCard(cardData);
            _cardInstanceObject.ActivateManaLayout();
        }
    }
}
