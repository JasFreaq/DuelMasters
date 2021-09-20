using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDataHandler : MonoBehaviour
{
    [SerializeField] private PlayerDataHandler _playerDataHandler;
    [SerializeField] private PlayerDataHandler _opponentDataHandler;
    
    #region Static Data Members

    private static GameDataHandler _Instance = null;

    public static GameDataHandler Instance
    {
        get
        {
            if (!_Instance)
                _Instance = FindObjectOfType<GameDataHandler>();
            return _Instance;
        }
    }

    #endregion

    private void Awake()
    {
        int count = FindObjectsOfType<GameDataHandler>().Length;
        if (count > 1)
            Destroy(gameObject);
        else
            _Instance = this;
    }

    public List<CardInstance> GenerateCardInstances(Deck deck)
    {
        List<CardInstance> cardInstances = new List<CardInstance>();
        foreach (CardData cardData in deck.GetCards())
        {
            cardInstances.Add(new CardInstance(cardData));
        }

        return cardInstances;
    }

    public void ShuffleCards(bool isPlayer)
    {
        PlayerDataHandler playerData = GetDataHandler(isPlayer);

        int n = playerData.CardsInDeck.Count;
        int seed = DateTime.Now.Second + DateTime.Now.Minute + DateTime.Now.Hour +
                   Random.Range(0, 360);
        System.Random rNG = new System.Random(seed);
        while (n > 1)
        {
            int k = rNG.Next(n--);
            CardObject tempCard = playerData.CardsInDeck[n];
            playerData.CardsInDeck[n] = playerData.CardsInDeck[k];
            playerData.CardsInDeck[k] = tempCard;
        }
    }

    public PlayerDataHandler GetDataHandler(bool isPlayer)
    {
        return isPlayer ? _playerDataHandler : _opponentDataHandler;
    }

    public List<CardBehaviour> GetZoneCards(bool isPlayer, CardZoneType zoneType)
    {
        PlayerDataHandler dataHandler = GetDataHandler(isPlayer);
        switch (zoneType)
        {
            case CardZoneType.Deck:
                return new List<CardBehaviour>(dataHandler.CardsInDeck);
            case CardZoneType.Hand:
                return new List<CardBehaviour>(dataHandler.CardsInHandList);
            case CardZoneType.Shields:
                return new List<CardBehaviour>(dataHandler.Shields);
            case CardZoneType.Graveyard:
                return new List<CardBehaviour>(dataHandler.CardsInGrave);
            case CardZoneType.ManaZone:
                return new List<CardBehaviour>(dataHandler.CardsInManaList);
            case CardZoneType.BattleZone:
                return new List<CardBehaviour>(dataHandler.CardsInBattleList);
        }

        return null;
    }

    public void CheckWhileConditions()
    {
        foreach (CreatureObject creatureObj in _playerDataHandler.CardsInBattle.Values)
        {
            if (creatureObj.CardInst.InstanceEffectHandler.HasWhileCondition)
                creatureObj.CardInst.InstanceEffectHandler.TriggerWhileCondition();
        }
        
        foreach (CreatureObject creatureObj in _opponentDataHandler.CardsInBattle.Values)
        {
            if (creatureObj.CardInst.InstanceEffectHandler.HasWhileCondition)
                creatureObj.CardInst.InstanceEffectHandler.TriggerWhileCondition();
        }
    }
}
