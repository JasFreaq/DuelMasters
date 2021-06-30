using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class DeckManager : MonoBehaviour
{
    [SerializeField] private bool _isPlayer = true;
    
    [Header("Layout")]
    [SerializeField] private float _cardWidth = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _cardScale = 0.5f;
    [SerializeField] private CreatureInstanceObject _creaturePrefab;
    [SerializeField] private SpellInstanceObject _spellPrefab;
    
    [Header("Transition")]
    [SerializeField] private float _fromTransitionTime = 1.5f;
    [SerializeField] private Transform _intermediateHolder;

    private PlayerDataHandler _playerData;

    private void Start()
    {
        _playerData = GameDataHandler.Instance.GetDataHandler(_isPlayer);
    }

    #region Functionality Methods

    public void Initialize(Deck deck, Action<CardInstanceObject> processAction)
    {
        List<CardData> cardList = deck.GetCards();

        float lastYPos = 0;

        foreach (CardData cardData in cardList)
        {
            CardInstanceObject cardObj = null;
            if (cardData is CreatureData)
            {
                cardObj = Instantiate(_creaturePrefab, transform);
            }
            else if (cardData is SpellData)
            {
                cardObj = Instantiate(_spellPrefab, transform);
            }

            cardObj.SetupCard(cardData);
            cardObj.name = cardData.Name;
            cardObj.IsPlayer = _isPlayer;

            cardObj.transform.localPosition = new Vector3(cardObj.transform.localPosition.x,
                lastYPos -= _cardWidth * _cardScale, cardObj.transform.localPosition.z);
            cardObj.transform.localScale = Vector3.one * _cardScale;

            cardObj.ActivateCardLayout();
            cardObj.CardLayout.Canvas.gameObject.SetActive(false);

            cardObj.RegisterOnProcessAction(processAction);

            _playerData.CardsInDeck.Add(cardObj);
            cardObj.CurrentZone = CardZone.Deck;
        }

        ShuffleCards();
    }
    
    public CardInstanceObject GetTopCard()
    {
        return _playerData.CardsInDeck[_playerData.CardsInDeck.Count - 1];
    }

    public CardInstanceObject RemoveTopCard()
    {
        CardInstanceObject card = GetTopCard();
        _playerData.CardsInDeck.Remove(card);
        return card;
    }

    #endregion

    #region Transition Methods

    public IEnumerator MoveFromDeckRoutine(CardInstanceObject card)
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
            CardInstanceObject tempCard = _playerData.CardsInDeck[n];
            _playerData.CardsInDeck[n] = _playerData.CardsInDeck[k];
            _playerData.CardsInDeck[k] = tempCard;
        }

        float lastYPos = 0;
        foreach (CardInstanceObject card in _playerData.CardsInDeck)
        {
            card.transform.localPosition = new Vector3(card.transform.localPosition.x,
                lastYPos -= _cardWidth * _cardScale, card.transform.localPosition.z);
        }
    }

    #endregion
}
