using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class DeckManager : MonoBehaviour
{
    [SerializeField] private bool _isPlayer = true;
    
    [Header("Layout")]
    [SerializeField] private float _cardWidth = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _cardScale = 0.5f;
    
    [Header("Transition")]
    [SerializeField] private float _fromTransitionTime = 1.5f;
    [SerializeField] private Transform _intermediateHolder;
    
    private PlayerDataHandler _playerData;
    
    private void Start()
    {
        _playerData = GameDataHandler.Instance.GetDataHandler(_isPlayer);
    }

    #region Functionality Methods

    public void GenerateCardObjects(List<CardInstance> cardsInsts, Action<CardObject> dragReleaseAction, Func<CardObject, Coroutine> sendToGraveFunc)
    {
        CreatureObject creaturePrefab = GameParamsHolder.Instance.CreaturePrefab;
        SpellObject spellPrefab = GameParamsHolder.Instance.SpellPrefab;

        foreach (CardInstance cardInst in cardsInsts) 
        {
            CardData cardData = cardInst.CardData;

            CardObject cardObj = null;
            if (cardData is CreatureData)
            {
                cardObj = Instantiate(creaturePrefab, transform);
            }
            else if (cardData is SpellData)
            {
                cardObj = Instantiate(spellPrefab, transform);
            }

            cardObj.Initialize(cardInst, _isPlayer);

            cardObj.RegisterOnDragRelease(dragReleaseAction);
            cardObj.RegisterOnSendToGrave(sendToGraveFunc);

            _playerData.CardsInDeck.Add(cardObj);
            cardInst.AssignCardObject(cardObj);
        }
    }
    
    public CardObject GetTopCard()
    {
        return _playerData.CardsInDeck[_playerData.CardsInDeck.Count - 1];
    }

    public CardObject RemoveTopCard()
    {
        CardObject cardObj = GetTopCard();
        _playerData.CardsInDeck.Remove(cardObj);
        return cardObj;
    }

    #endregion

    #region Transition Methods

    public IEnumerator MoveFromDeckRoutine(CardObject cardObj)
    {
        _playerData.CardsInDeck.Remove(cardObj);

        cardObj.transform.parent = null;
        cardObj.transform.DOMove(_intermediateHolder.position, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardObj.transform.DORotateQuaternion(_intermediateHolder.rotation, _fromTransitionTime).SetEase(Ease.OutQuint);
        cardObj.transform.DOScale(Vector3.one, _fromTransitionTime).SetEase(Ease.OutQuint);

        yield return new WaitForSeconds(_fromTransitionTime);

        ArrangeCards();
    }

    #endregion

    #region Layout Methods

    public void ArrangeCards()
    {
        float lastYPos = 0;
        foreach (CardObject cardObj in _playerData.CardsInDeck)
        {
            cardObj.transform.localPosition = new Vector3(cardObj.transform.localPosition.x,
                lastYPos -= _cardWidth * _cardScale, cardObj.transform.localPosition.z);
            cardObj.transform.localScale = Vector3.one * _cardScale;
        }
    }

    #endregion
}
