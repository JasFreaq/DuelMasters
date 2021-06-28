using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardManager : MonoBehaviour
{
    [SerializeField] private bool _isPlayer = true;
    
    [Header("Layout")] 
    [SerializeField] private float _cardWidth = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float _cardScale = 0.5f;
    
    private PlayerDataHandler _playerData;
    
    float _lastYPos = 0;

    private void Start()
    {
        _playerData = GameDataHandler.Instance.GetDataHandler(_isPlayer);
    }

    public void AddCard(CardInstanceObject cardObj)
    {
        cardObj.transform.parent = transform;
        cardObj.transform.localPosition = new Vector3(0, _lastYPos += _cardWidth * _cardScale, 0);
        cardObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        cardObj.transform.localScale = Vector3.one * _cardScale;

        if (_playerData.CardsInGrave.Count > 0)
        {
            _playerData.CardsInGrave[_playerData.CardsInGrave.Count - 1].HoverPreviewHandler.PreviewEnabled = false;
        }

        cardObj.HoverPreviewHandler.PreviewEnabled = true;
        _playerData.CardsInGrave.Add(cardObj);
        cardObj.CurrentZone = CardZone.Graveyard;
    }

    public CardInstanceObject RemoveCardAtIndex(int index)
    {
        if (index == _playerData.CardsInGrave.Count - 1)
        {
            _playerData.CardsInGrave[index].HoverPreviewHandler.PreviewEnabled = false;
            if (_playerData.CardsInGrave.Count > 1)
            {
                _playerData.CardsInGrave[index - 1].HoverPreviewHandler.PreviewEnabled = true;
            }
        }

        CardInstanceObject card = _playerData.CardsInGrave[index];
        _playerData.CardsInGrave.RemoveAt(index);
        ArrangeCards();
        return card;
    }

    private void ArrangeCards()
    {
        float lastYPos = 0;

        foreach (CardInstanceObject card in _playerData.CardsInGrave)
        {
            card.transform.localPosition = new Vector3(0, lastYPos += _cardWidth * _cardScale, 0);
            card.transform.localRotation = Quaternion.Euler(Vector3.zero);
            card.transform.localScale = Vector3.one * _cardScale;
        }

        _lastYPos = lastYPos;
    }
}
