using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CreatureObject : CardObject
{
    [SerializeField] private BattleCardLayoutHandler _battleCardLayoutHandler;

    private List<CreatureObject> _creaturesUnderEvolution = new List<CreatureObject>();

    public new CreatureData CardData
    {
        get { return (CreatureData) _cardInst.CardData; }
    }

    public new CreatureLayoutHandler CardLayout
    {
        get { return (CreatureLayoutHandler) _cardLayoutHandler; }
    }

    public bool IsEvolutionCreature
    {
        get { return _cardInst.CardData.CardType == CardParams.CardType.EvolutionCreature; }
    }
    
    public List<CreatureObject> CreaturesUnderEvolution
    {
        get { return _creaturesUnderEvolution; }
    }

    public BattleCardLayoutHandler BattleLayout
    {
        get { return _battleCardLayoutHandler; }
    }

    public Canvas BattleLayoutCanvas
    {
        get { return _battleCardLayoutHandler.Canvas; }
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
        CardFrameData cardFrameData = GameParamsHolder.Instance.GetCardFrameData(true, _cardInst.CardData.Civilization);
        _cardLayoutHandler.SetupCard(this, cardFrameData);
        _previewLayoutHandler.SetupCard(this, cardFrameData);

        CompactCardFrameData compactFrameData = GameParamsHolder.Instance.GetCompactFrameData(true, _cardInst.CardData.Civilization);
        _manaCardLayoutHandler.SetupCard(_cardInst.CardData, compactFrameData);
        _battleCardLayoutHandler.SetupCard(_cardInst.CardData, compactFrameData);

        base.SetupCard();
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

    public void AddPlusToPower()
    {
        ((CreatureLayoutHandler) _cardLayoutHandler).AddPlusToPower();
        ((CreatureLayoutHandler)_previewLayoutHandler.CardLayout).AddPlusToPower();
        _battleCardLayoutHandler.AddPlusToPower();
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

    public override void SetHighlightColor(bool baseColor)
    {
        base.SetHighlightColor(baseColor);

        _battleCardLayoutHandler.SetHighlightColor(baseColor ? GameParamsHolder.Instance.BaseHighlightColor
            : GameParamsHolder.Instance.PlayHighlightColor);
    }

    #endregion
}
