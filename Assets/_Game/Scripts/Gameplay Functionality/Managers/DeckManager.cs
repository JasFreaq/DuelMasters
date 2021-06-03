using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class DeckManager : MonoBehaviour
{
    [SerializeField] private PlayerDataHandler _playerData;

    [Header("Transition")]
    [SerializeField] private Transform _intermediateHolder;
    [SerializeField] private bool _isPlayer = true;
    [SerializeField] private float _fromTransitionTime = 1.5f;

    [Header("Layout")]
    [SerializeField] private float _cardWidth = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _cardScale = 0.5f;
    [SerializeField] private CreatureCardManager _creaturePrefab;
    [SerializeField] private SpellCardManager _spellPrefab;

    #region Functionality Methods

    public void Initialize(Deck deck, Action<CardManager> action)
    {
        SetupDeck(deck.GetCards(), action);
    }

    private void SetupDeck(List<Card> cardList, Action<CardManager> action)
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
            card.RegisterOnProcessAction(action);

            _playerData.CardsInDeck.Add(card);
        }

        ShuffleCards();
    }

    public CardManager GetTopCard()
    {
        return _playerData.CardsInDeck[_playerData.CardsInDeck.Count - 1];
    }

    public CardManager RemoveTopCard()
    {
        CardManager card = GetTopCard();
        _playerData.CardsInDeck.Remove(card);
        return card;
    }

    #endregion

    #region Transition Methods

    public IEnumerator MoveFromDeckRoutine(CardManager card)
    {
        card.transform.parent = null;
        card.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        if (_isPlayer)
            card.transform.DORotate(_intermediateHolder.rotation.eulerAngles, _fromTransitionTime).SetEase(Ease.OutQuint);
        card.transform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);
    }

    #endregion

    #region Layout Methods

    private void ShuffleCards()
    {
        int n = _playerData.CardsInDeck.Count;
        int seed = DateTime.Now.Second + DateTime.Now.Minute + DateTime.Now.Hour +
                   Random.Range(0, 360);
        System.Random rNG = new System.Random(seed);
        while (n > 1)
        {
            int k = rNG.Next(n--);
            CardManager tempCard = _playerData.CardsInDeck[n];
            _playerData.CardsInDeck[n] = _playerData.CardsInDeck[k];
            _playerData.CardsInDeck[k] = tempCard;
        }

        float lastYPos = 0;
        foreach (CardManager card in _playerData.CardsInDeck)
        {
            card.transform.localPosition = new Vector3(card.transform.localPosition.x,
                lastYPos -= _cardWidth * _cardScale, card.transform.localPosition.z);
        }
    }

    #endregion
}
