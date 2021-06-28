using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CreatureInstanceObject : CardInstanceObject
{
    [SerializeField] private BattleCardLayoutHandler _battleCardLayoutHandler;

    public BattleCardLayoutHandler BattleLayout
    {
        get { return _battleCardLayoutHandler; }
    }
    
    public override void ProcessMouseDown()
    {
        base.ProcessMouseDown();

        if (_currentZone == CardZone.BattleZone)
        {
            ToggleSelection();
            ProcessMouseExit();
            ProcessMouseEnter();
        }
    }

    #region Setup Methods

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

    #endregion

    #region State Methods

    public override void SetHighlight(bool highlight)
    {
        base.SetHighlight(highlight);

        _battleCardLayoutHandler.SetGlow(_isHighlighted);
    }

    public override void ToggleTap()
    {
        base.ToggleTap();

        _battleCardLayoutHandler.TappedOverlay.SetActive(_isTapped);
    }

    public override void SetHighlightColor(bool play)
    {
        base.SetHighlightColor(play);

        _battleCardLayoutHandler.SetHighlightColor(play ? GameParamsHolder.Instance.PlayHighlightColor
            : GameParamsHolder.Instance.BaseHighlightColor);
    }

    #endregion

    #region Functionality Methods
    
    public void Attack(Transform attackTarget)
    {
        transform.DOMove(attackTarget.position, 0.5f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InCubic);
    }

    #endregion
}
