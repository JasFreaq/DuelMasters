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

    public override void SetupCard(Card card)
    {
        base.SetupCard(card);

        _battleCardLayoutHandler.SetupCard(card);
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

    public void ActivateBattleLayout()
    {
        _cardLayoutHandler.gameObject.SetActive(false);
        _manaCardLayoutHandler.gameObject.SetActive(false);
        _battleCardLayoutHandler.gameObject.SetActive(true);

        ActivateCompactCardCollider();
    }

    protected override void SetGlow(bool enableGlow)
    {
        base.SetGlow(enableGlow);

        _battleCardLayoutHandler.SetGlow(enableGlow);
    }
}
