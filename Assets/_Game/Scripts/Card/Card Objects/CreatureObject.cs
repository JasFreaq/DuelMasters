using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CreatureObject : CardObject
{
    [SerializeField] private BattleCardLayoutHandler _battleCardLayoutHandler;

    public BattleCardLayoutHandler BattleLayout
    {
        get { return _battleCardLayoutHandler; }
    }
    
    public override void ProcessMouseDown()
    {
        base.ProcessMouseDown();

        if (_cardInst.CurrentZone == CardZoneType.BattleZone)
        {
            _isSelected = !_isSelected;
            ProcessMouseExit();
            ProcessMouseEnter();
        }
    }

    #region Setup Methods

    protected override void SetupCard()
    {
        base.SetupCard();

        _battleCardLayoutHandler.SetupCard(_cardInst.CardData);
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

    #endregion

    #region State Methods

    public override void SetHighlight(bool highlight)
    {
        base.SetHighlight(highlight);

        _battleCardLayoutHandler.SetGlow(_isHighlighted);
    }

    public override void ToggleTapState()
    {
        base.ToggleTapState();

        _battleCardLayoutHandler.TappedOverlay.SetActive(_cardInst.IsTapped);
    }

    public override void SetHighlightColor(bool play)
    {
        base.SetHighlightColor(play);

        _battleCardLayoutHandler.SetHighlightColor(play ? GameParamsHolder.Instance.PlayHighlightColor
            : GameParamsHolder.Instance.BaseHighlightColor);
    }

    #endregion
}