using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellObject : CardObject
{
    protected override void SetupCard()
    {
        CardFrameData cardFrameData = GameParamsHolder.Instance.GetCardFrameData(false, _cardInst.CardData.Civilization);
        _cardLayoutHandler.SetupCard(_cardInst.CardData, cardFrameData);
        previewHandler.SetupCard(_cardInst.CardData, cardFrameData);

        CompactCardFrameData compactFrameData = GameParamsHolder.Instance.GetCompactFrameData(false, _cardInst.CardData.Civilization);
        _manaCardLayoutHandler.SetupCard(_cardInst.CardData, compactFrameData);
    }
}
