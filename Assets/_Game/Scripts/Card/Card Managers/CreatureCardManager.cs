using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CreatureCardManager : CardManager
{
    [SerializeField] private BattleCardLayoutHandler _battleCardLayoutHandler;

    public BattleCardLayoutHandler BattleLayout
    {
        get { return _battleCardLayoutHandler; }
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

    public override void SetGlow(bool enableGlow)
    {
        base.SetGlow(enableGlow);

        _battleCardLayoutHandler.SetGlow(_isGlowing);
    }

    public override void ToggleTap()
    {
        base.ToggleTap();

        _battleCardLayoutHandler.TappedOverlay.SetActive(_isTapped);
    }

    public override void SetGlowColor(bool play)
    {
        base.SetGlowColor(play);

        _battleCardLayoutHandler.SetGlowColor(play ? GameParamsHolder.Instance.PlayGlowColor
            : GameParamsHolder.Instance.HighlightGlowColor);
    }

    #endregion

    #region Functionality Methods

    public void AttackTarget(Transform targetTransform)
    {
        transform.DOMove(targetTransform.position, 0.5f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InCubic).OnComplete(() =>
        {

        });
    }

    #endregion
}
