using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureLayoutHandler : CardLayoutHandler
{
    [SerializeField] private GameObject _creatureText;
    [SerializeField] private TextMeshProUGUI _creatureRaceText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private TextMeshProUGUI _powerTextShade;
    [SerializeField] private GameObject _evolutionCreatureText;
    [SerializeField] private GameObject _evolutionIconImage;
    [SerializeField] private GameObject _vortexEvolutionIconImage;
    [SerializeField] private GameObject _survivorIconImage;
    [SerializeField] private GameObject _waveStrikerIconImage;

    public override void SetupCard(Card card)
    {
        base.SetupCard(card);

        Creature creature = (Creature) card;

        string raceStr = CardParams.StringFromRace(creature.Race[0]);
        if (creature.Race.Length == 2)
            raceStr += $" \" {CardParams.StringFromRace(creature.Race[1])}";
        _creatureRaceText.text = raceStr;

        _powerText.text = creature.Power.ToString();
        _powerTextShade.text = creature.Power.ToString();
    }

    protected override void SetupRules(string rulesText)
    {
        base.SetupRules(rulesText);
    }
}
