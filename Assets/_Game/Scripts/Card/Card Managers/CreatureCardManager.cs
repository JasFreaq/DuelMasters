using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCardManager : CardManager
{
    [SerializeField] private BattleCardLayoutHandler _battleCardLayoutHandler;

    public BattleCardLayoutHandler BattleLayout
    {
        get { return _battleCardLayoutHandler; }
    }

    public override void SetupCard(CardData cardData, bool considerAsDataObject = false)
    {
        base.SetupCard(cardData);

        if (!considerAsDataObject) 
        {
            _battleCardLayoutHandler.SetupCard(cardData);
        }
    }

    public override void ActivateCardLayout()
    {
        base.ActivateCardLayout();
        _battleCardLayoutHandler.gameObject.SetActive(false);
    }

    public override void ActivateManaLayout()
    {
        base.ActivateManaLayout();
        _battleCardLayoutHandler.gameObject.SetActive(false);
    }

    public void ActicateBattleLayout()
    {
        _cardLayoutHandler.gameObject.SetActive(false);
        _manaCardLayoutHandler.gameObject.SetActive(false);
        _battleCardLayoutHandler.gameObject.SetActive(true);

        ActivateCompactCardCollider();
    }
}
