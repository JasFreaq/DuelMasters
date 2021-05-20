using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckLayoutHandler : MonoBehaviour
{
    [SerializeField] private float _cardWidth = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _cardScale = 0.5f;
    [SerializeField] private CreatureCardManager _creaturePrefab;
    [SerializeField] private SpellCardManager _spellPrefab;

    private List<CardManager> _cards = new List<CardManager>();

    public void SetupDeck(List<Card> cardList)
    {
        float lastYPos = 0;

        foreach (Card cardData in cardList)
        {
            CardManager card = null;
            if (cardData is Creature)
            {
                card = Instantiate(_creaturePrefab, transform);
            }
            else if (cardData is Spell)
            {
                card = Instantiate(_spellPrefab, transform);
            }

            card.SetupCard(cardData);
            card.transform.localPosition = new Vector3(card.transform.localPosition.x,
                lastYPos -= _cardWidth * _cardScale, card.transform.localPosition.z);
            card.transform.localScale = Vector3.one * _cardScale;
            card.ActivateCardLayout();
            card.CardLayout.Canvas.gameObject.SetActive(false);
            card.name = cardData.Name;

            _cards.Add(card);
        }

        ShuffleCards();
    }

    public CardManager GetTopCard()
    {
        return _cards[_cards.Count - 1];
    }

    public CardManager RemoveTopCard()
    {
        CardManager card = GetTopCard();
        _cards.Remove(card);
        return card;
    }

    public void ShuffleCards()
    {
        int n = _cards.Count;
        int seed = System.DateTime.Now.Second + System.DateTime.Now.Minute + System.DateTime.Now.Hour +
                   Random.Range(0, 360);
        System.Random rNG = new System.Random(seed);
        while (n > 1)
        {
            int k = rNG.Next(n--);
            CardManager tempCard = _cards[n];
            _cards[n] = _cards[k];
            _cards[k] = tempCard;
        }

        float lastYPos = 0;
        foreach (CardManager card in _cards)
        {
            card.transform.localPosition = new Vector3(card.transform.localPosition.x,
                lastYPos -= _cardWidth * _cardScale, card.transform.localPosition.z);
        }
    }
}
