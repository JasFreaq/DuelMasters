using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleCardLayoutHandler : CompactCardLayoutHandler
{
    [SerializeField] private TextMeshProUGUI _creatureRaceText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private TextMeshProUGUI _powerTextShade;

    public override void SetupCard(Card card)
    {
        base.SetupCard(card);

        Creature creature = (Creature)card;

        string raceStr = CardParams.StringFromRace(creature.Race[0]);
        if (creature.Race.Length == 2)
            raceStr += $" \" {CardParams.StringFromRace(creature.Race[1])}";
        _creatureRaceText.text = raceStr;

        _powerText.text = creature.Power.ToString();
        _powerTextShade.text = creature.Power.ToString();
    }
}
