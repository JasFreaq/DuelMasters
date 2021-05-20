using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "Deck", order = 53)]
public class Deck : ScriptableObject
{
    [System.Serializable]
    struct CardComposition
    {
        public Card card;
        [Range(1, 4)] public int number;
    }

    [SerializeField] private List<CardComposition> _cardList = new List<CardComposition>();

    public List<Card> GetCards()
    {
        List<Card> cards = new List<Card>();
        foreach (CardComposition cardComposition in _cardList)
        {
            for (int i = 0; i < cardComposition.number; i++)
            {
                cards.Add(cardComposition.card);
            }
        }

        if (cards.Count < 40) 
            Debug.LogError("Deck has less than 40 cards");

        return cards;
    }
}
