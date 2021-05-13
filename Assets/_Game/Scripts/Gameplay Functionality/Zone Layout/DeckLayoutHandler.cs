using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckLayoutHandler : MonoBehaviour
{
    [SerializeField] private float _cardWidth = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _cardScale = 0.5f;
    [SerializeField] private CreatureLayoutHandler _creaturePrefab;
    [SerializeField] private SpellLayoutHandler _spellPrefab;

    private List<CardLayoutHandler> _cardLayouts = new List<CardLayoutHandler>();

    public void SetupDeck(List<CardData> cardList)
    {
        float lastYPos = 0;

        foreach (CardData cardData in cardList)
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
                lastYPos -= _cardWidth * _cardScale, card.transform.localPosition.z);
            card.transform.localScale = Vector3.one * _cardScale;
            card.Canvas.gameObject.SetActive(false);
            card.name = cardData.Name;

            _cardLayouts.Add(card);
        }

        ShuffleCards();
    }

    public CardLayoutHandler GetTopCardLayout()
    {
        return _cardLayouts[_cardLayouts.Count - 1];
    }

    public CardLayoutHandler RemoveTopCardLayout()
    {
        CardLayoutHandler cardLayout = GetTopCardLayout();
        _cardLayouts.Remove(cardLayout);
        return cardLayout;
    }

    public void ShuffleCards()
    {
        int n = _cardLayouts.Count;
        int seed = System.DateTime.Now.Second + System.DateTime.Now.Minute + System.DateTime.Now.Hour +
                   Random.Range(0, 360);
        System.Random rNG = new System.Random(seed);
        while (n > 1)
        {
            int k = rNG.Next(n--);
            CardLayoutHandler tempCard = _cardLayouts[n];
            _cardLayouts[n] = _cardLayouts[k];
            _cardLayouts[k] = tempCard;
        }

        float lastYPos = 0;
        foreach (CardLayoutHandler card in _cardLayouts)
        {
            card.transform.localPosition = new Vector3(card.transform.localPosition.x,
                lastYPos -= _cardWidth * _cardScale, card.transform.localPosition.z);
        }
    }
}
