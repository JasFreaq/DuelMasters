using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleCardLayoutHandler : CompactCardLayoutHandler
{
    [SerializeField] private TextMeshProUGUI _creatureRaceText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private TextMeshProUGUI _powerTextShade;

    public override void SetupCard(CardData cardData)
    {
        base.SetupCard(cardData);

        CreatureData creatureData = (CreatureData)cardData;

        string raceStr = CardParams.StringFromRace(creatureData.Race[0]);
        if (creatureData.Race.Length == 2)
            raceStr += $" \" {CardParams.StringFromRace(creatureData.Race[1])}";
        _creatureRaceText.text = raceStr;

        _powerText.text = creatureData.Power.ToString();
        _powerTextShade.text = creatureData.Power.ToString();
    }
}
