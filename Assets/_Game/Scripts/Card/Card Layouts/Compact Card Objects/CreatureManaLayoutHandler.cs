using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureManaLayoutHandler : ManaCardLayoutHandler
{
    [SerializeField] private TextMeshProUGUI _creatureRaceText;

    public override void SetupCard(CardData cardData, CompactCardFrameData compactFrameData)
    {
        base.SetupCard(cardData, compactFrameData);

        CreatureData creatureData = (CreatureData)cardData;

        string raceStr = CardParams.StringFromRace(creatureData.Race[0]);
        if (creatureData.Race.Length == 2)
            raceStr += $" \" {CardParams.StringFromRace(creatureData.Race[1])}";
        _creatureRaceText.text = raceStr;
    }
}
