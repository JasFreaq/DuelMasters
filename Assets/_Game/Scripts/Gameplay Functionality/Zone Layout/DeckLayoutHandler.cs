using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckLayoutHandler : MonoBehaviour
{
    [SerializeField] private Deck _deck;
    [SerializeField] private float _cardWidth = 0.1f;
    [SerializeField] private CreatureLayoutHandler _creaturePrefab;
    [SerializeField] private SpellLayoutHandler _spellPrefab;

    private List<CardLayoutHandler> _cards = new List<CardLayoutHandler>();

    void Start()
    {
        SetupDeck();
    }

    private void SetupDeck()
    {
        float lastYPos = 0;
        List<CardData> _cardList = _deck.GetCards();

        foreach (CardData cardData in _cardList)
        {
            CardLayoutHandler card = null;
            if (cardData is CreatureData)
            {
                card = Instantiate(_creaturePrefab, transform);
            }
            else if (cardData is SpellData)
            {
                card = Instantiate(_spellPrefab, transform);
            }

            card.SetupCard(cardData);
            card.transform.localPosition = new Vector3(card.transform.localPosition.x,
                lastYPos -= _cardWidth, card.transform.localPosition.z);
            card.Canvas.gameObject.SetActive(false);
            card.name = cardData.Name;

            _cards.Add(card);
        }

        Shuffle();
    }

    public void Shuffle()
    {
        int n = _cards.Count;
        int seed = System.DateTime.Now.Second + System.DateTime.Now.Minute + System.DateTime.Now.Hour +
                   Random.Range(0, 360);
        System.Random rNG = new System.Random(seed);
        while (n > 1)
        {
            int k = rNG.Next(n--);
            CardLayoutHandler tempCard = _cards[n];
            _cards[n] = _cards[k];
            _cards[k] = tempCard;
        }

        float lastYPos = 0;
        foreach (CardLayoutHandler card in _cards)
        {
            card.transform.localPosition = new Vector3(card.transform.localPosition.x,
                lastYPos -= _cardWidth, card.transform.localPosition.z);
        }
    }
}
