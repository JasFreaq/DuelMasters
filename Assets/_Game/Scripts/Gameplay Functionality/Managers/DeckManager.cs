using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DeckManager : MonoBehaviour
{
    [SerializeField] private Deck _deck;

    private DeckLayoutHandler _deckLayoutHandler;

    private void Awake()
    {
        _deckLayoutHandler = GetComponent<DeckLayoutHandler>();
    }

    private void Start()
    {
        _deckLayoutHandler.SetupDeck(_deck.GetCards());
    }

    public CardLayoutHandler GetTopCard()
    {
        return _deckLayoutHandler.GetTopCardLayout();
    }

    public CardLayoutHandler RemoveTopCard()
    {
        CardLayoutHandler cardLayout = _deckLayoutHandler.RemoveTopCardLayout();
        return cardLayout;
    }
}
