using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] private Card card;

    private CardInstanceObject _cardInstanceObject;

    void Awake()
    {
        _cardInstanceObject = GetComponent<CardInstanceObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            _cardInstanceObject.SetupCard(card);
            _cardInstanceObject.ActivateManaLayout();
        }
    }
}
