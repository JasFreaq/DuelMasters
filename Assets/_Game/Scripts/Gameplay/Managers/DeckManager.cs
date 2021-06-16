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
    
    [Header("Layout")]
    [SerializeField] private float _cardWidth = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _cardScale = 0.5f;
    [SerializeField] private CreatureCardManager _creaturePrefab;
    [SerializeField] private SpellCardManager _spellPrefab;
    
    [Header("Transition")]
    [SerializeField] private float _fromTransitionTime = 1.5f;
    [SerializeField] private Transform _intermediateHolder;

    #region Functionality Methods

    public void Initialize(Deck deck, Action<CardManager> processAction, Action<CardManager> selectAction)
    {
        List<Card> cardList = deck.GetCards();

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
            card.name = cardData.Name;

            card.transform.localPosition = new Vector3(card.transform.localPosition.x,
                lastYPos -= _cardWidth * _cardScale, card.transform.localPosition.z);
            card.transform.localScale = Vector3.one * _cardScale;

            card.ActivateCardLayout();
            card.CardLayout.Canvas.gameObject.SetActive(false);

            card.RegisterOnProcessAction(processAction);
            card.RegisterOnSelect(selectAction);

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
        card.transform.DORotateQuaternion(_intermediateHolder.rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
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
